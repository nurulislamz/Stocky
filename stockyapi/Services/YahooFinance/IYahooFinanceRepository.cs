using stockyapi.Responses;

namespace stockyapi.Repository.YahooFinance;

public interface IYahooFinanceRepository
{
    Task<(CurrentPriceData? Data, string? Error)> GetCurrentTickerPriceAsync(string ticker);
    Task<(HistoricalPriceData? Data, string? Error)> GetHistoricalTickerPriceAsync(string ticker, string startTimestamp, string endTimestamp);
}
