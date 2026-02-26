using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using stockyapi.Application.MarketPricing;
using stockyapi.Controllers.Helpers;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Helper;

namespace stockytests.Integration.LiveServiceTests;

[TestFixture]
[Explicit("Calls the real Yahoo Finance API. Run manually only.")]
[Category("Live")]
public class YahooChartLiveTests
{
    private IMarketPricingApi _marketPricingApi = null!;

    [SetUp]
    public void SetUp()
    {
        var logger = NullLogger<YahooApiServiceClient>.Instance;
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgents.GetRandomNewUserAgent());

        var serviceClient = new YahooApiServiceClient(httpClient, memoryCache, logger, 1, 3, 2, 10);
        _marketPricingApi = new MarketPricingApi(new YahooFinanceService(serviceClient));
    }

    [Test]
    public async Task GetCurrentPrice_ReturnsMarketPriceForSymbol()
    {
        var ticker = "AAPL";

        var result = await _marketPricingApi.GetCurrentPrice(ticker, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);

        var meta = result.Value.Chart.Result!.First().Meta;
        Assert.That(meta.Symbol, Is.EqualTo(ticker));
        Assert.That(meta.RegularMarketPrice, Is.GreaterThan(0));
        Assert.That(meta.Currency, Is.Not.Null.And.Not.Empty);
        Assert.That(meta.ExchangeName, Is.Not.Null.And.Not.Empty);
        Assert.That(meta.InstrumentType, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task GetChart_ReturnsTimestampsAndIndicators()
    {
        var symbol = "MSFT";
        var range = YahooRange.FiveDays;
        var interval = YahooInterval.OneHour;

        var result = await _marketPricingApi.GetChart(symbol, range, interval, [YahooFields.marketCap], CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Chart.Result, Is.Not.Null.And.Not.Empty);

        var chartResult = result.Value.Chart.Result!.First();
        Assert.That(chartResult.Meta.Symbol, Is.EqualTo(symbol));
        Assert.That(chartResult.Meta.Currency, Is.Not.Null.And.Not.Empty);
        Assert.That(chartResult.Timestamp, Is.Not.Null.And.Not.Empty);

        var quote = chartResult.Indicators.Quote.First();
        Assert.That(quote.Close, Is.Not.Empty);
        Assert.That(quote.Close.Count, Is.EqualTo(chartResult.Timestamp!.Count));
    }
}
