using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
public class YahooFundamentalsMockTests
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
    public async Task GetFundamentals_WithAAPLFinancialsQuarterlyMock_ReturnsData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("fundamentalsTimeSeries-AAPL-financials-quarterly.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetFundamentalsTimeSeriesAsync(
            "AAPL",
            CancellationToken.None,
            "quarterlyTotalRevenue", "quarterlyNetIncome");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Timeseries, Is.Not.Null);
        Assert.That(result.Value.Timeseries.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetFundamentals_WithAAPLBalanceSheetQuarterlyMock_ReturnsData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("fundamentalsTimeSeries-AAPL-balance-sheet-quarterly.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetFundamentalsTimeSeriesAsync(
            "AAPL",
            CancellationToken.None,
            "quarterlyTotalAssets", "quarterlyTotalLiabilitiesNetMinorityInterest");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Timeseries, Is.Not.Null);
        Assert.That(result.Value.Timeseries.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetFundamentals_WithAAPLCashFlowQuarterlyMock_ReturnsData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("fundamentalsTimeSeries-AAPL-cash-flow-quarterly.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetFundamentalsTimeSeriesAsync(
            "AAPL",
            CancellationToken.None,
            "quarterlyOperatingCashFlow", "quarterlyFreeCashFlow");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Timeseries, Is.Not.Null);
        Assert.That(result.Value.Timeseries.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetFundamentals_WithGOOGFinancialsQuarterlyMock_ReturnsData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("fundamentalsTimeSeries-GOOG-financials-quarterly.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetFundamentalsTimeSeriesAsync(
            "GOOG",
            CancellationToken.None,
            "quarterlyTotalRevenue");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Timeseries, Is.Not.Null);
        Assert.That(result.Value.Timeseries.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetFundamentals_WithAMZNFinancialsQuarterlyMock_ReturnsData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("fundamentalsTimeSeries-AMZN-financials-quarterly.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetFundamentalsTimeSeriesAsync(
            "AMZN",
            CancellationToken.None,
            "quarterlyTotalRevenue");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Timeseries, Is.Not.Null);
        Assert.That(result.Value.Timeseries.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetFundamentals_WithAAPLFinancialsQuarterlyMock_HasMetadata()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("fundamentalsTimeSeries-AAPL-financials-quarterly.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetFundamentalsTimeSeriesAsync(
            "AAPL",
            CancellationToken.None,
            "quarterlyTotalRevenue");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var firstResult = result.Value.Timeseries.Result.First();
        Assert.That(firstResult.Meta, Is.Not.Null);
        Assert.That(firstResult.Meta.Symbol, Is.Not.Empty);
        Assert.That(firstResult.Meta.Type, Is.Not.Empty);
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
