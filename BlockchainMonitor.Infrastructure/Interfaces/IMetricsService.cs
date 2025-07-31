namespace BlockchainMonitor.Infrastructure.Interfaces;

/// <summary>
/// Service for collecting and retrieving application metrics.
/// Provides a unified interface for tracking performance, errors, and operational data.
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Increments the global request count across all endpoints.
    /// </summary>
    /// <param name="endpoint">The endpoint that received the request (for logging purposes)</param>
    void IncrementRequestCount(string endpoint);

    /// <summary>
    /// Increments the global error count across all endpoints.
    /// </summary>
    /// <param name="endpoint">The endpoint where the error occurred</param>
    /// <param name="errorType">The type of error that occurred</param>
    void IncrementErrorCount(string endpoint, string errorType);

    /// <summary>
    /// Records response time for performance monitoring.
    /// </summary>
    /// <param name="endpoint">The endpoint that was called</param>
    /// <param name="duration">The time taken to process the request</param>
    void RecordResponseTime(string endpoint, TimeSpan duration);

    /// <summary>
    /// Increments the count of blockchain data fetch operations.
    /// </summary>
    /// <param name="blockchainName">The name of the blockchain (e.g., "BTC.main")</param>
    void IncrementBlockchainDataFetched(string blockchainName);

    /// <summary>
    /// Increments the cache hit count for performance monitoring.
    /// </summary>
    /// <param name="cacheKey">The cache key that was hit</param>
    void IncrementCacheHit(string cacheKey);

    /// <summary>
    /// Increments the cache miss count for performance monitoring.
    /// </summary>
    /// <param name="cacheKey">The cache key that was missed</param>
    void IncrementCacheMiss(string cacheKey);

    /// <summary>
    /// Records database operation time for performance monitoring.
    /// </summary>
    /// <param name="operation">The name of the database operation</param>
    /// <param name="duration">The time taken to complete the operation</param>
    void RecordDatabaseOperation(string operation, TimeSpan duration);

    /// <summary>
    /// Increments the count of rate limit exceeded events.
    /// </summary>
    /// <param name="endpoint">The endpoint where rate limit was exceeded</param>
    void IncrementRateLimitExceeded(string endpoint);

    /// <summary>
    /// Retrieves all collected metrics as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all metric values</returns>
    Task<Dictionary<string, object>> GetMetrics();
}
