using System.Net;
using System.Text.Json;
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
        _retryCount = retryCount;
        _breakLimit = breakLimit;
        _breakDuration = breakDuration;

        // Circuit breaker outer: fail fast when open. Retry inner: retries only when circuit allows.
        _policy = new Lazy<AsyncPolicyWrap<HttpResponseMessage>>(() =>
            Policy.WrapAsync(CreateCircuitBreakerPolicy(), CreateRetryPolicy()));
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

            // Read body once; HttpContent stream is not rewindable.
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = $"Yahoo returned {response.StatusCode}. Response: {body}";
                _logger.LogError("{Error}", error);
                return new InternalServerFailure500(error);
            }

            _logger.LogDebug("Yahoo returned {StatusCode} for {Uri}", response.StatusCode, uri);

            T? payload;
            try
            {
                payload = JsonSerializer.Deserialize<T>(body);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from {Uri}", uri);
                return new InternalServerFailure500($"Invalid response: {ex.Message}");
            }

            if (payload is null)
                return new InternalServerFailure500("Yahoo returned empty payload");

            _cache.Set(cacheKey, payload, cacheTtl);

            return payload;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
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
                    var reason = outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown";
                    _logger.LogWarning(
                        "Retry {Attempt} after {Delay}ms due to {Reason}",
                        attempt,
                        delay.TotalMilliseconds,
                        reason);

                    if (outcome.Result?.StatusCode == HttpStatusCode.TooManyRequests)
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
