using BlockchainMonitor.Application.DTOs;

namespace BlockchainMonitor.Application.Interfaces;

public interface IBlockCypherService
{
    Task<BlockchainDataDto?> FetchEthereumDataAsync();
    Task<BlockchainDataDto?> FetchDashDataAsync();
    Task<BlockchainDataDto?> FetchBitcoinDataAsync();
    Task<BlockchainDataDto?> FetchBitcoinTestDataAsync();
    Task<BlockchainDataDto?> FetchLitecoinDataAsync();
    Task<BlockchainDataDto?> FetchBlockchainDataAsync(string blockchainName);
} 