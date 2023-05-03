using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Implementations;

public class BalancesApi
{
    private const string BalancesUrl = "balances";
    private readonly HttpClient _httpClient;
    private readonly ILogger<BalancesApi> _logger;

    public BalancesApi(HttpClient httpClient, ILogger<BalancesApi> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Dictionary<string, decimal>> GetBalances(IEnumerable<string> addresses)
    {
        var stringContent = JsonSerializer.Serialize(addresses.ToList());
        var response = await _httpClient.PostAsync(BalancesUrl, new StringContent(stringContent, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(await response.Content.ReadAsStringAsync());
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, decimal>>(responseContent);
        return result;
    }
}