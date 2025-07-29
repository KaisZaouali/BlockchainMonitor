using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Application.Exceptions;
using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Domain.Interfaces;

namespace BlockchainMonitor.Application.Services;

public class BlockchainService : IBlockchainService
{
    private readonly IUnitOfWork _unitOfWork;

    public BlockchainService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BlockchainDataDto?> GetLatestBlockchainDataAsync(string blockchainName)
    {
        var data = await _unitOfWork.BlockchainRepository.GetLatestByNameAsync(blockchainName);
        return data != null ? MapToDto(data) : null;
    }

    public async Task<IEnumerable<BlockchainDataDto>> GetAllBlockchainDataAsync()
    {
        var data = await _unitOfWork.BlockchainRepository.GetAllAsync();
        return data.Select(MapToDto);
    }

    public async Task<BlockchainDataDto> CreateBlockchainDataAsync(BlockchainDataDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new InvalidBlockchainDataException("Blockchain name is required");

        if (dto.Height <= 0)
            throw new InvalidBlockchainDataException("Block height must be greater than zero");

        if (string.IsNullOrWhiteSpace(dto.Hash))
            throw new InvalidBlockchainDataException("Block hash is required");

        var entity = new BlockchainData
        {
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
            LastForkHash = dto.LastForkHash
        };

        var result = await _unitOfWork.BlockchainRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return MapToDto(result);
    }

    public async Task<IEnumerable<BlockchainDataDto>> GetBlockchainHistoryAsync(string blockchainName, int limit = 100)
    {
        if (string.IsNullOrWhiteSpace(blockchainName))
            throw new InvalidBlockchainDataException("Blockchain name is required");

        if (limit <= 0 || limit > 1000)
            throw new InvalidBlockchainDataException("Limit must be between 1 and 1000");

        var history = await _unitOfWork.BlockchainRepository.GetHistoryByNameAsync(blockchainName, limit);
        return history.Select(MapToDto);
    }

    public async Task<IEnumerable<BlockchainDataDto>> GetLatestDataAsync()
    {
        var data = await _unitOfWork.BlockchainRepository.GetLatestDataAsync();
        return data.Select(MapToDto);
    }

    public async Task<int> GetTotalRecordsAsync()
    {
        return await _unitOfWork.BlockchainRepository.GetTotalRecordsAsync();
    }

    private static BlockchainDataDto MapToDto(BlockchainData entity)
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
} 