using stockyapi.Requests;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace stockyapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly ILogger<AuthController> _logger;

  public PortfolioController(
    ILogger<AuthController> logger,
    IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [HttpGet("portfolio")]
  public async Task<IActionResult> GetPortfolio([FromBody] GetPortfolioRequest request, CancellationToken cancellationToken)
  {
    var response = await _mediator.Send(request, cancellationToken);

    if (!response.Success)
       return Unauthorized(new { error = response.Error });

    return Ok(new { message = "Login successful", token = response.Token });
  }

  [HttpPost("buy")]
  public async Task<IActionResult> Buy([FromBody] BuyStockRequest request, CancellationToken cancellationToken)
  {
      var response = await _mediator.Send(request, cancellationToken);

      if (!response.Success)
        return BadRequest(new { error = response.Error });

      return Ok(new { message = "Registration successful", token = response.Token });
  }

  [HttpPost("sell")]
  public async Task<IActionResult> Sell([FromBody] BuyStockRequest request, CancellationToken cancellationToken)
  {
      var response = await _mediator.Send(request, cancellationToken);

      if (!response.Success)
        return BadRequest(new { error = response.Error });

      return Ok(new { message = "Registration successful", token = response.Token });
  }
}
