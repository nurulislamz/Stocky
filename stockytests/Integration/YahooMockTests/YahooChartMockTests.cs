using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
public class YahooChartMockTests
{
    private IMemoryCache _memoryCache = null!;

    [SetUp]
    public void SetUp()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [TearDown]
    public void TearDown()
    {
        _memoryCache.Dispose();
    }

    [Test]
    public async Task GetChart_WithAAPLMockData_ReturnsCorrectSymbolAndMeta()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "chart-AAPL-2025-02-01-to-2025-02-02-includePrePost-false-interval-1m.static.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "AAPL",
            YahooRange.OneDay,
            YahooInterval.OneMinute,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Chart, Is.Not.Null);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);

        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("AAPL"));
        Assert.That(chartResult.Meta.Currency, Is.EqualTo("USD"));
        Assert.That(chartResult.Meta.RegularMarketPrice, Is.GreaterThan(0));
        Assert.That(chartResult.Meta.ExchangeName, Is.EqualTo("NMS"));
        Assert.That(chartResult.Meta.InstrumentType, Is.EqualTo("EQUITY"));
        Assert.That(chartResult.Meta.LongName, Is.EqualTo("Apple Inc."));
    }

    [Test]
    public async Task GetChart_WithAAPLMockData_ReturnsValidMarketData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "chart-AAPL-2025-02-01-to-2025-02-02-includePrePost-false-interval-1m.static.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "AAPL",
            YahooRange.OneDay,
            YahooInterval.OneMinute,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var meta = result.Value.Chart.Result!.First().Meta;

        Assert.That(meta.FiftyTwoWeekHigh, Is.GreaterThan(0));
        Assert.That(meta.FiftyTwoWeekLow, Is.GreaterThan(0));
        Assert.That(meta.RegularMarketDayHigh, Is.GreaterThan(0));
        Assert.That(meta.RegularMarketDayLow, Is.GreaterThan(0));
        Assert.That(meta.RegularMarketVolume, Is.GreaterThan(0));
        Assert.That(meta.DataGranularity, Is.EqualTo("1m"));
    }

    [Test]
    public async Task GetChart_WithAAPLMockData_ReturnsTradingPeriods()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "chart-AAPL-2025-02-01-to-2025-02-02-includePrePost-false-interval-1m.static.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "AAPL",
            YahooRange.OneDay,
            YahooInterval.OneMinute,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var meta = result.Value.Chart.Result!.First().Meta;

        Assert.That(meta.CurrentTradingPeriod, Is.Not.Null);
        Assert.That(meta.CurrentTradingPeriod.Pre, Is.Not.Null);
        Assert.That(meta.CurrentTradingPeriod.Regular, Is.Not.Null);
        Assert.That(meta.CurrentTradingPeriod.Post, Is.Not.Null);
        Assert.That(meta.Timezone, Is.EqualTo("EST"));
    }

    [Test]
    public async Task GetChart_WithTSLAFakeMockData_ReturnsChartData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "chart-TSLA-2025-01-01-to-2025-01-02-interval-1h.fake.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "TSLA",
            YahooRange.OneDay,
            YahooInterval.OneHour,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart, Is.Not.Null);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);

        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("TSLA"));
    }

    [Test]
    public async Task GetChart_WithValidMockData_ReturnsIndicators()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "chart-AAPL-2025-02-01-to-2025-02-02-includePrePost-false-interval-1m.static.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "AAPL",
            YahooRange.OneDay,
            YahooInterval.OneMinute,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var chartResult = result.Value.Chart.Result!.First();
        
        Assert.That(chartResult.Indicators, Is.Not.Null);
        Assert.That(chartResult.Indicators.Quote, Is.Not.Null);
    }

    [Test]
    public async Task GetChart_MockResponseHasNoError()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "chart-AAPL-2025-02-01-to-2025-02-02-includePrePost-false-interval-1m.static.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "AAPL",
            YahooRange.OneDay,
            YahooInterval.OneMinute,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart.Error, Is.Null);
    }

    private (IYahooFinanceService Service, HttpClient HttpClient) CreateService(MockCallFile mockCall)
    {
        var handler = new MockHttpMessageHandler(mockCall);
        var httpClient = new HttpClient(handler);
        var logger = NullLogger<YahooApiServiceClient>.Instance;
        var serviceClient = new YahooApiServiceClient(httpClient, _memoryCache, logger, 1, 3, 2, 10);
        var service = new YahooFinanceService(serviceClient);
        
        return (service, httpClient);
    }
}
