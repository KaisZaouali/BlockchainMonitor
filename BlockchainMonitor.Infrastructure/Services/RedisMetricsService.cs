using BlockchainMonitor.Infrastructure.Interfaces;
using BlockchainMonitor.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace BlockchainMonitor.Infrastructure.Services;

public class RedisMetricsService : IMetricsService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisMetricsService> _logger;
    private readonly MetricsSettings _settings;
    private readonly string _keyPrefix = "metrics:";

    public RedisMetricsService(IConnectionMultiplexer redis, ILogger<RedisMetricsService> logger, IOptions<MetricsSettings> settings)
    {
        _redis = redis;
        _logger = logger;
        _settings = settings.Value;
    }

    public void IncrementRequestCount(string endpoint)
    {
        _ = Task.Run(async () => await IncrementMetric("request_count"));
        _logger.LogDebug("Request count incremented. Endpoint: {Endpoint}", endpoint);
    }

    public void IncrementErrorCount(string endpoint, string errorType)
    {
        _ = Task.Run(async () => await IncrementMetric("error_count"));
        _logger.LogWarning("Error count incremented. Error type: {ErrorType}. Endpoint: {Endpoint}", errorType, endpoint);
    }

    public void RecordResponseTime(string endpoint, TimeSpan duration)
    {
        _ = Task.Run(async () => await AddMetric("response_time", duration.TotalMilliseconds));
        _logger.LogDebug("Response time recorded. Duration: {Duration}ms. Endpoint: {Endpoint}", duration.TotalMilliseconds, endpoint);
    }

    public void IncrementBlockchainDataFetched(string blockchainName)
    {
        _ = Task.Run(async () => await IncrementMetric("blockchain_data_fetched"));
        _logger.LogInformation("Blockchain data fetched for: {BlockchainName}", blockchainName);
    }

    public void IncrementCacheHit(string cacheKey)
    {
        _ = Task.Run(async () => await IncrementMetric("cache_hit"));
        _logger.LogDebug("Cache hit. Key: {CacheKey}", cacheKey);
    }

    public void IncrementCacheMiss(string cacheKey)
    {
        _ = Task.Run(async () => await IncrementMetric("cache_miss"));
        _logger.LogDebug("Cache miss. Key: {CacheKey}", cacheKey);
    }

    public void RecordDatabaseOperation(string operation, TimeSpan duration)
    {
        _ = Task.Run(async () => await AddMetric("database_operation", duration.TotalMilliseconds));
        _logger.LogDebug("Database operation recorded. Operation: {Operation}. Duration: {Duration}ms", operation, duration.TotalMilliseconds);
    }

    public void IncrementRateLimitExceeded(string endpoint)
    {
        _ = Task.Run(async () => await IncrementMetric("rate_limit_exceeded"));
        _logger.LogWarning("Rate limit exceeded. Endpoint: {Endpoint}", endpoint);
    }

    public async Task<Dictionary<string, object>> GetMetrics()
    {
        try
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{_keyPrefix}*");
            
            var metrics = new Dictionary<string, object>();
            
            foreach (var key in keys)
            {
                var metricName = key.ToString().Replace(_keyPrefix, "");
                var keyType = await db.KeyTypeAsync(key);
                
                if (keyType == RedisType.String)
                {
                    // Handle simple counter values
                    var value = await db.StringGetAsync(key);
                    if (value.HasValue)
                    {
                        if (double.TryParse(value, out var numericValue))
                        {
                            metrics[metricName] = numericValue;
                        }
                        else
                        {
                            metrics[metricName] = value.ToString();
                        }
                    }
                }
                else if (keyType == RedisType.List)
                {
                    // Handle list values (for response times and database operations)
                    var listValues = await db.ListRangeAsync(key);
                    if (listValues.Length > 0)
                    {
                        var numericValues = new List<double>();
                        foreach (var listValue in listValues)
                        {
                            if (double.TryParse(listValue, out var numericValue))
                            {
                                numericValues.Add(numericValue);
                            }
                        }
                        
                        if (numericValues.Count > 0)
                        {
                            // Calculate average for list metrics
                            var average = numericValues.Average();
                            metrics[metricName] = average;
                            
                            // Also store the count of measurements
                            metrics[$"{metricName}_count"] = numericValues.Count;
                        }
                    }
                }
            }
            
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metrics from Redis");
            return new Dictionary<string, object>();
        }
    }

    private async Task IncrementMetric(string metricName)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{_keyPrefix}{metricName}";
            
            // Increment the value
            await db.StringIncrementAsync(key);
            
            // Set TTL for the key
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(_settings.RedisTtlSeconds));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing metric: {MetricName}", metricName);
        }
    }

    private async Task AddMetric(string metricName, double value)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{_keyPrefix}{metricName}";
            
            // Add value to the list
            await db.ListRightPushAsync(key, value.ToString());
            
            // Keep only the last N values to prevent unlimited growth
            await db.ListTrimAsync(key, -_settings.MaxListSize, -1);
            
            // Set TTL for the key
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(_settings.RedisTtlSeconds));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding metric: {MetricName}", metricName);
        }
    }
} 