using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Application.Auth;
using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Controllers;

/// <summary>
/// Controller for handling user authentication and registration.
/// </summary>
[AllowAnonymous] 
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
  private readonly ILogger<AuthController> _logger;
  private readonly IAuthenticationApi _authenticationApi;

  public AuthController(
    ILogger<AuthController> logger,
    IAuthenticationApi authenticationApi)
  {
    _logger = logger;
    _authenticationApi = authenticationApi;
  }

  /// <summary>
  /// Authenticates a user and returns a JWT token.
  /// </summary>
  /// <param name="request">The login request containing username and password.</param>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>A response containing the JWT token and user details.</returns>
  [HttpPost("login")] 
  public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var response = await _authenticationApi.Login(request, cancellationToken);
    return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
  }

  /// <summary>
  /// Registers a new user.
  /// </summary>
  /// <param name="request">The registration request containing user details.</param>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>A response containing the created user details.</returns>
  [HttpPost("register")]
  public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
  {
    var response = await _authenticationApi.Register(request, cancellationToken);
    return response.IsSuccess ? ProcessSuccess(HttpStatusCode.Created, response.Value) : ProcessFailure(response.Failure);
  }
}