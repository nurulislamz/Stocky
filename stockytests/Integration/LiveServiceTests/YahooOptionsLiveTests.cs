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
public class YahooOptionsLiveTests
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
    public async Task GetOptions_ReturnsOptionsChain()
    {
        var symbol = "AAPL";

        var result = await _yahooFinanceService.GetOptionsAsync(symbol, CancellationToken.None);

        // Options endpoint requires a valid Yahoo crumb/session; skip if unauthorized
        if (!result.IsSuccess && result.Failure?.Detail?.Contains("Unauthorized") == true)
            Assert.Ignore("Options endpoint requires a valid Yahoo Finance session (crumb).");

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.OptionChain.Result, Is.Not.Empty);

        var optionsResult = result.Value.OptionChain.Result.First();
        Assert.That(optionsResult.UnderlyingSymbol, Is.EqualTo(symbol));
        Assert.That(optionsResult.ExpirationDates, Is.Not.Empty);
        Assert.That(optionsResult.Strikes, Is.Not.Empty);
        Assert.That(optionsResult.Options, Is.Not.Empty);
    }

    [Test]
    public async Task GetOptions_WithExpiryDate_ReturnsFilteredChain()
    {
        var symbol = "AAPL";

        var baseResult = await _yahooFinanceService.GetOptionsAsync(symbol, CancellationToken.None);

        // Options endpoint requires a valid Yahoo crumb/session; skip if unauthorized
        if (!baseResult.IsSuccess && baseResult.Failure?.Detail?.Contains("Unauthorized") == true)
            Assert.Ignore("Options endpoint requires a valid Yahoo Finance session (crumb).");

        Assert.That(baseResult.IsSuccess, Is.True);
        Assert.That(baseResult.Value.OptionChain.Result, Is.Not.Empty);

        var firstExpiry = baseResult.Value.OptionChain.Result.First().ExpirationDates.First();
        var result = await _yahooFinanceService.GetOptionsAsync(symbol, firstExpiry.ToUnixTimeSeconds().ToString(), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.OptionChain.Result, Is.Not.Empty);

        var optionsResult = result.Value.OptionChain.Result.First();
        Assert.That(optionsResult.UnderlyingSymbol, Is.EqualTo(symbol));
        Assert.That(optionsResult.Options, Is.Not.Empty);

        var chain = optionsResult.Options.First();
        Assert.That(chain.Calls.Count + chain.Puts.Count, Is.GreaterThan(0));
    }
}
