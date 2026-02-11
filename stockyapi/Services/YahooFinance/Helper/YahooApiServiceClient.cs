using Microsoft.Extensions.Caching.Memory;

namespace stockyapi.Services.YahooFinance.Helper;

public class YahooApiServiceClient : BaseApiServiceClient
{
    public YahooApiServiceClient(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<YahooApiServiceClient> logger,
        int maxConcurrentRequests,
        int retryCount,
        int breakLimit,
        int breakDuration)
        : base(httpClient, cache, logger, maxConcurrentRequests, retryCount, breakLimit, breakDuration)
    {
    }
}
