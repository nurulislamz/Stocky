using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Extensions.Http;
using stockyapi.Failures;
using stockyapi.Middleware;

namespace stockyapi.Repository.YahooFinance.Helper;

public sealed class YahooExecutionHelper
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<YahooExecutionHelper> _logger;
    private readonly SemaphoreSlim _rateLimiter;
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public YahooExecutionHelper(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<YahooExecutionHelper> logger,
        int maxConcurrentRequests = 50)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _rateLimiter = new SemaphoreSlim(maxConcurrentRequests);
        _policy = Policy.WrapAsync(CreateRetryPolicy(_logger), CreateCircuitBreakerPolicy());
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
            var response = await _policy.ExecuteAsync(_ => _httpClient.GetAsync(uri, ct), ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Yahoo returned {StatusCode} for {Uri}. Response is below: {Response}",
                    response.StatusCode,
                    uri,
                    response.Content.ToString());

                return new InternalServerFailure500(
                    $"Yahoo Finance API error: {response.StatusCode}");
            }

            var payload = await response.Content.ReadFromJsonAsync<T>(ct);

            if (payload is null)
                return new InternalServerFailure500("Yahoo returned empty payload");

            _cache.Set(cacheKey, payload, cacheTtl);

            return Result<T>.Success(payload);
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

    private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(ILogger logger) =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)) +
                    TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                onRetry: (outcome, delay, attempt, _) =>
                {
                    logger.LogWarning(
                        "Retry {Attempt} after {Delay}ms due to {Reason}",
                        attempt,
                        delay.TotalMilliseconds,
                        outcome.Exception?.Message ?? nameof(outcome.Result.StatusCode));
                });
    
    private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy() =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
    
}
