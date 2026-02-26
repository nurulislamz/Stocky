using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using stockyapi.Application.MarketPricing;
using stockyapi.Controllers.Helpers;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;

namespace stockytests.Integration.LiveServiceTests;

[TestFixture]
[Explicit("Calls the real Yahoo Finance API. Run manually only.")]
[Category("Live")]
public class YahooScreenerLiveTests
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
    public async Task RunScreener_ReturnsQuotesWithPriceData()
    {
        var result = await _marketPricingApi.RunScreener("day_gainers", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);

        var screenerResult = result.Value.Finance.Result.First();
        Assert.That(screenerResult.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(screenerResult.Title, Is.Not.Null.And.Not.Empty);
        Assert.That(screenerResult.Total, Is.GreaterThan(0));
        Assert.That(screenerResult.Quotes, Is.Not.Null.And.Not.Empty);

        var firstQuote = screenerResult.Quotes.First();
        Assert.That(firstQuote.Symbol, Is.Not.Null.And.Not.Empty);
        Assert.That(firstQuote.RegularMarketPrice, Is.GreaterThan(0));
    }
}
