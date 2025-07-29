namespace BlockchainMonitor.Application.DTOs;

public class BlockchainDataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "ETH.main", "BTC.main"
    public long Height { get; set; }
    public string Hash { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string? LatestUrl { get; set; }
    public string? PreviousHash { get; set; }
    public string? PreviousUrl { get; set; }
    public int PeerCount { get; set; }
    public int UnconfirmedCount { get; set; }
    
    // Fee-related fields
    public long? HighFeePerKb { get; set; }
    public long? MediumFeePerKb { get; set; }
    public long? LowFeePerKb { get; set; }
    
    // Gas-related fields (for Ethereum)
    public long? HighGasPrice { get; set; }
    public long? MediumGasPrice { get; set; }
    public long? LowGasPrice { get; set; }
    public long? HighPriorityFee { get; set; }
    public long? MediumPriorityFee { get; set; }
    public long? LowPriorityFee { get; set; }
    public long? BaseFee { get; set; }
    
    // Fork information
    public long? LastForkHeight { get; set; }
    public string? LastForkHash { get; set; }
    
    // Historical timestamp
    public DateTime CreatedAt { get; set; }
} 