using BlockchainMonitor.Domain.Entities;

namespace BlockchainMonitor.Infrastructure.Interfaces;

public interface IBlockCypherService
{
    Task<BlockchainData?> FetchEthereumDataAsync();
    Task<BlockchainData?> FetchDashDataAsync();
    Task<BlockchainData?> FetchBitcoinDataAsync();
    Task<BlockchainData?> FetchBitcoinTestDataAsync();
    Task<BlockchainData?> FetchLitecoinDataAsync();
    Task<BlockchainData?> FetchBlockchainDataAsync(string blockchainName);
}
