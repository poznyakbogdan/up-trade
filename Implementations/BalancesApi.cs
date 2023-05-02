using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Implementations;

public class BalancesApi
{
    private const string BalancesUrl = "balances";
    private readonly HttpClient _httpClient;

    public BalancesApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<string, decimal>> GetBalances(IEnumerable<string> addresses)
    {
        var stringContent = JsonSerializer.Serialize(addresses.ToList());
        var response = await _httpClient.PostAsync(BalancesUrl, new StringContent(stringContent, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, decimal>>(responseContent);
        return result;
    }
}