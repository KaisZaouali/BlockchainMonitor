namespace BlockchainMonitor.Infrastructure.Configuration;

public class CacheSettings
{
    public const string SectionName = "CacheSettings";

    /// <summary>
    /// Default cache duration in minutes
    /// </summary>
    public int DefaultDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Cache duration for all blockchain data in minutes
    /// </summary>
    public int AllBlockchainDataDurationMinutes { get; set; } = 2;

    /// <summary>
    /// Cache duration for latest blockchain data in minutes
    /// </summary>
    public int LatestBlockchainDataDurationMinutes { get; set; } = 1;

    /// <summary>
    /// Cache duration for blockchain history in minutes
    /// </summary>
    public int BlockchainHistoryDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Cache duration for latest data from all blockchains in minutes
    /// </summary>
    public int LatestDataDurationMinutes { get; set; } = 2;

    /// <summary>
    /// Cache duration for total records count in minutes
    /// </summary>
    public int TotalRecordsDurationMinutes { get; set; } = 10;
}
