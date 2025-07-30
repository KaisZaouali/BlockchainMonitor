using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Domain.Entities;

namespace BlockchainMonitor.Application.Mappers;

public static class BlockchainMapper
{
    /// <summary>
    /// Maps a BlockchainData entity to BlockchainDataDto
    /// </summary>
    public static BlockchainDataDto MapToDto(BlockchainData entity)
    {
        return new BlockchainDataDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Height = entity.Height,
            Hash = entity.Hash,
            Time = entity.Time,
            LatestUrl = entity.LatestUrl,
            PreviousHash = entity.PreviousHash,
            PreviousUrl = entity.PreviousUrl,
            PeerCount = entity.PeerCount,
            UnconfirmedCount = entity.UnconfirmedCount,
            HighFeePerKb = entity.HighFeePerKb,
            MediumFeePerKb = entity.MediumFeePerKb,
            LowFeePerKb = entity.LowFeePerKb,
            HighGasPrice = entity.HighGasPrice,
            MediumGasPrice = entity.MediumGasPrice,
            LowGasPrice = entity.LowGasPrice,
            HighPriorityFee = entity.HighPriorityFee,
            MediumPriorityFee = entity.MediumPriorityFee,
            LowPriorityFee = entity.LowPriorityFee,
            BaseFee = entity.BaseFee,
            LastForkHeight = entity.LastForkHeight,
            LastForkHash = entity.LastForkHash,
            CreatedAt = entity.CreatedAt
        };
    }

    /// <summary>
    /// Maps a BlockchainDataDto to BlockchainData entity
    /// </summary>
    public static BlockchainData MapToEntity(BlockchainDataDto dto)
    {
        return new BlockchainData
        {
            Id = dto.Id,
            Name = dto.Name,
            Height = dto.Height,
            Hash = dto.Hash,
            Time = dto.Time,
            LatestUrl = dto.LatestUrl,
            PreviousHash = dto.PreviousHash,
            PreviousUrl = dto.PreviousUrl,
            PeerCount = dto.PeerCount,
            UnconfirmedCount = dto.UnconfirmedCount,
            HighFeePerKb = dto.HighFeePerKb,
            MediumFeePerKb = dto.MediumFeePerKb,
            LowFeePerKb = dto.LowFeePerKb,
            HighGasPrice = dto.HighGasPrice,
            MediumGasPrice = dto.MediumGasPrice,
            LowGasPrice = dto.LowGasPrice,
            HighPriorityFee = dto.HighPriorityFee,
            MediumPriorityFee = dto.MediumPriorityFee,
            LowPriorityFee = dto.LowPriorityFee,
            BaseFee = dto.BaseFee,
            LastForkHeight = dto.LastForkHeight,
            LastForkHash = dto.LastForkHash,
            CreatedAt = dto.CreatedAt
        };
    }
} 