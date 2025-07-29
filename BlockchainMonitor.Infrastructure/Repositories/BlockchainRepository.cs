using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Domain.Interfaces;
using BlockchainMonitor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlockchainMonitor.Infrastructure.Repositories;

public class BlockchainRepository : Repository<BlockchainData>, IBlockchainRepository
{
    public BlockchainRepository(BlockchainDbContext context) : base(context)
    {
    }

    public async Task<BlockchainData?> GetLatestByNameAsync(string name)
    {
        return await _dbSet
            .Where(x => x.Name == name)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BlockchainData>> GetHistoryByNameAsync(string name, int limit = 100)
    {
        return await _dbSet
            .Where(x => x.Name == name)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlockchainData>> GetLatestDataAsync()
    {
        // Get the latest record for each blockchain
        var latestRecords = await _dbSet
            .GroupBy(x => x.Name)
            .Select(g => g.OrderByDescending(x => x.CreatedAt).First())
            .ToListAsync();
        
        return latestRecords;
    }

    public async Task<IEnumerable<BlockchainData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalRecordsAsync()
    {
        return await _dbSet.CountAsync();
    }
} 