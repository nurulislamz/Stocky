using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
public class YahooInsightsMockTests
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
    public async Task GetInsights_WithAAPLMock_ReturnsInsightsForSymbol()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("insights-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetInsightsAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Null);
        Assert.That(result.Value.Finance.Result!.Symbol, Is.EqualTo("AAPL"));
    }

    [Test]
    public async Task GetInsights_WithAAPLMock_HasInstrumentInfo()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("insights-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetInsightsAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var insights = result.Value.Finance.Result!;
        Assert.That(insights.InstrumentInfo, Is.Not.Null);
        Assert.That(insights.InstrumentInfo!.TechnicalEvents, Is.Not.Null);
    }

    [Test]
    public async Task GetInsights_WithAAPLMock_HasCompanySnapshot()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("insights-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetInsightsAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var insights = result.Value.Finance.Result!;
        Assert.That(insights.CompanySnapshot, Is.Not.Null);
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
