using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlockchainMonitor.Infrastructure.Services;

public class LocalMetricsService : IMetricsService
{
    private readonly ILogger<LocalMetricsService> _logger;
    private readonly Dictionary<string, long> _requestCounts = new();
    private readonly Dictionary<string, long> _errorCounts = new();
    private readonly List<TimeSpan> _responseTimes = new();
    private readonly Dictionary<string, long> _blockchainDataFetched = new();
    private readonly Dictionary<string, long> _cacheHits = new();
    private readonly Dictionary<string, long> _cacheMisses = new();
    private readonly List<TimeSpan> _databaseOperations = new();
    private readonly Dictionary<string, long> _rateLimitExceeded = new();
    private readonly object _lock = new();

    public LocalMetricsService(ILogger<LocalMetricsService> logger)
    {
        _logger = logger;
    }

    public void IncrementRequestCount(string endpoint)
    {
        lock (_lock)
        {
            _requestCounts["total"] = _requestCounts.GetValueOrDefault("total", 0) + 1;
            _logger.LogDebug("Request count incremented");
        }
    }

    public void IncrementErrorCount(string endpoint, string errorType)
    {
        lock (_lock)
        {
            _errorCounts["total"] = _errorCounts.GetValueOrDefault("total", 0) + 1;
            _logger.LogWarning("Error count incremented for error type: {ErrorType}", errorType);
        }
    }

    public void RecordResponseTime(string endpoint, TimeSpan duration)
    {
        lock (_lock)
        {
            _responseTimes.Add(duration);
            
            // Keep only last 100 measurements to prevent memory growth
            if (_responseTimes.Count > 100)
                _responseTimes.RemoveAt(0);
        }
    }

    public void IncrementBlockchainDataFetched(string blockchainName)
    {
        lock (_lock)
        {
            _blockchainDataFetched["total"] = _blockchainDataFetched.GetValueOrDefault("total", 0) + 1;
            _logger.LogInformation("Blockchain data fetched for: {BlockchainName}", blockchainName);
        }
    }

    public void IncrementCacheHit(string cacheKey)
    {
        lock (_lock)
        {
            _cacheHits["total"] = _cacheHits.GetValueOrDefault("total", 0) + 1;
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
        }
    }

    public void IncrementCacheMiss(string cacheKey)
    {
        lock (_lock)
        {
            _cacheMisses["total"] = _cacheMisses.GetValueOrDefault("total", 0) + 1;
            _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
        }
    }

    public void RecordDatabaseOperation(string operation, TimeSpan duration)
    {
        lock (_lock)
        {
            _databaseOperations.Add(duration);
            
            // Keep only last 50 measurements to prevent memory growth
            if (_databaseOperations.Count > 50)
                _databaseOperations.RemoveAt(0);
        }
    }

    public void IncrementRateLimitExceeded(string endpoint)
    {
        lock (_lock)
        {
            _rateLimitExceeded["total"] = _rateLimitExceeded.GetValueOrDefault("total", 0) + 1;
            _logger.LogWarning("Rate limit exceeded");
        }
    }

    public Task<Dictionary<string, object>> GetMetrics()
    {
        lock (_lock)
        {
            var metrics = new Dictionary<string, object>
            {
                ["request_count"] = _requestCounts.GetValueOrDefault("total", 0),
                ["error_count"] = _errorCounts.GetValueOrDefault("total", 0),
                ["response_time"] = _responseTimes.Any() ? _responseTimes.Average(ts => ts.TotalMilliseconds) : 0.0,
                ["response_time_count"] = _responseTimes.Count,
                ["blockchain_data_fetched"] = _blockchainDataFetched.GetValueOrDefault("total", 0),
                ["cache_hit"] = _cacheHits.GetValueOrDefault("total", 0),
                ["cache_miss"] = _cacheMisses.GetValueOrDefault("total", 0),
                ["database_operation"] = _databaseOperations.Any() ? _databaseOperations.Average(ts => ts.TotalMilliseconds) : 0.0,
                ["database_operation_count"] = _databaseOperations.Count,
                ["rate_limit_exceeded"] = _rateLimitExceeded.GetValueOrDefault("total", 0)
            };
            
            return Task.FromResult(metrics);
        }
    }
} 