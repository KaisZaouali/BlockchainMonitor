namespace BlockchainMonitor.Infrastructure.Interfaces;

public interface IDataFetchingService
{
    Task FetchAndStoreAllBlockchainDataAsync();
    Task FetchAndStoreEthereumDataAsync();
    Task FetchAndStoreDashDataAsync();
    Task FetchAndStoreBitcoinDataAsync();
    Task FetchAndStoreBitcoinTestDataAsync();
    Task FetchAndStoreLitecoinDataAsync();
}
