using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
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
public class YahooLiveServiceTests
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

        // Act
        var result = await _marketPricingApi.GetCurrentPrice(ticker, CancellationToken.None);
        
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Chart, Is.Not.Null);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);
        
        var meta = result.Value.Chart.Result!.First().Meta;
        Assert.That(meta.Symbol, Is.EqualTo(ticker));
        Assert.That(meta.RegularMarketPrice, Is.GreaterThan(0));
        Assert.That(meta.Currency, Is.Not.Null.And.Not.Empty);
        Assert.That(meta.ExchangeName, Is.Not.Null.And.Not.Empty);
        Assert.That(meta.InstrumentType, Is.Not.Null.And.Not.Empty);
        Assert.That(meta.FirstTradeDate, Is.Not.Null);
    }

    [Test]
    public async Task GetChart_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var symbol = "MSFT";
        var range = YahooRange.FiveDays;
        var interval = YahooInterval.OneHour;
        var additionalFields = new[] { YahooFields.marketCap };

        // Act
        var result = await _marketPricingApi.GetChart(symbol, range, interval, additionalFields, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Chart, Is.Not.Null);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);
        
        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo(symbol));
        Assert.That(chartResult.Meta.DataGranularity, Is.Not.Null.And.Not.Empty);
        Assert.That(chartResult.Meta.Range, Is.Not.Null.And.Not.Empty);
        Assert.That(chartResult.Meta.Currency, Is.Not.Null.And.Not.Empty);
        
        Assert.That(chartResult.Timestamp, Is.Not.Null.And.Not.Empty);
        Assert.That(chartResult.Indicators.Quote, Is.Not.Null.And.Not.Empty);
        
        var quote = chartResult.Indicators.Quote.First();
        Assert.That(quote.Close, Is.Not.Empty);
        Assert.That(quote.Close.Count, Is.EqualTo(chartResult.Timestamp!.Count));
        Assert.That(quote.High, Is.Not.Empty);
        Assert.That(quote.Low, Is.Not.Empty);
        Assert.That(quote.Open, Is.Not.Empty);
        Assert.That(quote.Volume, Is.Not.Empty);
    }

    [Test]
    public async Task GetHistorical_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var symbol = "GOOG";
        var period1 = DateTime.UtcNow.AddMonths(-1);
        var period2 = DateTime.UtcNow;
        var interval = YahooInterval.OneDay;

        // Act
        var result = await _marketPricingApi.GetHistorical(symbol, period1, period2, interval, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value, Is.Not.Empty);
        Assert.That(result.Value.Count, Is.GreaterThan(1));

        var firstRow = result.Value.First();
        Assert.That(firstRow.Close, Is.GreaterThan(0));
        Assert.That(firstRow.Open, Is.GreaterThan(0));
        Assert.That(firstRow.High, Is.GreaterThan(0));
        Assert.That(firstRow.Low, Is.GreaterThan(0));
        Assert.That(firstRow.Volume, Is.GreaterThanOrEqualTo(0));
        
        var lastRow = result.Value.Last();
        Assert.That(lastRow.Date, Is.GreaterThan(firstRow.Date));
    }
    
    [Test]
    public async Task RunScreener_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var screenerId = "day_gainers";

        // Act
        var result = await _marketPricingApi.RunScreener(screenerId, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Value.Title, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Value.Total, Is.GreaterThan(0));
        Assert.That(result.Value.Quotes, Is.Not.Null.And.Not.Empty);
        
        var firstQuote = result.Value.Quotes.First();
        Assert.That(firstQuote.Symbol, Is.Not.Null.And.Not.Empty);
        Assert.That(firstQuote.ShortName, Is.Not.Null.And.Not.Empty);
        Assert.That(firstQuote.RegularMarketPrice, Is.GreaterThan(0));
        Assert.That(firstQuote.Exchange, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Search_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var query = "Tesla";

        // Act
        var result = await _marketPricingApi.Search(query, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Count, Is.GreaterThan(0));
        Assert.That(result.Value.Quotes, Is.Not.Null.And.Not.Empty);
        
        var tslaQuote = result.Value.Quotes.FirstOrDefault(q => q.Symbol == "TSLA");
        Assert.That(tslaQuote, Is.Not.Null, "Should find TSLA in search results");
        Assert.That(tslaQuote!.Shortname, Is.Not.Null.And.Not.Empty);
        Assert.That(tslaQuote.Exchange, Is.Not.Null.And.Not.Empty);
        Assert.That(tslaQuote.QuoteType, Is.Not.Null.And.Not.Empty);
        
        Assert.That(result.Value.News, Is.Not.Null);
    }

    [Test]
    public async Task GetTrendingSymbols_ShouldCallYahooFinanceService_WithCorrectParameters()
    {
        // Arrange
        var region = "US";

        // Act
        var result = await _marketPricingApi.GetTrendingSymbols(region, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Count, Is.GreaterThan(0));
        Assert.That(result.Value.JobTimestamp, Is.GreaterThan(0));
        Assert.That(result.Value.Quotes, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Value.Quotes.First().Symbol, Is.Not.Null.And.Not.Empty);
    }
}