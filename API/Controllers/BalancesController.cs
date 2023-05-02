using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class BalancesController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    
    public BalancesController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Get([FromBody] List<string> addresses)
    {
        // TODO addresses validation
        var result = await _balanceService.GetBalances(addresses);
        return Ok(result);
    }
}