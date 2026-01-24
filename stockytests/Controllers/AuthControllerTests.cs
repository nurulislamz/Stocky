using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using stockyapi.Application.Auth;
using stockyapi.Application.Auth.Login;
using stockyapi.Application.Auth.Register;
using stockyapi.Controllers;
using stockyapi.Middleware;
using stockyunittests.Helpers;

namespace stockyunittests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthenticationApi> _authenticationApi = null!;
    private AuthController _controller = null!;
    private static readonly CancellationToken Token = CancellationToken.None;

    [SetUp]
    public void SetUp()
    {
        _authenticationApi = new Mock<IAuthenticationApi>();
        _controller = ControllerTestHelpers.SetupController(
            new AuthController(Mock.Of<ILogger<AuthController>>(), _authenticationApi.Object));
    }

    [Test]
    public async Task Login_WhenSuccess_ReturnsOk()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "password123" };
        var response = new LoginResponse("token");
        _authenticationApi.Setup(api => api.Login(request, Token))
            .ReturnsAsync(Result<LoginResponse>.Success(response));

        var result = await _controller.Login(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _authenticationApi.Verify(api => api.Login(request, Token), Times.Once);
    }

    [Test]
    public async Task Login_WhenFailure_ReturnsProblemDetails()
    {
        var request = new LoginRequest { Email = "user@example.com", Password = "password123" };
        var failure = new UnauthorizedFailure401("Invalid password");
        _authenticationApi.Setup(api => api.Login(request, Token))
            .ReturnsAsync(Result<LoginResponse>.Fail(failure));

        var result = await _controller.Login(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task Register_WhenSuccess_ReturnsCreated()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "password123",
            FirstName = "First",
            Surname = "Last"
        };
        var response = new RegisterResponse("token", "user@example.com", "1");
        _authenticationApi.Setup(api => api.Register(request, Token))
            .ReturnsAsync(Result<RegisterResponse>.Success(response));

        var result = await _controller.Register(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _authenticationApi.Verify(api => api.Register(request, Token), Times.Once);
    }

    [Test]
    public async Task Register_WhenFailure_ReturnsProblemDetails()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "password123",
            FirstName = "First",
            Surname = "Last"
        };
        var failure = new ConflictFailure409("Email address already exists.");
        _authenticationApi.Setup(api => api.Register(request, Token))
            .ReturnsAsync(Result<RegisterResponse>.Fail(failure));

        var result = await _controller.Register(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }
}
