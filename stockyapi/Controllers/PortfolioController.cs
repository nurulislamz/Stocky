using stockyapi.Requests;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using stockyapi.Responses;

namespace stockyapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PortfolioController> _logger;

    public PortfolioController(
        ILogger<PortfolioController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("portfolio")]
    public async Task<ActionResult<GetPortfolioResponse>> GetPortfolio([FromBody] GetPortfolioRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("buy")]
    public async Task<ActionResult<BuyTickerResponse>> Buy([FromBody] BuyTickerRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("sell")]
    public async Task<ActionResult<SellTickerResponse>> Sell([FromBody] SellTickerRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
