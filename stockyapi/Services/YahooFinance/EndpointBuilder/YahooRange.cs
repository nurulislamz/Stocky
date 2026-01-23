namespace stockyapi.Services.YahooFinance.Types;

public enum YahooRange
{
    OneDay,
    FiveDays,
    OneMonth,
    ThreeMonths,
    SixMonths,
    OneYear,
    TwoYears,
    FiveYears,
    TenYears,
    YearToDate,
    Max
}

public static class YahooRangedExtensions
{
    public static string ToApiString(this YahooRange range) => range switch
    {
        YahooRange.OneDay => "1d",
        YahooRange.FiveDays => "5d",
        YahooRange.OneMonth => "1mo",
        YahooRange.ThreeMonths => "3mo",
        YahooRange.SixMonths => "6mo",
        YahooRange.OneYear => "1y",
        YahooRange.TwoYears => "2y",
        YahooRange.FiveYears => "5y",
        YahooRange.TenYears => "10y",
        YahooRange.YearToDate => "ytd",
        YahooRange.Max => "max",
        _ => throw new ArgumentOutOfRangeException(nameof(range), range, null)
    };
}