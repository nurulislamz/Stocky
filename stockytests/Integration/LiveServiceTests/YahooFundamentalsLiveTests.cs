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
public class YahooFundamentalsLiveTests
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
    public async Task GetFundamentalsTimeSeries_ReturnsTimeSeriesWithMeta()
    {
        var symbol = "AAPL";

        var result = await _yahooFinanceService.GetFundamentalsTimeSeriesAsync(
            symbol,
            CancellationToken.None,
            "annualTotalRevenue");

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Timeseries, Is.Not.Null);
        Assert.That(result.Value.Timeseries.Result, Is.Not.Empty);

        var first = result.Value.Timeseries.Result.First();
        Assert.That(first.Meta.Symbol, Is.Not.Empty);
        Assert.That(first.Meta.Type, Is.Not.Empty);
    }
}
