namespace BlockchainMonitor.Infrastructure.Interfaces;

public interface IMetricsService
{
    void IncrementRequestCount(string endpoint);
    void IncrementErrorCount(string endpoint, string errorType);
    void RecordResponseTime(string endpoint, TimeSpan duration);
    void IncrementBlockchainDataFetched(string blockchainName);
    void IncrementCacheHit(string cacheKey);
    void IncrementCacheMiss(string cacheKey);
    void RecordDatabaseOperation(string operation, TimeSpan duration);
    void IncrementRateLimitExceeded(string endpoint);
    Task<Dictionary<string, object>> GetMetrics();
} 