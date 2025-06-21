using stockyapi.Requests;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using stockyapi.Responses;

namespace stockyapi.Controllers;

[AllowAnonymous] 
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly ILogger<AuthController> _logger;

  public AuthController(
    ILogger<AuthController> logger,
    IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [HttpPost("login")] 
  public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var response = await _mediator.Send(request, cancellationToken);

    return Ok(response);
  }

  [HttpPost("register")]
  public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
  {
    var response = await _mediator.Send(request, cancellationToken);

    return Ok(response);
  }
}