namespace Interfaces;

public interface IBalanceService
{
    Task<Dictionary<string, decimal>> GetBalances(IEnumerable<string> addresses);
}