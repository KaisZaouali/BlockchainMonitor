namespace BlockchainMonitor.DataFetcher.Configuration;

public class DataFetchingSettings
{
    public const string SectionName = "DataFetching";
    
    /// <summary>
    /// Delay between requests in milliseconds to avoid rate limiting
    /// </summary>
    public int RequestDelayMs { get; set; } = 1000;
} 