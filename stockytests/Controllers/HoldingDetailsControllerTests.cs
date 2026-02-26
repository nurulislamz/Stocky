using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using stockyapi.Controllers;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Types;
using stockyapi.Middleware;
using stockytests.Helpers;

namespace stockytests.Controllers;

[TestFixture]
[Category("Unit")]
public class HoldingDetailsControllerTests
{
    private Mock<IYahooFinanceService> _yahooFinanceService = null!;
    private HoldingDetailsController _controller = null!;
    private static readonly CancellationToken Token = CancellationToken.None;

    [SetUp]
    public void SetUp()
    {
        _yahooFinanceService = new Mock<IYahooFinanceService>();
        _controller = ControllerTestHelpers.SetupController(new HoldingDetailsController(
            Mock.Of<Microsoft.Extensions.Logging.ILogger<HoldingDetailsController>>(),
            _yahooFinanceService.Object));
    }

    [Test]
    public async Task GetFundamentalsTimeSeries_WhenSuccess_ReturnsOk()
    {
        var symbol = "AAPL";
        var expected = new FundamentalsTimeSeriesResponse();
        _yahooFinanceService.Setup(x => x.GetFundamentalsTimeSeriesAsync(symbol, Token, It.IsAny<string[]>()))
            .ReturnsAsync(Result<FundamentalsTimeSeriesResponse>.Success(expected));

        var result = await _controller.GetFundamentalsTimeSeries(symbol, null, Token);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expected));
        _yahooFinanceService.Verify(x => x.GetFundamentalsTimeSeriesAsync(symbol, Token, It.Is<string[]>(a => a.Length > 0)), Times.Once);
    }

    [Test]
    public async Task GetFundamentalsTimeSeries_WhenFailure_ReturnsProblemDetails()
    {
        var symbol = "INVALID";
        var failure = new InternalServerFailure500("API error");
        _yahooFinanceService.Setup(x => x.GetFundamentalsTimeSeriesAsync(symbol, Token, It.IsAny<string[]>()))
            .ReturnsAsync(Result<FundamentalsTimeSeriesResponse>.Fail(failure));

        var result = await _controller.GetFundamentalsTimeSeries(symbol, null, Token);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(objectResult.Value, Is.InstanceOf<ProblemDetails>());
    }

    [Test]
    public async Task GetInsights_WhenSuccess_ReturnsOk()
    {
        var symbol = "MSFT";
        var expected = new InsightsApiResponse();
        _yahooFinanceService.Setup(x => x.GetInsightsAsync(symbol, Token))
            .ReturnsAsync(Result<InsightsApiResponse>.Success(expected));

        var result = await _controller.GetInsights(symbol, Token);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expected));
        _yahooFinanceService.Verify(x => x.GetInsightsAsync(symbol, Token), Times.Once);
    }

    [Test]
    public async Task GetInsights_WhenFailure_ReturnsProblemDetails()
    {
        var symbol = "BAD";
        var failure = new NotFoundFailure404("Symbol not found");
        _yahooFinanceService.Setup(x => x.GetInsightsAsync(symbol, Token))
            .ReturnsAsync(Result<InsightsApiResponse>.Fail(failure));

        var result = await _controller.GetInsights(symbol, Token);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
    }

    [Test]
    public async Task GetOptions_WhenNoDate_WhenSuccess_ReturnsOk()
    {
        var symbol = "TSLA";
        var expected = new OptionsApiResponse();
        _yahooFinanceService.Setup(x => x.GetOptionsAsync(symbol, Token))
            .ReturnsAsync(Result<OptionsApiResponse>.Success(expected));

        var result = await _controller.GetOptions(symbol, null, Token);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expected));
        _yahooFinanceService.Verify(x => x.GetOptionsAsync(symbol, Token), Times.Once);
        _yahooFinanceService.Verify(x => x.GetOptionsAsync(symbol, It.IsAny<string>(), Token), Times.Never);
    }

    [Test]
    public async Task GetOptions_WithDate_WhenSuccess_ReturnsOk()
    {
        var symbol = "TSLA";
        var date = "2025-03-21";
        var expected = new OptionsApiResponse();
        _yahooFinanceService.Setup(x => x.GetOptionsAsync(symbol, date, Token))
            .ReturnsAsync(Result<OptionsApiResponse>.Success(expected));

        var result = await _controller.GetOptions(symbol, date, Token);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _yahooFinanceService.Verify(x => x.GetOptionsAsync(symbol, date, Token), Times.Once);
    }

    [Test]
    public async Task GetOptions_WhenFailure_ReturnsProblemDetails()
    {
        var symbol = "X";
        var failure = new InternalServerFailure500("Error");
        _yahooFinanceService.Setup(x => x.GetOptionsAsync(symbol, Token))
            .ReturnsAsync(Result<OptionsApiResponse>.Fail(failure));

        var result = await _controller.GetOptions(symbol, null, Token);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task GetRecommendations_WhenSuccess_ReturnsOk()
    {
        var symbol = "AAPL";
        var expected = new RecommendationsBySymbolApiResponse();
        _yahooFinanceService.Setup(x => x.GetRecommendationsBySymbolAsync(symbol, Token))
            .ReturnsAsync(Result<RecommendationsBySymbolApiResponse>.Success(expected));

        var result = await _controller.GetRecommendations(symbol, Token);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expected));
        _yahooFinanceService.Verify(x => x.GetRecommendationsBySymbolAsync(symbol, Token), Times.Once);
    }

    [Test]
    public async Task GetRecommendations_WhenFailure_ReturnsProblemDetails()
    {
        var symbol = "NONE";
        var failure = new InternalServerFailure500("Error");
        _yahooFinanceService.Setup(x => x.GetRecommendationsBySymbolAsync(symbol, Token))
            .ReturnsAsync(Result<RecommendationsBySymbolApiResponse>.Fail(failure));

        var result = await _controller.GetRecommendations(symbol, Token);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
}
