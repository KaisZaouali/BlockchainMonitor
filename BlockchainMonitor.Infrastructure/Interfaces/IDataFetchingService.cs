namespace BlockchainMonitor.Infrastructure.Interfaces;

public interface IDataFetchingService
{
    Task FetchAndStoreAllBlockchainDataAsync();
    Task FetchAndStoreBlockchainDataAsync(string blockchainName);
    Task FetchAndStoreEthereumDataAsync();
    Task FetchAndStoreDashDataAsync();
    Task FetchAndStoreBitcoinDataAsync();
    Task FetchAndStoreBitcoinTestDataAsync();
    Task FetchAndStoreLitecoinDataAsync();
} 