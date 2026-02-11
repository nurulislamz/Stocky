using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Extensions.Http;
using Polly.Wrap;
using stockyapi.Controllers.Helpers;
using stockyapi.Middleware;

namespace stockyapi.Services.YahooFinance.Helper;

public abstract class BaseApiServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BaseApiServiceClient> _logger;
    private readonly SemaphoreSlim _rateLimiter;
    private readonly Lazy<AsyncPolicyWrap<HttpResponseMessage>> _policy;

    private int _maxConcurrencyRequests;
    private readonly int _retryCount;
    private readonly int _breakLimit;
    private readonly int _breakDuration;

    protected BaseApiServiceClient(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<BaseApiServiceClient> logger,
        int maxConcurrentRequests = 50,
        int retryCount = 3,
        int breakLimit = 5,
        int breakDuration = 30
        )
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _rateLimiter = new SemaphoreSlim(maxConcurrentRequests);
        _maxConcurrencyRequests = maxConcurrentRequests;
        _retryCount = retryCount;
        _breakLimit = breakLimit;
        _breakDuration = breakDuration;
        
        _policy = new Lazy<AsyncPolicyWrap<HttpResponseMessage>>(() => 
            Policy.WrapAsync(CreateRetryPolicy(), CreateCircuitBreakerPolicy()));
    }

    public async Task<Result<T>> ExecuteAsync<T>(
        string cacheKey,
        TimeSpan cacheTtl,
        Uri uri,
        CancellationToken ct)
    {
        // TODO: Implement fusionCache
        if (_cache.TryGetValue(cacheKey, out T? cached) && cached is not null)
            return Result<T>.Success(cached);

        await _rateLimiter.WaitAsync(ct);

        try
        {
            var response = await _policy.Value.ExecuteAsync(_ => _httpClient.GetAsync(uri, ct), ct);

            if (!response.IsSuccessStatusCode)
            {
                string error = $"Yahoo returned {response.StatusCode} for {uri}. Response is below: {response.Content.ReadAsStringAsync(ct)}";
                _logger.LogError(error);
                return new InternalServerFailure500(error);
            }

            var payload = await response.Content.ReadFromJsonAsync<T>(ct);

            if (payload is null)
                return new InternalServerFailure500("Yahoo returned empty payload");

            _cache.Set(cacheKey, payload, cacheTtl);

            return payload;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Propagate cancellation correctly
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Yahoo request failed: {Uri}", uri);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _rateLimiter.Release();
        }
    }

    protected virtual IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy() =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: _retryCount,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)) +
                    TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                onRetry: (outcome, delay, attempt, _) =>
                {
                    _logger.LogWarning(
                        "Retry {Attempt} after {Delay}ms due to {Reason}",
                        attempt,
                        delay.TotalMilliseconds,
                        outcome.Exception?.Message ?? nameof(outcome.Result.StatusCode));
                    
                    if (outcome.Result.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        string oldUserAgent = _httpClient.DefaultRequestHeaders.UserAgent.ToString();
                        string newUserAgent = UserAgents.GetRandomNewUserAgent(oldUserAgent);

                        _logger.LogInformation($"429 TooManyRequests detected. Rotating User-Agent from {oldUserAgent}.");

                        // IMPORTANT: Clear the old headers first
                        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
    
                        // Use ParseAdd to ensure the string is correctly formatted for the header
                        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(newUserAgent);
    
                        _logger.LogInformation("New User-Agent applied: {newUserAgent}", newUserAgent);
                    }
                });
    
    protected virtual IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy() =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: _breakLimit,
                durationOfBreak: TimeSpan.FromSeconds(_breakDuration));
}
