using BlockchainMonitor.Application.DTOs;

namespace BlockchainMonitor.Application.Interfaces;

public interface IBlockchainService
{
    Task<BlockchainDataDto?> GetLatestBlockchainDataAsync(string blockchainName);
    Task<IEnumerable<BlockchainDataDto>> GetAllBlockchainDataAsync();
    Task<BlockchainDataDto> CreateBlockchainDataAsync(BlockchainDataDto dto);
    Task<IEnumerable<BlockchainDataDto>> GetBlockchainHistoryAsync(string blockchainName, int limit = 100);
    Task<IEnumerable<BlockchainDataDto>> GetLatestDataAsync();
    Task<int> GetTotalRecordsAsync();
} 