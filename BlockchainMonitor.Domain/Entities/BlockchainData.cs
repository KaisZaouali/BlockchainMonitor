using System.ComponentModel.DataAnnotations;

namespace BlockchainMonitor.Domain.Entities;

public class BlockchainData
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty; // e.g., "ETH.main", "BTC.main"
    
    [Required]
    public long Height { get; set; } // Block height
    
    [Required]
    [MaxLength(100)]
    public string Hash { get; set; } = string.Empty; // Latest block hash
    
    [Required]
    public DateTime Time { get; set; } // Block time
    
    [MaxLength(200)]
    public string? LatestUrl { get; set; }
    
    [MaxLength(100)]
    public string? PreviousHash { get; set; }
    
    [MaxLength(200)]
    public string? PreviousUrl { get; set; }
    
    public int PeerCount { get; set; }
    
    public int UnconfirmedCount { get; set; }
    
    // Fee-related fields (different for each blockchain)
    public long? HighFeePerKb { get; set; } // For BTC, DASH, LTC
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
    
    [MaxLength(100)]
    public string? LastForkHash { get; set; }
    
    // Historical timestamp - when this data was recorded
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public BlockchainData()
    {
        CreatedAt = DateTime.UtcNow;
    }
    
    public BlockchainData(string name, long height, string hash, DateTime time)
        : this()
    {
        Name = name;
        Height = height;
        Hash = hash;
        Time = time;
    }
} 