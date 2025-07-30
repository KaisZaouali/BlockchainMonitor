namespace BlockchainMonitor.Application.Constants;

public static class BlockchainConstants
{
    /// <summary>
    /// Maximum number of history records that can be requested
    /// </summary>
    public const int MaxHistoryLimit = 1000;
    
    /// <summary>
    /// Default number of history records when limit is not specified
    /// </summary>
    public const int DefaultHistoryLimit = 100;
} 