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
public class YahooSearchLiveTests
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
    public async Task Search_ReturnsTSLAInTeslaResults()
    {
        var result = await _marketPricingApi.Search("Tesla", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Count, Is.GreaterThan(0));
        Assert.That(result.Value.Quotes, Is.Not.Null.And.Not.Empty);

        var tsla = result.Value.Quotes.FirstOrDefault(q => q.Symbol == "TSLA");
        Assert.That(tsla, Is.Not.Null, "Expected TSLA in results for query 'Tesla'");
        Assert.That(tsla!.Shortname, Is.Not.Null.And.Not.Empty);
        Assert.That(tsla.Exchange, Is.Not.Null.And.Not.Empty);
        Assert.That(tsla.QuoteType, Is.Not.Null.And.Not.Empty);
    }
}
