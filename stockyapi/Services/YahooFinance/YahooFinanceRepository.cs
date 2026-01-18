using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using stockyapi.Responses;

namespace stockyapi.Repository.YahooFinance;

public class YahooFinanceRepository : IYahooFinanceRepository
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<YahooFinanceRepository> _logger;
    private readonly SemaphoreSlim _throttler;
    private const int MaxRequestsPerMinute = 50;

    public YahooFinanceRepository(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<YahooFinanceRepository> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _throttler = new SemaphoreSlim(MaxRequestsPerMinute);
    }


    public async Task<(CurrentPriceData? Data, string? Error)> GetCurrentTickerPriceAsync(string ticker)
    {
        var url = $"chart/{ticker}?interval=1d&range=1d";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to fetch data for {ticker}. Status: {response.StatusCode}");
            return (null, $"Failed to fetch price data. Status code: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<YahooFinanceResponse>(content);

        if (data == null || data.Chart.Error != null || !data.Chart.Data.Any())
        {
            _logger.LogError($"Failed to parse data for {ticker} to json, Invalid body");
            return (null, $"No price data available for {ticker}");
        }

        var result = data.Chart.Data[0];

        if (result.Timestamp == null || result.Timestamp.Count == 0)
        {
            _logger.LogError($"No timestamp data available for {ticker}");
            _ = Task.Delay(TimeSpan.FromMinutes(1))
                .ContinueWith(_ => _throttler.Release());
            return (null, $"No timestamp data available for {ticker}");
        }

        var priceData = new CurrentPriceData
        {
            Symbol = ticker,
            CurrentPrice = result.Meta.RegularMarketPrice,
            Timestamp = DateTimeOffset.FromUnixTimeSeconds(result.Timestamp.FirstOrDefault()).UtcDateTime
        };
        return (priceData, null);
    }
    
    
    public async Task<(HistoricalPriceData? Data, string? Error)> GetHistoricalTickerPriceAsync(string ticker, string startTimestamp, string endTimestamp)
    {
        string url = $"chart/{ticker}?interval=1d&range=1d&start={startTimestamp}&end={endTimestamp}";
        var cacheKey = $"price_data_{ticker}";

        if (_cache.TryGetValue(cacheKey, out HistoricalPriceData cachedData))
        {
            return (cachedData, null);
        }

        await _throttler.WaitAsync();
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to fetch data for {ticker}. Status: {response.StatusCode}");
            _ = Task.Delay(TimeSpan.FromMinutes(1))
                .ContinueWith(_ => _throttler.Release());
            return (null, $"Failed to fetch price data. Status code: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<YahooFinanceResponse>(content);

        if (data == null || data.Chart.Error != null || !data.Chart.Data.Any())
        {
            _logger.LogError($"Failed to parse data for {ticker}");
            _ = Task.Delay(TimeSpan.FromMinutes(1))
                .ContinueWith(_ => _throttler.Release());
            return (null, $"No price data available for {ticker}");
        }

        var result = data.Chart.Data[0];

        if (result.Timestamp == null || result.Timestamp.Count == 0)
        {
            _logger.LogError($"No timestamp data available for {ticker}");
            _ = Task.Delay(TimeSpan.FromMinutes(1))
                .ContinueWith(_ => _throttler.Release());
            return (null, $"No timestamp data available for {ticker}");
        }

        var priceData = new HistoricalPriceData
        {
            Symbol = ticker,
            CurrentPrice = result.Meta.RegularMarketPrice,
            Timestamp = DateTimeOffset.FromUnixTimeSeconds(result.Timestamp.FirstOrDefault()).UtcDateTime
        };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, priceData, cacheOptions);

        _ = Task.Delay(TimeSpan.FromMinutes(1))
            .ContinueWith(_ => _throttler.Release());

        return (priceData, null);
    }
}
