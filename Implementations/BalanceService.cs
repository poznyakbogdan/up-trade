using Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;

namespace Implementations;

public class BalanceService : IBalanceService
{
    private readonly IClient _client;
    private readonly IWeb3 _web3;

    public BalanceService(IWeb3 web3)
    {
        _web3 = web3;
        _client = _web3.Client;
    }

    public async Task<Dictionary<string, decimal>> GetBalances(IEnumerable<string> addresses)
    {
        var batches = CreateBatches(addresses.ToList());
        var result = new RpcRequestResponseBatch();
        foreach (var batch in batches)
        {
            var batchResult = await _client.SendBatchRequestAsync(batch);
            result.BatchItems.AddRange(batchResult.BatchItems);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
        return BuildResult(result);
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