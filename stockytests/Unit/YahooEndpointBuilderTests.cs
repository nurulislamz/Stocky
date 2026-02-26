using NUnit.Framework;
using stockyapi.Services.YahooFinance.EndpointBuilder;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class YahooEndpointBuilderTests
{
    [Test]
    public void BuildChartUri_ProducesCorrectPathAndQuery()
    {
        var uri = YahooEndpointBuilder.BuildChartUri("AAPL", YahooRange.OneDay, YahooInterval.OneMinute, [YahooFields.regularMarketPrice]);

        Assert.That(uri.Host, Is.EqualTo("query1.finance.yahoo.com"));
        Assert.That(uri.AbsolutePath, Does.Contain("chart").And.Contain("AAPL"));
        Assert.That(uri.Query, Does.Contain("range=1d"));
        Assert.That(uri.Query, Does.Contain("interval=1m"));
    }

    [Test]
    public void BuildFundamentalsTimeSeriesUri_WithTypes_IncludesTypeParam()
    {
        var uri = YahooEndpointBuilder.BuildFundamentalsTimeSeriesUri("MSFT", "annual", "quarterly");

        Assert.That(uri.AbsolutePath, Does.Contain("timeseries").And.Contain("MSFT"));
        Assert.That(uri.Query, Does.Contain("type="));
    }

    [Test]
    public void BuildInsightsUri_IncludesSymbol()
    {
        var uri = YahooEndpointBuilder.BuildInsightsUri("GOOGL");

        Assert.That(uri.Host, Is.EqualTo("query1.finance.yahoo.com"));
        Assert.That(uri.AbsolutePath, Does.Contain("insights"));
        Assert.That(uri.Query, Does.Contain("symbol=GOOGL"));
    }

    [Test]
    public void BuildOptionsUri_WithDate_IncludesDateParam()
    {
        var uri = YahooEndpointBuilder.BuildOptionsUri("TSLA", "2025-03-21");

        Assert.That(uri.AbsolutePath, Does.Contain("options").And.Contain("TSLA"));
        Assert.That(uri.Query, Does.Contain("date=2025-03-21"));
    }

    [Test]
    public void BuildRecommendationsBySymbolUri_ProducesCorrectPath()
    {
        var uri = YahooEndpointBuilder.BuildRecommendationsBySymbolUri("AAPL");

        Assert.That(uri.AbsolutePath, Does.Contain("recommendationsbysymbol").And.Contain("AAPL"));
    }

    [Test]
    public void BuildScreenerUri_IncludesScreenerId()
    {
        var uri = YahooEndpointBuilder.BuildScreenerUri("day_gainers");

        Assert.That(uri.AbsolutePath, Does.Contain("screener"));
        Assert.That(uri.Query, Does.Contain("scrIds=day_gainers"));
    }

    [Test]
    public void BuildSearchUri_IncludesQuery()
    {
        var uri = YahooEndpointBuilder.BuildSearchUri("Apple");

        Assert.That(uri.AbsolutePath, Does.Contain("search"));
        Assert.That(uri.Query, Does.Contain("q=Apple"));
    }

    [Test]
    public void BuildTrendingSymbolsUri_IncludesRegion()
    {
        var uri = YahooEndpointBuilder.BuildTrendingSymbolsUri("US");

        Assert.That(uri.AbsolutePath, Does.Contain("trending").And.Contain("US"));
    }

    [Test]
    public void BuildHistoricalUri_IncludesPeriodAndInterval()
    {
        var p1 = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var p2 = new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var uri = YahooEndpointBuilder.BuildHistoricalUri("AAPL", p1, p2, "1d");

        Assert.That(uri.AbsolutePath, Does.Contain("chart").And.Contain("AAPL"));
        Assert.That(uri.Query, Does.Contain("period1="));
        Assert.That(uri.Query, Does.Contain("period2="));
        Assert.That(uri.Query, Does.Contain("interval=1d"));
    }

    [Test]
    public void YahooRange_ToApiString_ReturnsExpectedStrings()
    {
        Assert.That(YahooRange.OneDay.ToApiString(), Is.EqualTo("1d"));
        Assert.That(YahooRange.OneMonth.ToApiString(), Is.EqualTo("1mo"));
        Assert.That(YahooRange.OneYear.ToApiString(), Is.EqualTo("1y"));
        Assert.That(YahooRange.Max.ToApiString(), Is.EqualTo("max"));
    }

    [Test]
    public void YahooInterval_ToApiString_ReturnsExpectedStrings()
    {
        Assert.That(YahooInterval.OneMinute.ToApiString(), Is.EqualTo("1m"));
        Assert.That(YahooInterval.FifteenMinutes.ToApiString(), Is.EqualTo("15m"));
        Assert.That(YahooInterval.OneDay.ToApiString(), Is.EqualTo("1d"));
    }
}
