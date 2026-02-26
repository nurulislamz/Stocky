using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
[Category("Integration")]
public class YahooHistoricalMockTests
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
    public async Task GetHistorical_WithMSFTDividendsMockData_ReturnsChartWithDividends()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-MSFT-dividends-2021-02-01-to-2022-01-31.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "MSFT",
            YahooRange.OneYear,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart, Is.Not.Null);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);

        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("MSFT"));
        Assert.That(chartResult.Meta.Currency, Is.EqualTo("USD"));
    }

    [Test]
    public async Task GetHistorical_WithMSFTMockData_HasTimestampData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-MSFT-dividends-2021-02-01-to-2022-01-31.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "MSFT",
            YahooRange.OneYear,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var chartResult = result.Value.Chart.Result!.First();

        Assert.That(chartResult.Timestamp, Is.Not.Null.And.Not.Empty);
        Assert.That(chartResult.Timestamp!.Count, Is.GreaterThan(100));
    }

    [Test]
    public async Task GetHistorical_WithMSFTMockData_HasDividendEvents()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-MSFT-dividends-2021-02-01-to-2022-01-31.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "MSFT",
            YahooRange.OneYear,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var chartResult = result.Value.Chart.Result!.First();

        Assert.That(chartResult.Events, Is.Not.Null);
        Assert.That(chartResult.Events!.Dividends, Is.Not.Null.And.Not.Empty);

        var firstDividend = chartResult.Events.Dividends!.Values.First();
        Assert.That(firstDividend.Amount, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetHistorical_WithMSFTMockData_HasPriceIndicators()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-MSFT-dividends-2021-02-01-to-2022-01-31.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "MSFT",
            YahooRange.OneYear,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var chartResult = result.Value.Chart.Result!.First();

        Assert.That(chartResult.Indicators, Is.Not.Null);
        Assert.That(chartResult.Indicators.Quote, Is.Not.Null.And.Not.Empty);

        var quote = chartResult.Indicators.Quote.First();
        Assert.That(quote.Open, Is.Not.Empty);
        Assert.That(quote.High, Is.Not.Empty);
        Assert.That(quote.Low, Is.Not.Empty);
        Assert.That(quote.Close, Is.Not.Empty);
        Assert.That(quote.Volume, Is.Not.Empty);
    }

    [Test]
    public async Task GetHistorical_WithNVDASplitMockData_HasSplitEvents()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-NVDA-split-2021-02-01-to-2022-01-31.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "NVDA",
            YahooRange.OneYear,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var chartResult = result.Value.Chart.Result!.First();

        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("NVDA"));
        Assert.That(chartResult.Events, Is.Not.Null);
        Assert.That(chartResult.Events!.Splits, Is.Not.Null.And.Not.Empty);

        var split = chartResult.Events.Splits!.Values.First();
        Assert.That(split.Numerator, Is.GreaterThan(0));
        Assert.That(split.Denominator, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetHistorical_WithInternationalSymbol_ReturnsValidData()
    {
        // Arrange - Test with Danish stock ORSTED.CO
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-ORSTED.CO-2020-01-01-to-2020-01-03.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "ORSTED.CO",
            YahooRange.FiveDays,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);
        
        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("ORSTED.CO"));
    }

    [Test]
    public async Task GetHistorical_WithCryptoSymbol_ReturnsValidData()
    {
        // Arrange - Test with Bitcoin
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-BTC-USD-2020-01-01-to-2020-01-03.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "BTC-USD",
            YahooRange.FiveDays,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);
        
        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("BTC-USD"));
    }

    [Test]
    public async Task GetHistorical_WithForexSymbol_ReturnsValidData()
    {
        // Arrange - Test with EUR/USD
        var mockCall = MockCallLoader.LoadMockCall(
            "historical-via-chart-EURUSD=X-2020-01-01-to-2020-01-03.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetChartAsync(
            "EURUSD=X",
            YahooRange.FiveDays,
            YahooInterval.OneDay,
            [],
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);
        
        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo("EURUSD=X"));
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
