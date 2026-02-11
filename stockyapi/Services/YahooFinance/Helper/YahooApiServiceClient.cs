using Microsoft.Extensions.Caching.Memory;

namespace stockyapi.Services.YahooFinance.Helper;

public class YahooApiServiceClient : BaseApiServiceClient
{
    public YahooApiServiceClient(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<YahooApiServiceClient> logger) 
        : base(httpClient, cache, logger)
    {
    }
}
