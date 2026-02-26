using Moq;
using NUnit.Framework;
using stockyapi.Application.MarketPricing;
using stockyapi.Middleware;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class MarketPricingApiTests
{
    private Mock<IYahooFinanceService> _yahoo = null!;
    private MarketPricingApi _api = null!;
    private static readonly CancellationToken Token = CancellationToken.None;

    [SetUp]
    public void SetUp()
    {
        _yahoo = new Mock<IYahooFinanceService>();
        _api = new MarketPricingApi(_yahoo.Object);
    }

    [Test]
    public async Task GetCurrentPrice_CallsGetChart_WithOneDayAndFifteenMinutes()
    {
        var expected = new YahooChartResponse();
        _yahoo.Setup(x => x.GetChartAsync("AAPL", YahooRange.OneDay, YahooInterval.FifteenMinutes, It.IsAny<YahooFields[]>(), Token))
            .ReturnsAsync(Result<YahooChartResponse>.Success(expected));

        var result = await _api.GetCurrentPrice("AAPL", Token);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.SameAs(expected));
        _yahoo.Verify(x => x.GetChartAsync("AAPL", YahooRange.OneDay, YahooInterval.FifteenMinutes, It.IsAny<YahooFields[]>(), Token), Times.Once);
    }

    [Test]
    public async Task GetCurrentPrice_WhenYahooFails_ReturnsFailure()
    {
        var failure = new InternalServerFailure500("API error");
        _yahoo.Setup(x => x.GetChartAsync(It.IsAny<string>(), It.IsAny<YahooRange>(), It.IsAny<YahooInterval>(), It.IsAny<YahooFields[]>(), Token))
            .ReturnsAsync(Result<YahooChartResponse>.Fail(failure));

        var result = await _api.GetCurrentPrice("INVALID", Token);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.SameAs(failure));
    }

    [Test]
    public async Task GetChart_ConcatsRegularMarketPrice_WithAdditionalFields()
    {
        var expected = new YahooChartResponse();
        _yahoo.Setup(x => x.GetChartAsync("MSFT", YahooRange.OneMonth, YahooInterval.OneDay, It.IsAny<YahooFields[]>(), Token))
            .ReturnsAsync(Result<YahooChartResponse>.Success(expected));

        var result = await _api.GetChart("MSFT", YahooRange.OneMonth, YahooInterval.OneDay, new[] { YahooFields.fiftyTwoWeekHigh }, Token);

        Assert.That(result.IsSuccess, Is.True);
        _yahoo.Verify(x => x.GetChartAsync("MSFT", YahooRange.OneMonth, YahooInterval.OneDay,
            It.Is<YahooFields[]>(a => a.Length == 2 && a[0] == YahooFields.regularMarketPrice && a[1] == YahooFields.fiftyTwoWeekHigh), Token), Times.Once);
    }

    [Test]
    public async Task GetChart_WhenAdditionalFieldsNull_StillIncludesRegularMarketPrice()
    {
        _yahoo.Setup(x => x.GetChartAsync(It.IsAny<string>(), It.IsAny<YahooRange>(), It.IsAny<YahooInterval>(), It.IsAny<YahooFields[]>(), Token))
            .ReturnsAsync(Result<YahooChartResponse>.Success(new YahooChartResponse()));

        await _api.GetChart("GOOGL", YahooRange.OneDay, YahooInterval.OneMinute, null, Token);

        _yahoo.Verify(x => x.GetChartAsync("GOOGL", YahooRange.OneDay, YahooInterval.OneMinute, It.Is<YahooFields[]>(a => a.Length == 1 && a[0] == YahooFields.regularMarketPrice), Token), Times.Once);
    }

    [Test]
    public async Task GetHistorical_DelegatesToYahoo()
    {
        var p1 = DateTime.UtcNow.AddDays(-7);
        var p2 = DateTime.UtcNow;
        var expected = new HistoricalHistoryResult();
        _yahoo.Setup(x => x.GetHistoricalAsync("TSLA", p1, p2, YahooInterval.OneDay, Token))
            .ReturnsAsync(Result<HistoricalHistoryResult>.Success(expected));

        var result = await _api.GetHistorical("TSLA", p1, p2, YahooInterval.OneDay, Token);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.SameAs(expected));
        _yahoo.Verify(x => x.GetHistoricalAsync("TSLA", p1, p2, YahooInterval.OneDay, Token), Times.Once);
    }

    [Test]
    public async Task Search_WhenSuccess_ReturnsValue()
    {
        var expected = new SearchResult();
        _yahoo.Setup(x => x.SearchAsync("Apple", Token))
            .ReturnsAsync(Result<SearchResult>.Success(expected));

        var result = await _api.Search("Apple", Token);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.SameAs(expected));
    }

    [Test]
    public async Task Search_WhenFailure_ReturnsFailure()
    {
        var failure = new InternalServerFailure500("Error");
        _yahoo.Setup(x => x.SearchAsync(It.IsAny<string>(), Token))
            .ReturnsAsync(Result<SearchResult>.Fail(failure));

        var result = await _api.Search("x", Token);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.SameAs(failure));
    }

    [Test]
    public async Task RunScreener_DelegatesToYahoo()
    {
        var expected = new ScreenerResponse();
        _yahoo.Setup(x => x.RunScreenerAsync("day_gainers", Token))
            .ReturnsAsync(Result<ScreenerResponse>.Success(expected));

        var result = await _api.RunScreener("day_gainers", Token);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.SameAs(expected));
    }

    [Test]
    public async Task GetTrendingSymbols_DelegatesToYahoo()
    {
        var expected = new TrendingSymbolsResponse();
        _yahoo.Setup(x => x.GetTrendingSymbolsAsync("US", Token))
            .ReturnsAsync(Result<TrendingSymbolsResponse>.Success(expected));

        var result = await _api.GetTrendingSymbols("US", Token);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.SameAs(expected));
    }
}
