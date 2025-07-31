namespace BlockchainMonitor.Infrastructure.Configuration;

public class MetricsSettings
{
    public const string SectionName = "Metrics";
    
    public int RedisTtlSeconds { get; set; } = 300; // 5 minutes default
    public int MaxListSize { get; set; } = 100; // Maximum items in lists
} 