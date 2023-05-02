using DAL;
using Implementations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebUI.Pages.Shared;

namespace WebUI.Pages;

public class Balances : PageModel
{
    public List<BalancesTable.BalanceModel> Models { get; set; }
    private readonly IDesignTimeDbContextFactory<AppDbContext> _factory;
    private readonly BalancesApi _balancesApi;

    public Balances(IDesignTimeDbContextFactory<AppDbContext> factory, BalancesApi balancesApi)
    {
        _factory = factory;
        _balancesApi = balancesApi;
    }

    public async Task OnGet()
    {
        await using var context = _factory.CreateDbContext(new[] { "" });
        var wallets = await context.Wallets.AsNoTracking().ToListAsync();
        var balances = await _balancesApi.GetBalances(wallets.Select(x => x.Address).Distinct());
        Models = wallets.Select(x => new BalancesTable.BalanceModel
        {
            Id = x.Id,
            Address = x.Address,
            Balance = balances[x.Address]
        }).ToList();
    }
}