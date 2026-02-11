using Moq;
using stockyapi.Application.MarketPricing;
using stockyapi.Middleware;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;
using stockytests.Helpers;

namespace stockytests.Integration;

[TestFixture]
public class MarketPricingIntegrationTests
{
    private Mock<IYahooFinanceService> _yahooFinanceServiceMock = null!;
    private IMarketPricingApi _marketPricingApi = null!;

    [SetUp]
    public void Setup()
    {
        _yahooFinanceServiceMock = new Mock<IYahooFinanceService>();
        _marketPricingApi = new MarketPricingApi(_yahooFinanceServiceMock.Object);
    }

    [Test]
    public async Task GetCurrentPrice_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResult = Result<ChartResultArray>.Success(new ChartResultArray());
        _yahooFinanceServiceMock.Setup(s => s.GetChartAsync(ticker, YahooRange.OneDay, YahooInterval.FifteenMinutes, It.IsAny<YahooFields[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _marketPricingApi.GetCurrentPrice(ticker, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        _yahooFinanceServiceMock.Verify(s => s.GetChartAsync(ticker, YahooRange.OneDay, YahooInterval.FifteenMinutes, It.IsAny<YahooFields[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetChart_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var symbol = "MSFT";
        var range = YahooRange.FiveDays;
        var interval = YahooInterval.OneHour;
        var additionalFields = new[] { YahooFields.marketCap };
        var expectedResult = Result<ChartResultArray>.Success(new ChartResultArray());
        _yahooFinanceServiceMock.Setup(s => s.GetChartAsync(symbol, range, interval, It.Is<YahooFields[]>(f => f.Contains(YahooFields.regularMarketPrice) && f.Contains(YahooFields.marketCap)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _marketPricingApi.GetChart(symbol, range, interval, additionalFields, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        _yahooFinanceServiceMock.Verify(s => s.GetChartAsync(symbol, range, interval, It.Is<YahooFields[]>(f => f.Contains(YahooFields.regularMarketPrice) && f.Contains(YahooFields.marketCap)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetHistorical_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var symbol = "GOOG";
        var period1 = new DateTime(2023, 1, 1);
        var period2 = new DateTime(2023, 1, 31);
        var interval = YahooInterval.OneDay;
        var expectedResult = Result<HistoricalHistoryResult>.Success(new HistoricalHistoryResult());
        _yahooFinanceServiceMock.Setup(s => s.GetHistoricalAsync(symbol, period1, period2, interval, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _marketPricingApi.GetHistorical(symbol, period1, period2, interval, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        _yahooFinanceServiceMock.Verify(s => s.GetHistoricalAsync(symbol, period1, period2, interval, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task RunScreener_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var screenerId = "most_actives";
        var expectedResult = Result<ScreenerResult>.Success(new ScreenerResult());
        _yahooFinanceServiceMock.Setup(s => s.RunScreenerAsync(screenerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _marketPricingApi.RunScreener(screenerId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        _yahooFinanceServiceMock.Verify(s => s.RunScreenerAsync(screenerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Search_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var query = "Tesla";
        var expectedResult = Result<SearchResult>.Success(new SearchResult());
        _yahooFinanceServiceMock.Setup(s => s.SearchAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _marketPricingApi.Search(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        _yahooFinanceServiceMock.Verify(s => s.SearchAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetTrendingSymbols_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var region = "US";
        var expectedResult = Result<TrendingSymbolsResult>.Success(new TrendingSymbolsResult());
        _yahooFinanceServiceMock.Setup(s => s.GetTrendingSymbolsAsync(region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _marketPricingApi.GetTrendingSymbols(region, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        _yahooFinanceServiceMock.Verify(s => s.GetTrendingSymbolsAsync(region, It.IsAny<CancellationToken>()), Times.Once);
    }
}