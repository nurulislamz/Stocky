using stockyapi.Requests;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using stockyapi.Responses;

namespace stockyapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountSettingsController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly ILogger<AccountSettingsController> _logger;

  public AccountSettingsController(
    ILogger<AccountSettingsController> logger,
    IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [HttpPost("change-password")]
  public async Task<ActionResult<ChangePasswordResponse>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
  {
    var response = await _mediator.Send(request, cancellationToken);

    return Ok(response);
  }

  [HttpPost("set-openai-api-key")]
  public async Task<ActionResult<SetOpenAiApiKeyResponse>> SetOpenAiApiKey([FromBody] SetOpenAiApiKeyRequest request, CancellationToken cancellationToken)
  {
    var response = await _mediator.Send(request, cancellationToken);

    return Ok(response);
  }
}