using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using stockyapi.Application.MarketPricing;
using stockyapi.Controllers.Helpers;
using stockyapi.Middleware;
using stockyapi.Repository.PortfolioRepository;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Helper;
using stockyapi.Services.YahooFinance.Types;
using stockytests.Helpers;
using DateTime = System.DateTime;

namespace stockytests.Integration;

[TestFixture]
public class MarketPricingIntegrationTests
{
    private IMarketPricingApi _marketPricingApi = null!;
    private YahooApiServiceClient _yahooApiServiceClient = null!;
    private IYahooFinanceService _yahooFinanceService = null!;

    [SetUp]
    public void Setup()
    {
        var logger = NullLogger<YahooApiServiceClient>.Instance;
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgents.GetRandomNewUserAgent());

        _yahooApiServiceClient = new YahooApiServiceClient(httpClient, memoryCache, logger, 1, 3, 2, 10);
        _yahooFinanceService = new YahooFinanceService(_yahooApiServiceClient);
        _marketPricingApi = new MarketPricingApi(_yahooFinanceService);
    }

    [Test]
    public async Task GetCurrentPrice_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedResult = Result<ChartResultObject>.Success(new ChartResultObject());

        // Act
        var result = await _marketPricingApi.GetCurrentPrice(ticker, CancellationToken.None);
    }

    [Test]
    public async Task GetChart_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var symbol = "MSFT";
        var range = YahooRange.FiveDays;
        var interval = YahooInterval.OneHour;
        var additionalFields = new[] { YahooFields.marketCap };
        var expectedResult = Result<ChartResultObject>.Success(new ChartResultObject());

        // Act
        var result = await _marketPricingApi.GetChart(symbol, range, interval, additionalFields, CancellationToken.None);

        // Assert
    }

    [Test]
    public async Task GetHistorical_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var symbol = "GOOG";
        var period1 = new DateTime(2023, 1, 1);
        var period2 = new DateTime(2023, 1, 31);
        var interval = YahooInterval.OneDay;

        // Act
        var result = await _marketPricingApi.GetHistorical(symbol, period1, period2, interval, CancellationToken.None);

        // Assert
    }
    
    [Test]
    public async Task RunScreener_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var screenerId = "most_actives";

        // Act
        var result = await _marketPricingApi.RunScreener(screenerId, CancellationToken.None);

        // Assert
    }

    [Test]
    public async Task Search_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var query = "Tesla";

        // Act
        var result = await _marketPricingApi.Search(query, CancellationToken.None);

        // Assert
    }

    [Test]
    public async Task GetTrendingSymbols_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var region = "US";

        // Act
        var result = await _marketPricingApi.GetTrendingSymbols(region, CancellationToken.None);

        // Assert
    }
}