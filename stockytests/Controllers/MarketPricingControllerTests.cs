using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using stockyapi.Application.MarketPricing;
using stockyapi.Controllers;
using stockyapi.Middleware;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;
using stockyunittests.Helpers;

namespace stockytests.Controllers;

[TestFixture]
public class MarketPricingControllerTests
{
    private Mock<IMarketPricingApi> _marketPricingApi;
    private MarketPricingController _controller;
    private static readonly CancellationToken Token = CancellationToken.None;
    
    [SetUp]
    public void Setup()
    {
        _marketPricingApi = new Mock<IMarketPricingApi>();
        _controller = ControllerTestHelpers.SetupController(
            new MarketPricingController(_marketPricingApi.Object));
    }

    /// <summary>
    /// BLUEPRINT
    /// </summary>
    [Test]
    public async Task GetCurrentPrice_WithValidTicker_ReturnsOkResult()
    {
        // Arrange
        const string ticker = "AAPL";
        var expectedResponse = new YahooChartResponse();
        var successResult = Result<YahooChartResponse>.Success(expectedResponse);

        _marketPricingApi.Setup(x => x.GetCurrentPrice(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetCurrentPrice(ticker, Token);

        // Assert
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.InstanceOf<YahooChartResponse>());
        Assert.That(objectResult.Value, Is.EqualTo(expectedResponse));
        _marketPricingApi.Verify(api => api.GetCurrentPrice(ticker, Token), Times.Once);
    }
     
    /// <summary>
    /// BLUEPRINT
    /// </summary>
    [Test]
    public async Task GetCurrentPrice_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var ticker = "INVALID";
        var failure =  new InternalServerFailure500("Yahoo Finance API error: 500");

        _marketPricingApi.Setup(x => x.GetCurrentPrice(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.GetCurrentPrice(ticker, CancellationToken.None);

        // Assert
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        Assert.That(objectResult.Value, Is.InstanceOf<ProblemDetails>());

        var problemDetails = objectResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
        _marketPricingApi.Verify(api => api.GetCurrentPrice(ticker, Token), Times.Once);
    }

    [Test]
    public async Task GetChart_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var symbol = "MSFT";
        var range = YahooRange.OneDay;
        var interval = YahooInterval.OneMinute;
        var expectedChartData = new YahooChartResponse(); 

        _marketPricingApi.Setup(x => x.GetChart(symbol, range, interval, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<YahooChartResponse>.Success(expectedChartData));

        // Act
        var result = await _controller.GetChart(symbol, range, interval, null, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedChartData));
        _marketPricingApi.Verify(api => api.GetChart(symbol, range, interval, null, Token), Times.Once);
    }

    [Test]
    public async Task GetChart_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var symbol = "MSFT";
        var failure = new InternalServerFailure500("API Error");

        _marketPricingApi.Setup(x => x.GetChart(symbol, It.IsAny<YahooRange>(), It.IsAny<YahooInterval>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.GetChart(symbol, YahooRange.OneDay, YahooInterval.OneMinute, null, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
        _marketPricingApi.Verify(api => api.GetChart(symbol, It.IsAny<YahooRange>(), It.IsAny<YahooInterval>(), null, Token), Times.Once);
    }

    [Test]
    public async Task GetHistorical_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var symbol = "GOOGL";
        var period1 = DateTime.UtcNow.AddDays(-7);
        var period2 = DateTime.UtcNow;
        var interval = YahooInterval.OneDay;
        var expectedData = new HistoricalHistoryResult();

        _marketPricingApi.Setup(x => x.GetHistorical(symbol, period1, period2, interval, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<HistoricalHistoryResult>.Success(expectedData));

        // Act
        var result = await _controller.GetHistorical(symbol, period1, period2, interval, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedData));
        _marketPricingApi.Verify(api => api.GetHistorical(symbol, period1, period2, interval, Token), Times.Once);
    }

    [Test]
    public async Task GetHistorical_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var symbol = "GOOGL";
        var failure = new InternalServerFailure500("API Error");

        _marketPricingApi.Setup(x => x.GetHistorical(symbol, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<YahooInterval>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.GetHistorical(symbol, DateTime.UtcNow, DateTime.UtcNow, YahooInterval.OneDay, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
    }

    [Test]
    public async Task GetQuotes_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var symbols = new[] { "AAPL", "MSFT" };
        var expectedData = new QuoteResponseArray();

        _marketPricingApi.Setup(x => x.GetQuote(symbols, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<QuoteResponseArray>.Success(expectedData));

        // Act
        var result = await _controller.GetQuotes(symbols, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedData));
        _marketPricingApi.Verify(api => api.GetQuote(symbols, Token), Times.Once);
    }

    [Test]
    public async Task GetQuotes_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var symbols = new[] { "AAPL" };
        var failure = new InternalServerFailure500("API Error");

        _marketPricingApi.Setup(x => x.GetQuote(symbols, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.GetQuotes(symbols, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
    }

    [Test]
    public async Task GetQuoteSummary_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var symbol = "TSLA";
        var modules = new[] { "price" };
        var expectedData = new QuoteSummaryResult();

        _marketPricingApi.Setup(x => x.GetQuoteSummary(symbol, modules, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<QuoteSummaryResult>.Success(expectedData));

        // Act
        var result = await _controller.GetQuoteSummary(symbol, modules, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedData));
        _marketPricingApi.Verify(api => api.GetQuoteSummary(symbol, modules, Token), Times.Once);
    }

    [Test]
    public async Task GetQuoteSummary_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var symbol = "TSLA";
        var failure = new InternalServerFailure500("API Error");

        _marketPricingApi.Setup(x => x.GetQuoteSummary(symbol, It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.GetQuoteSummary(symbol, null, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
    }

    [Test]
    public async Task RunScreener_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var screenerId = "day_gainers";
        var expectedData = new ScreenerResult();

        _marketPricingApi.Setup(x => x.RunScreener(screenerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ScreenerResult>.Success(expectedData));

        // Act
        var result = await _controller.RunScreener(screenerId, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedData));
        _marketPricingApi.Verify(api => api.RunScreener(screenerId, Token), Times.Once);
    }

    [Test]
    public async Task RunScreener_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var screenerId = "day_gainers";
        var failure = new InternalServerFailure500("API Error");

        _marketPricingApi.Setup(x => x.RunScreener(screenerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.RunScreener(screenerId, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
    }

    [Test]
    public async Task Search_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var query = "Apple";
        var expectedData = new SearchResult();

        _marketPricingApi.Setup(x => x.Search(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SearchResult>.Success(expectedData));

        // Act
        var result = await _controller.Search(query, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedData));
        _marketPricingApi.Verify(api => api.Search(query, Token), Times.Once);
    }

    [Test]
    public async Task Search_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var query = "Unknown";
        var failure =  new InternalServerFailure500("Yahoo Finance API error: 500");

        _marketPricingApi.Setup(x => x.Search(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.Search(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        
        Assert.That(actionResult.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
    }

    [Test]
    public async Task GetTrendingSymbols_WithValidParams_ReturnsOkResult()
    {
        // Arrange
        var region = "US";
        var expectedData = new TrendingSymbolsResult();

        _marketPricingApi.Setup(x => x.GetTrendingSymbols(region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TrendingSymbolsResult>.Success(expectedData));

        // Act
        var result = await _controller.GetTrendingSymbols(region, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        Assert.That(actionResult.Value, Is.EqualTo(expectedData));
        _marketPricingApi.Verify(api => api.GetTrendingSymbols(region, Token), Times.Once);
    }

    [Test]
    public async Task GetTrendingSymbols_WhenApiFails_ReturnsErrorResult()
    {
        // Arrange
        var region = "US";
        var failure = new InternalServerFailure500("API Error");

        _marketPricingApi.Setup(x => x.GetTrendingSymbols(region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);

        // Act
        var result = await _controller.GetTrendingSymbols(region, Token);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var actionResult = (ObjectResult)result;
        Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        
        var problemDetails = actionResult.Value as ProblemDetails;
        Assert.That(problemDetails!.Title, Does.Contain(failure.Title));
        Assert.That(problemDetails!.Detail, Does.Contain(failure.Detail));
    }
}