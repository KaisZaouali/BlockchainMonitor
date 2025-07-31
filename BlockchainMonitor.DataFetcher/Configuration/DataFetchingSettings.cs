namespace BlockchainMonitor.DataFetcher.Configuration;

public class DataFetchingSettings
{
    public const string SectionName = "DataFetching";
    
    /// <summary>
    /// Delay between requests in milliseconds to avoid rate limiting
    /// </summary>
    public int RequestDelayMs { get; set; } = 1000;

    /// <summary>
    /// Whether the data fetching service is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Interval in seconds between data fetching operations
    /// </summary>
    public int IntervalSeconds { get; set; } = 600;
} 