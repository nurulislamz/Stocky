namespace stockyapi.Services.YahooFinance.EndpointBuilder;

public enum YahooInterval
{
    OneMinute,
    TwoMinutes,
    FiveMinutes,
    FifteenMinutes,
    ThirtyMinutes,
    SixtyMinutes,
    NinetyMinutes,
    OneHour,
    OneDay,
    FiveDays,
    OneWeek,
    OneMonth,
    ThreeMonths
}

public static class YahooIntervalExtension
{
    public static string ToApiString(this YahooInterval interval) => interval switch
    {
        YahooInterval.OneMinute => "1m",
        YahooInterval.TwoMinutes => "2m",
        YahooInterval.FiveMinutes => "5m",
        YahooInterval.FifteenMinutes => "15m",
        YahooInterval.ThirtyMinutes => "30m",
        YahooInterval.SixtyMinutes => "60m",
        YahooInterval.NinetyMinutes => "90m",
        YahooInterval.OneHour => "1h",
        YahooInterval.OneDay => "1d",
        YahooInterval.FiveDays => "5d",
        YahooInterval.OneWeek => "1wk",
        YahooInterval.OneMonth => "1mo",
        YahooInterval.ThreeMonths => "3mo",
        _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
    };
}
