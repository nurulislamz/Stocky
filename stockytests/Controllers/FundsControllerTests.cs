using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using stockyapi.Application.Funds;
using stockyapi.Application.Funds.AddFunds;
using stockyapi.Application.Funds.Response;
using stockyapi.Application.Funds.SubtractFunds;
using stockyapi.Controllers;
using stockyapi.Middleware;
using stockytests.Helpers;

namespace stockytests.Controllers;

[TestFixture]
[Category("Unit")]
public class FundsControllerTests
{
    private Mock<IFundsApi> _fundsApi = null!;
    private FundsController _controller = null!;
    private static readonly CancellationToken Token = CancellationToken.None;

    [SetUp]
    public void SetUp()
    {
        _fundsApi = new Mock<IFundsApi>();
        _controller = ControllerTestHelpers.SetupController(
            new FundsController(Mock.Of<ILogger<PortfolioController>>(), _fundsApi.Object));
    }

    [Test]
    public async Task GetFunds_WhenSuccess_ReturnsOk()
    {
        var response = new FundsResponse(100m, 200m, 50m);
        _fundsApi.Setup(api => api.GetFunds(Token))
            .ReturnsAsync(Result<FundsResponse>.Success(response));

        var result = await _controller.GetFunds(Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _fundsApi.Verify(api => api.GetFunds(Token), Times.Once);
    }

    [Test]
    public async Task GetFunds_WhenFailure_ReturnsProblemDetails()
    {
        var failure = new UnauthorizedFailure401();
        _fundsApi.Setup(api => api.GetFunds(Token))
            .ReturnsAsync(Result<FundsResponse>.Fail(failure));

        var result = await _controller.GetFunds(Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task AddFunds_WhenSuccess_ReturnsOk()
    {
        var request = new DepositFundsRequest { Amount = 10m };
        var response = new FundsResponse(110m, 210m, 60m);
        _fundsApi.Setup(api => api.DepositFunds(request, Token))
            .ReturnsAsync(Result<FundsResponse>.Success(response));

        var result = await _controller.DepositFunds(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _fundsApi.Verify(api => api.DepositFunds(request, Token), Times.Once);
    }

    [Test]
    public async Task AddFunds_WhenFailure_ReturnsProblemDetails()
    {
        var request = new DepositFundsRequest { Amount = 10m };
        var failure = new ValidationFailure422();
        _fundsApi.Setup(api => api.DepositFunds(request, Token))
            .ReturnsAsync(Result<FundsResponse>.Fail(failure));

        var result = await _controller.DepositFunds(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task SubtractFunds_WhenSuccess_ReturnsOk()
    {
        var request = new WithdrawFundsRequest { Amount = 10m };
        var response = new FundsResponse(90m, 190m, 40m);
        _fundsApi.Setup(api => api.WithdrawFunds(request, Token))
            .ReturnsAsync(Result<FundsResponse>.Success(response));

        var result = await _controller.WithdrawFunds(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _fundsApi.Verify(api => api.WithdrawFunds(request, Token), Times.Once);
    }

    [Test]
    public async Task SubtractFunds_WhenFailure_ReturnsProblemDetails()
    {
        var request = new WithdrawFundsRequest { Amount = 10m };
        var failure = new ConflictFailure409();
        _fundsApi.Setup(api => api.WithdrawFunds(request, Token))
            .ReturnsAsync(Result<FundsResponse>.Fail(failure));

        var result = await _controller.WithdrawFunds(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }
}
