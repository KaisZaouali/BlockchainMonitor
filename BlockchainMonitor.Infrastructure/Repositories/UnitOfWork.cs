using BlockchainMonitor.Domain.Interfaces;
using BlockchainMonitor.Infrastructure.Data;
using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlockchainMonitor.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BlockchainDbContext _context;
    private readonly IMetricsService _metricsService;
    private IBlockchainRepository? _blockchainRepository;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(BlockchainDbContext context, IMetricsService metricsService)
    {
        _context = context;
        _metricsService = metricsService;
    }

    public IBlockchainRepository BlockchainRepository
    {
        get
        {
            _blockchainRepository ??= new BlockchainRepository(_context, _metricsService);
            return _blockchainRepository;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
} 