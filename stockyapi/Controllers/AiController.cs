using stockyapi.Requests;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using stockyapi.Responses;
using OpenAI;

namespace stockyapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly ILogger<AiController> _logger;

  public AiController(
    ILogger<AiController> logger,
    IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [HttpPost("message")]
  public async Task<ActionResult<AiResponse>> Message([FromBody] AiRequest request, CancellationToken cancellationToken)
  {
    var response = await _mediator.Send(request, cancellationToken);

    return Ok(response);
  }
}