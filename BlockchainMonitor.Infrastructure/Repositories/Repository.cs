using BlockchainMonitor.Domain.Interfaces;
using BlockchainMonitor.Infrastructure.Data;
using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlockchainMonitor.Infrastructure.Repositories;

/// <summary>
/// Base repository class that provides common database operations with metrics tracking.
/// Automatically measures and records database operation performance for monitoring.
/// </summary>
/// <typeparam name="T">The entity type for this repository</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly BlockchainDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly IMetricsService _metricsService;

    public Repository(BlockchainDbContext context, IMetricsService metricsService)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _metricsService = metricsService;
    }

    /// <summary>
    /// Executes a database operation with metrics tracking.
    /// Records the operation time and any errors that occur.
    /// </summary>
    /// <typeparam name="TResult">The return type of the operation</typeparam>
    /// <param name="operation">The database operation to execute</param>
    /// <param name="operationName">The name of the operation for metrics</param>
    /// <returns>The result of the database operation</returns>
    protected async Task<TResult> ExecuteWithMetricsAsync<TResult>(Func<Task<TResult>> operation, string operationName)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await operation();
            stopwatch.Stop();
            _metricsService.RecordDatabaseOperation(operationName, stopwatch.Elapsed);
            return result;
        }
        catch
        {
            stopwatch.Stop();
            _metricsService.RecordDatabaseOperation(operationName, stopwatch.Elapsed);
            throw;
        }
    }

    /// <summary>
    /// Executes a void database operation with metrics tracking.
    /// Records the operation time and any errors that occur.
    /// </summary>
    /// <param name="operation">The database operation to execute</param>
    /// <param name="operationName">The name of the operation for metrics</param>
    protected async Task ExecuteWithMetricsAsync(Func<Task> operation, string operationName)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await operation();
            stopwatch.Stop();
            _metricsService.RecordDatabaseOperation(operationName, stopwatch.Elapsed);
        }
        catch
        {
            stopwatch.Stop();
            _metricsService.RecordDatabaseOperation(operationName, stopwatch.Elapsed);
            throw;
        }
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await ExecuteWithMetricsAsync(async () =>
        {
            return await _dbSet.FindAsync(id);
        }, "GetById");
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await ExecuteWithMetricsAsync(async () =>
        {
            return await _dbSet.ToListAsync();
        }, "GetAll");
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        return await ExecuteWithMetricsAsync(async () =>
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }, "Add");
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await ExecuteWithMetricsAsync(async () =>
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }, "Update");
    }

    public virtual async Task DeleteAsync(T entity)
    {
        await ExecuteWithMetricsAsync(async () =>
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }, "Delete");
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await ExecuteWithMetricsAsync(async () =>
        {
            return await _dbSet.FindAsync(id) != null;
        }, "Exists");
    }

    public virtual async Task<int> CountAsync()
    {
        return await ExecuteWithMetricsAsync(async () =>
        {
            return await _dbSet.CountAsync();
        }, "Count");
    }
}
