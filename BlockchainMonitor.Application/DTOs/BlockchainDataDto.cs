using System.ComponentModel.DataAnnotations;

namespace BlockchainMonitor.Application.DTOs;

public class BlockchainDataDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty; // e.g., "ETH.main", "BTC.main"
    
    [Range(0, long.MaxValue)]
    public long Height { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Hash { get; set; } = string.Empty;
    
    public DateTime Time { get; set; }
    
    [StringLength(200)]
    public string? LatestUrl { get; set; }
    
    [StringLength(100)]
    public string? PreviousHash { get; set; }
    
    [StringLength(200)]
    public string? PreviousUrl { get; set; }
    
    [Range(0, int.MaxValue)]
    public int PeerCount { get; set; }
    
    [Range(0, int.MaxValue)]
    public int UnconfirmedCount { get; set; }
    
    // Fee-related fields
    [Range(0, long.MaxValue)]
    public long? HighFeePerKb { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? MediumFeePerKb { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? LowFeePerKb { get; set; }
    
    // Gas-related fields (for Ethereum)
    [Range(0, long.MaxValue)]
    public long? HighGasPrice { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? MediumGasPrice { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? LowGasPrice { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? HighPriorityFee { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? MediumPriorityFee { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? LowPriorityFee { get; set; }
    
    [Range(0, long.MaxValue)]
    public long? BaseFee { get; set; }
    
    // Fork information
    [Range(0, long.MaxValue)]
    public long? LastForkHeight { get; set; }
    
    [StringLength(100)]
    public string? LastForkHash { get; set; }
    
    // Historical timestamp
    public DateTime CreatedAt { get; set; }
} 