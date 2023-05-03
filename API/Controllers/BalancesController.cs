using API.Models;
using API.Validation;
using FluentValidation;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class BalancesController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    private readonly IValidator<PostBalances> _validator;

    public BalancesController(IBalanceService balanceService, IValidator<PostBalances> validator)
    {
        _balanceService = balanceService;
        _validator = validator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Get([FromBody] PostBalances addresses)
    {
        var validationResult = await _validator.ValidateAsync(addresses);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest();
        }

        var result = await _balanceService.GetBalances(addresses);
        return Ok(result);
    }
}