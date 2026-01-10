using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountSettingsController : ControllerBase
{
  private readonly ILogger<AccountSettingsController> _logger;

  public AccountSettingsController(
    ILogger<AccountSettingsController> logger)
  {
    _logger = logger;
  }

  [HttpPost("change-password")]
  public async Task<ActionResult> ChangePassword([FromBody] string request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  [HttpPost("set-openai-api-key")]
  public async Task<ActionResult> SetOpenAiApiKey([FromBody] string request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}