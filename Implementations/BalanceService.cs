using Interfaces;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Polly;

namespace Implementations;

public class BalanceService : IBalanceService
{
    private class InnerRequestErrorException : Exception
    {
        
    }
    
    private readonly IClient _client;
    private readonly IWeb3 _web3;
    private readonly ILogger<BalanceService> _logger;
    
    private readonly AsyncPolicy _batchRequestRetryPolicy = Policy
        .Handle<InnerRequestErrorException>()
        .WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(1 * retryAttempt)
        );
    
    public BalanceService(IWeb3 web3, ILogger<BalanceService> logger)
    {
        _web3 = web3;
        _client = _web3.Client;
        _logger = logger;
    }

    public async Task<Dictionary<string, decimal>> GetBalances(IEnumerable<string> addresses)
    {
        var batches = CreateBatches(addresses.ToList());
        var result = new RpcRequestResponseBatch();
        var withError = new List<string>();
        
        foreach (var batch in batches)
        {
            var (ok, error) = await SendRequest(batch);
            result.BatchItems.AddRange(ok);
            withError.AddRange(error.Select(x => (string)x.RpcRequestMessage.Id));
        }
        
        try
        {
            while (withError.Any())
            {
                _logger.LogInformation("Responses with error: {count}", withError.Count);
                batches = CreateBatches(withError);
                foreach (var batch in batches)
                {
                    await _batchRequestRetryPolicy.ExecuteAsync(async () =>
                    {
                        var (ok, errored) = await SendRequest(batch);
                        withError = withError.Except(ok.Select(x => (string)x.RpcRequestMessage.Id)).ToList();
                        result.BatchItems.AddRange(ok);
                        if (!ok.Any()) throw new InnerRequestErrorException();
                    });
                } 
            }        
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during fetching balances");
            throw;
        }

        return BuildResult(result);
    }

    private async Task<(List<IRpcRequestResponseBatchItem>, List<IRpcRequestResponseBatchItem>)> SendRequest(RpcRequestResponseBatch batch) 
    {
        var batchResult = await _client.SendBatchRequestAsync(batch);
        
        return (
            batchResult.BatchItems.Where(x => !x.HasError).ToList(),
            batchResult.BatchItems.Where(x => x.HasError).ToList());
    }

    private Dictionary<string, decimal> BuildResult(RpcRequestResponseBatch batch)
    {
        var res =  batch.BatchItems
            .Cast<RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>>()
            .Select(x => ((string)x.RpcRequestMessage.Id, x.Response.Value.ToDecimal()))
            .ToDictionary(x => x.Item1, x => x.Item2);

        return res;
    }
    
    private List<RpcRequestResponseBatch> CreateBatches(List<string> addresses, int batchSize = 1000)
    {
        var items = new List<IRpcRequestResponseBatchItem>();

        foreach (var address in addresses)
        {
            var item = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>(
                (EthGetBalance)_web3.Eth.GetBalance, 
                _web3.Eth.GetBalance
                    .BuildRequest(address, new BlockParameter(), address));
            items.Add(item);
        }

        return items.Batch(batchSize)
            .Select(x => new RpcRequestResponseBatch{BatchItems = x.ToList()})
            .ToList();
    }
}