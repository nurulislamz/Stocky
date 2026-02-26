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
public class YahooTrendingSymbolsLiveTests
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
    public async Task GetTrendingSymbols_ReturnsQuotesWithSymbols()
    {
        var result = await _marketPricingApi.GetTrendingSymbols("US", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);

        var trendingResult = result.Value.Finance.Result.First();
        Assert.That(trendingResult.Count, Is.GreaterThan(0));
        Assert.That(trendingResult.Quotes, Is.Not.Null.And.Not.Empty);
        Assert.That(trendingResult.Quotes.First().Symbol, Is.Not.Null.And.Not.Empty);
    }
}
