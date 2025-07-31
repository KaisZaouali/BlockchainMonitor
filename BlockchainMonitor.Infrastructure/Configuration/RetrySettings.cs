namespace BlockchainMonitor.Infrastructure.Configuration;

public class RetrySettings
{
    public const string SectionName = "RetrySettings";

    /// <summary>
    /// Maximum retry attempts for failed requests
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Retry delay in milliseconds between attempts
    /// </summary>
    public int RetryDelayMs { get; set; } = 2000;
}
