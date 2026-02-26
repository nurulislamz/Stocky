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
public class YahooHistoricalLiveTests
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
    public async Task GetHistorical_ReturnsDailyRowsInChronologicalOrder()
    {
        var symbol = "GOOG";
        var period1 = DateTime.UtcNow.AddMonths(-1);
        var period2 = DateTime.UtcNow;

        var result = await _marketPricingApi.GetHistorical(symbol, period1, period2, YahooInterval.OneDay, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Empty);
        Assert.That(result.Value.Count, Is.GreaterThan(1));

        var firstRow = result.Value.First();
        Assert.That(firstRow.Close, Is.GreaterThan(0));
        Assert.That(firstRow.Open, Is.GreaterThan(0));
        Assert.That(firstRow.Volume, Is.GreaterThanOrEqualTo(0));

        Assert.That(result.Value.Last().Date, Is.GreaterThan(firstRow.Date));
    }
}
