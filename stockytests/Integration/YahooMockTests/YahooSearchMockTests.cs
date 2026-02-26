using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
[Category("Integration")]
public class YahooSearchMockTests
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
    public async Task Search_WithAAPLMock_ReturnsMatchingQuotes()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
        Assert.That(result.Value.Count, Is.GreaterThan(0));
        
        var firstQuote = result.Value.Quotes.First();
        Assert.That(firstQuote.Symbol, Is.EqualTo("AAPL"));
        Assert.That(firstQuote.Exchange, Is.Not.Empty);
    }

    [Test]
    public async Task Search_WithAAPLMock_ReturnsCorrectQuoteType()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("AAPL", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var firstQuote = result.Value.Quotes.First();
        Assert.That(firstQuote.QuoteType, Is.EqualTo("EQUITY"));
        Assert.That(firstQuote.Shortname, Is.EqualTo("Apple Inc."));
    }

    [Test]
    public async Task Search_WithGOOGMock_ReturnsGoogleResults()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-GOOG.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("GOOG", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
        Assert.That(result.Value.Quotes.Any(q => q.Symbol == "GOOG"), Is.True);
    }

    [Test]
    public async Task Search_WithAMZNMock_ReturnsAmazonResults()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-AMZN.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("AMZN", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
        Assert.That(result.Value.Quotes.Any(q => q.Symbol == "AMZN"), Is.True);
    }

    [Test]
    public async Task Search_WithForexSymbol_ReturnsForexQuote()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-EURUSD=X.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("EURUSD=X", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task Search_WithCryptoSymbol_ReturnsCryptoQuote()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-BTC-USD.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("BTC-USD", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task Search_WithCommoditySymbol_ReturnsCommodityQuote()
    {
        // Arrange - Gold futures
        var mockCall = MockCallLoader.LoadMockCall("search-GC=F.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("GC=F", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task Search_WithInternationalStock_ReturnsResults()
    {
        // Arrange - Danish stock
        var mockCall = MockCallLoader.LoadMockCall("search-ORSTED.CO.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("ORSTED.CO", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task Search_WithETF_ReturnsETFQuote()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("search-QQQ.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("QQQ", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task Search_WithCompanyName_ReturnsMatchingResults()
    {
        // Arrange - Search by company name
        var mockCall = MockCallLoader.LoadMockCall("search-Bayerische Motoren Werke AG.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.SearchAsync("Bayerische Motoren Werke AG", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Quotes, Is.Not.Empty);
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
