using BlockchainMonitor.Domain.Entities;

namespace BlockchainMonitor.Domain.Interfaces;

public interface IBlockchainRepository : IRepository<BlockchainData>
{
    Task<BlockchainData?> GetLatestByNameAsync(string name);
    Task<IEnumerable<BlockchainData>> GetHistoryByNameAsync(string name, int limit = 100);
    Task<IEnumerable<BlockchainData>> GetLatestDataAsync();
    Task<IEnumerable<BlockchainData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<int> GetTotalRecordsAsync();
} 