using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

    [Test]
    public async Task GetCurrentPrice_WithValidTicker_ReturnsOkResult()
    {
        // Arrange
        const string ticker = "AAPL";
        var expectedResponse = new ChartResultArray();
        var successResult = Result<ChartResultArray>.Success(expectedResponse);

        _marketPricingApi.Setup(x => x.GetCurrentPrice(ticker, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.GetCurrentPrice(ticker, Token);

        // Assert
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.InstanceOf<ChartResultArray>());
        Assert.That(objectResult.Value, Is.EqualTo(expectedResponse));
        _marketPricingApi.Verify(api => api.GetCurrentPrice(ticker, Token), Times.Once);
    }

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
        
        // Using object as the generic type since the specific Chart type isn't visible in the context,
        // but assuming the interface allows generic mocking or the actual type is compatible.
        var expectedChartData = new ChartResultArray(); 

        _marketPricingApi.Setup(x => x.GetChart(symbol, range, interval, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedChartData);

        // Act
        var result = await _controller.GetChart(symbol, range, interval, null, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actionResult = (OkObjectResult)result;
        
        Assert.That(actionResult.Value, Is.EqualTo(expectedChartData));
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
    }
}