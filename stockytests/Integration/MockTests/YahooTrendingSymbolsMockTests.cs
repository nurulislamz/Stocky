using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
public class YahooTrendingSymbolsMockTests
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
    public async Task GetTrendingSymbols_WithUSMock_ReturnsSymbols()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("trendingSymbols-US.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetTrendingSymbolsAsync("US", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
        
        var trendingResult = result.Value.Finance.Result.First();
        Assert.That(trendingResult.Quotes, Is.Not.Empty);
        Assert.That(trendingResult.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetTrendingSymbols_WithUSMock_QuotesHaveSymbols()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("trendingSymbols-US.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetTrendingSymbolsAsync("US", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var trendingResult = result.Value.Finance.Result.First();
        foreach (var quote in trendingResult.Quotes)
        {
            Assert.That(quote.Symbol, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test]
    public async Task GetTrendingSymbols_WithGBMock_ReturnsUKSymbols()
    {
        // Arrange - Great Britain
        var mockCall = MockCallLoader.LoadMockCall("trendingSymbols-GB.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetTrendingSymbolsAsync("GB", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetTrendingSymbols_WithITMock_ReturnsItalianSymbols()
    {
        // Arrange - Italy
        var mockCall = MockCallLoader.LoadMockCall("trendingSymbols-IT.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetTrendingSymbolsAsync("IT", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetTrendingSymbols_WithAUMock_ReturnsAustralianSymbols()
    {
        // Arrange - Australia
        var mockCall = MockCallLoader.LoadMockCall("trendingSymbols-AU.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetTrendingSymbolsAsync("AU", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetTrendingSymbols_WithUSMock_HasTimestampData()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("trendingSymbols-US.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetTrendingSymbolsAsync("US", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var trendingResult = result.Value.Finance.Result.First();
        Assert.That(trendingResult.JobTimestamp, Is.GreaterThan(0));
        Assert.That(trendingResult.StartInterval, Is.GreaterThan(0));
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
