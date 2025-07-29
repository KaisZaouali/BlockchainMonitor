namespace BlockchainMonitor.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBlockchainRepository BlockchainRepository { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
} 