using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
public class YahooScreenerMockTests
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
    public async Task RunScreener_DayGainers_ReturnsScreenerResults()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-day_gainers.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("day_gainers", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);
        
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Title, Is.EqualTo("Day Gainers"));
        Assert.That(screenerResult.CanonicalName, Is.EqualTo("DAY_GAINERS"));
    }

    [Test]
    public async Task RunScreener_DayGainers_ReturnsQuotes()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-day_gainers.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("day_gainers", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
        Assert.That(screenerResult.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task RunScreener_DayGainers_QuotesHaveRequiredFields()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-day_gainers.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("day_gainers", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        var firstQuote = screenerResult.Quotes.First();
        Assert.That(firstQuote.Symbol, Is.Not.Empty);
        Assert.That(firstQuote.RegularMarketPrice, Is.Not.Null.And.GreaterThan(0));
    }

    [Test]
    public async Task RunScreener_DayLosers_ReturnsScreenerResults()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-day_losers.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("day_losers", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.CanonicalName, Is.EqualTo("DAY_LOSERS"));
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_MostActives_ReturnsActiveStocks()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-most_actives.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("most_actives", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.CanonicalName, Is.EqualTo("MOST_ACTIVES"));
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_MostShorted_ReturnsShortedStocks()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-most_shorted_stocks.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("most_shorted_stocks", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_GrowthTech_ReturnsTechStocks()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-growth_technology_stocks.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("growth_technology_stocks", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_UndervaluedLargeCaps_ReturnsValueStocks()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-undervalued_large_caps.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("undervalued_large_caps", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_TopMutualFunds_ReturnsFunds()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-top_mutual_funds.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("top_mutual_funds", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_SmallCapGainers_ReturnsSmallCaps()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-small_cap_gainers.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("small_cap_gainers", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Quotes, Is.Not.Empty);
    }

    [Test]
    public async Task RunScreener_DayGainers_HasCriteriaMeta()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("screener-day_gainers.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.RunScreenerAsync("day_gainers", CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.CriteriaMeta, Is.Not.Null);
        Assert.That(screenerResult.CriteriaMeta.QuoteType, Is.EqualTo("EQUITY"));
        Assert.That(screenerResult.CriteriaMeta.SortField, Is.Not.Empty);
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
