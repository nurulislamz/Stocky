using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
[Category("Integration")]
public class YahooRecommendationsMockTests
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
    public async Task GetRecommendations_WithAAPLMock_ReturnsRecommendations()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetRecommendations_WithAAPLMock_ReturnsCorrectSymbol()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var response = result.Value.Finance.Result.First();
        Assert.That(response.Symbol, Is.EqualTo("AAPL"));
    }

    [Test]
    public async Task GetRecommendations_WithAAPLMock_HasRecommendedSymbols()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var response = result.Value.Finance.Result.First();
        Assert.That(response.RecommendedSymbols, Is.Not.Empty);
    }

    [Test]
    public async Task GetRecommendations_WithAAPLMock_RecommendationsHaveScores()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var response = result.Value.Finance.Result.First();
        foreach (var rec in response.RecommendedSymbols)
        {
            Assert.That(rec.Symbol, Is.Not.Empty);
            Assert.That(rec.Score, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task GetRecommendations_WithGOOGMock_ReturnsRecommendations()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-GOOG.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("GOOG", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
        Assert.That(result.Value.Finance.Result.First().Symbol, Is.EqualTo("GOOG"));
    }

    [Test]
    public async Task GetRecommendations_WithAMZNMock_ReturnsRecommendations()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-AMZN.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("AMZN", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetRecommendations_WithBABAMock_ReturnsRecommendations()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-BABA.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("BABA", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetRecommendations_WithSPOTMock_ReturnsRecommendations()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-SPOT.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("SPOT", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetRecommendations_WithInternationalStock_ReturnsRecommendations()
    {
        // Arrange - Danish stock
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-ORSTED.CO.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("ORSTED.CO", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
    }

    [Test]
    public async Task GetRecommendations_WithETF_ReturnsRecommendations()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("recommendationsBySymbol-QQQ.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetRecommendationsBySymbolAsync("QQQ", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
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
