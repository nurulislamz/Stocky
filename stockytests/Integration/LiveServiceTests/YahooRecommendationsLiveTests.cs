using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using stockyapi.Controllers.Helpers;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;

namespace stockytests.Integration.LiveServiceTests;

[TestFixture]
[Explicit("Calls the real Yahoo Finance API. Run manually only.")]
[Category("Live")]
public class YahooRecommendationsLiveTests
{
    private IYahooFinanceService _yahooFinanceService = null!;

    [SetUp]
    public void SetUp()
    {
        var logger = NullLogger<YahooApiServiceClient>.Instance;
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgents.GetRandomNewUserAgent());

        var serviceClient = new YahooApiServiceClient(httpClient, memoryCache, logger, 1, 3, 2, 10);
        _yahooFinanceService = new YahooFinanceService(serviceClient);
    }

    [Test]
    public async Task GetRecommendationsBySymbol_ReturnsRelatedSymbols()
    {
        var result = await _yahooFinanceService.GetRecommendationsBySymbolAsync("AAPL", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Finance, Is.Not.Null);
        Assert.That(result.Value.Finance.Result, Is.Not.Empty);

        var first = result.Value.Finance.Result.First();
        Assert.That(first.Symbol, Is.Not.Null.And.Not.Empty);
        Assert.That(first.RecommendedSymbols, Is.Not.Null.And.Not.Empty);
        Assert.That(first.RecommendedSymbols.First().Symbol, Is.Not.Null.And.Not.Empty);
    }
}
