using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Application.Portfolio;
using stockyapi.Middleware;
using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Controllers;

/// <summary>
/// Controller for managing user funds.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FundsController : BaseController
{
    private readonly ILogger<PortfolioController> _logger;
    private readonly IFundsApi _fundsApi;

    public FundsController(
        ILogger<PortfolioController> logger,
        IFundsApi fundsApi)
    {
        _logger = logger;
        _fundsApi = fundsApi;
    }

    /// <summary>
    /// Retrieves the current funds for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current funds details.</returns>
    [HttpGet("get")]
    public async Task<ActionResult<FundsResponse>> GetFunds(CancellationToken cancellationToken)
    {
        var response = await _fundsApi.GetFunds(cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Adds funds to the authenticated user's account.
    /// </summary>
    /// <param name="request">The request containing the amount to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated funds details.</returns>
    [HttpPut("add")]
    public async Task<ActionResult<FundsResponse>> AddFunds([FromBody] DepositFundsRequest request, CancellationToken cancellationToken)
    {
        var response = await _fundsApi.DepositFunds(request, cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Subtracts funds from the authenticated user's account.
    /// </summary>
    /// <param name="request">The request containing the amount to subtract.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated funds details.</returns>
    [HttpPut("subtract")]
    public async Task<ActionResult<FundsResponse>> SubtractFunds([FromBody] WithdrawFundsRequest request, CancellationToken cancellationToken)
    {
        var response = await _fundsApi.WithdrawFunds(request, cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    // TODO: Get fund transaction Model history
}
