using BlockchainMonitor.Infrastructure.Configuration;
using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BlockchainMonitor.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly CacheSettings _cacheSettings;
    private readonly IMetricsService _metricsService;

    public CacheService(IMemoryCache memoryCache, 
        IOptions<CacheSettings> cacheSettings,
        IMetricsService metricsService)
    {
        _memoryCache = memoryCache;
        _cacheSettings = cacheSettings.Value;
        _metricsService = metricsService;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        var value = _memoryCache.Get<T>(key);
        
        if (value != null)
        {
            _metricsService.IncrementCacheHit(key);
        }
        else
        {
            _metricsService.IncrementCacheMiss(key);
        }
        
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            // Use default cache time from configuration
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheSettings.DefaultDurationMinutes);
        }

        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        
        if (exists)
        {
            _metricsService.IncrementCacheHit(key);
        }
        else
        {
            _metricsService.IncrementCacheMiss(key);
        }
        
        return Task.FromResult(exists);
    }
} 