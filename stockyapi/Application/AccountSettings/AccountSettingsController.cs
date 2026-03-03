using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Application.Auth;
using stockyapi.Application.Auth.Login;
using stockyapi.Controllers.Helpers;

namespace stockyapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountSettingsController : BaseController
{

  private readonly IAccountSettingsApi _accountSettingsApi;
  private readonly ILogger<AccountSettingsController> _logger;

  public AccountSettingsController(
    IAccountSettingsApi accountSettingsApi,
    ILogger<AccountSettingsController> logger)
  {
    _accountSettingsApi = accountSettingsApi;
    _logger = logger;
  }

  /// <summary> /// Registers a new user.
  /// </summary>
  /// <param name="request">The registration request containing user details.</param>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>A response containing the created user details.</returns>
  [Authorize]
  [HttpPost("changeName")]
  public async Task<ActionResult<string>> ChangeName([FromBody] ChangeNameRequest request, CancellationToken cancellationToken)
  {
    var response = await _accountSettingsApi.ChangeName(request, cancellationToken);
    return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
  }

  [HttpPost("set-ai-api-key")]
  public async Task<ActionResult> SetAiApiKey([FromBody] string request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}