using BlockchainMonitor.Application.DTOs;

namespace BlockchainMonitor.Application.Interfaces;

public interface IBlockchainService
{
    Task<IEnumerable<BlockchainDataDto>> GetAllBlockchainDataAsync();
    Task<BlockchainDataDto?> GetLatestBlockchainDataAsync(string blockchainName);
    Task<IEnumerable<BlockchainDataDto>> GetBlockchainHistoryAsync(string blockchainName, int limit = 100);
    Task<IEnumerable<BlockchainDataDto>> GetLatestDataAsync();
    Task<BlockchainDataDto> CreateBlockchainDataAsync(BlockchainDataDto dto);
    Task<int> GetTotalRecordsAsync();
    Task InvalidateRelatedCaches(string blockchainName);
} 