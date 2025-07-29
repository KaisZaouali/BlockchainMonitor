using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [JsonPropertyName("latest_url")]
    public string? LatestUrl { get; set; }
    
    [MaxLength(100)]
    [JsonPropertyName("previous_hash")]
    public string? PreviousHash { get; set; }
    
    [MaxLength(200)]
    [JsonPropertyName("previous_url")]
    public string? PreviousUrl { get; set; }
    
    [JsonPropertyName("peer_count")]
    public int PeerCount { get; set; }
    
    [JsonPropertyName("unconfirmed_count")]
    public int UnconfirmedCount { get; set; }
    
    // Fee-related fields (different for each blockchain)
    [JsonPropertyName("high_fee_per_kb")]
    public long? HighFeePerKb { get; set; } // For BTC, DASH, LTC
    [JsonPropertyName("medium_fee_per_kb")]
    public long? MediumFeePerKb { get; set; }
    [JsonPropertyName("low_fee_per_kb")]
    public long? LowFeePerKb { get; set; }
    
    // Gas-related fields (for Ethereum)
    [JsonPropertyName("high_gas_price")]
    public long? HighGasPrice { get; set; }
    [JsonPropertyName("medium_gas_price")]
    public long? MediumGasPrice { get; set; }
    [JsonPropertyName("low_gas_price")]
    public long? LowGasPrice { get; set; }
    [JsonPropertyName("high_priority_fee")]
    public long? HighPriorityFee { get; set; }
    [JsonPropertyName("medium_priority_fee")]
    public long? MediumPriorityFee { get; set; }
    [JsonPropertyName("low_priority_fee")]
    public long? LowPriorityFee { get; set; }
    [JsonPropertyName("base_fee")]
    public long? BaseFee { get; set; }
    
    // Fork information
    [JsonPropertyName("last_fork_height")]
    public long? LastForkHeight { get; set; }
    
    [MaxLength(100)]
    [JsonPropertyName("last_fork_hash")]
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