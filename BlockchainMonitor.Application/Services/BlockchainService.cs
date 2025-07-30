using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Application.Exceptions;
using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Application.Constants;
using BlockchainMonitor.Application.Configuration;
using BlockchainMonitor.Application.Mappers;
using BlockchainMonitor.Domain.Events;
using BlockchainMonitor.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockchainMonitor.Application.Services;

public class BlockchainService : IBlockchainService
{
    private readonly IBlockchainRepository _blockchainRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BlockchainService> _logger;
    private readonly CacheSettings _cacheSettings;

    public BlockchainService(
        IBlockchainRepository blockchainRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IEventPublisher eventPublisher,
        ILogger<BlockchainService> logger,
        IOptions<CacheSettings> cacheSettings)
    {
        _blockchainRepository = blockchainRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<IEnumerable<BlockchainDataDto>> GetAllBlockchainDataAsync()
    {
        var cacheKey = GetAllBlockchainDataCacheKey();
        
        // Try to get from cache first
        var cachedData = await _cacheService.GetAsync<IEnumerable<BlockchainDataDto>>(cacheKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Cached data found for {CacheKey}", cacheKey);
            return cachedData;
        }

        // If not in cache, get from database
        var entities = await _blockchainRepository.GetAllAsync();
        var dtos = entities.Select(BlockchainMapper.MapToDto).ToList();

        // Cache the result using configuration
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(_cacheSettings.AllBlockchainDataDurationMinutes));

        return dtos;
    }

    public async Task<BlockchainDataDto?> GetLatestBlockchainDataAsync(string blockchainName)
    {
        // we could use history records cache key here, but it's not necessary in case users are spamming the latest blockchain data endpoint
        var cacheKey = GetLatestBlockchainDataCacheKey(blockchainName);
        
        // Try to get from cache first
        var cachedData = await _cacheService.GetAsync<BlockchainDataDto>(cacheKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Cached data found for {CacheKey}", cacheKey);
            return cachedData;
        }

        // If not in cache, get from database
        var entity = await _blockchainRepository.GetLatestByNameAsync(blockchainName);
        if (entity == null)
        {
            return null;
        }

        var dto = BlockchainMapper.MapToDto(entity);

        // Cache the result using configuration
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(_cacheSettings.LatestBlockchainDataDurationMinutes));

        return dto;
    }

    public async Task<IEnumerable<BlockchainDataDto>> GetBlockchainHistoryAsync(string blockchainName,
        int limit = BlockchainConstants.DefaultHistoryLimit)
    {
        // Always cache with max limit, but return only requested limit
        var cacheKey = GetBlockchainHistoryCacheKey(blockchainName);
        
        // Try to get from cache first
        var cachedData = await _cacheService.GetAsync<IEnumerable<BlockchainDataDto>>(cacheKey);
        if (cachedData != null)
        {   
            _logger.LogInformation("Cached data found for {CacheKey}", cacheKey);
            // Return only the requested limit from cached data
            return cachedData.Take(limit);
        }

        // If not in cache, get from database with max limit
        var entities = await _blockchainRepository.GetHistoryByNameAsync(blockchainName, BlockchainConstants.MaxHistoryLimit);
        var dtos = entities.Select(BlockchainMapper.MapToDto).ToList();

        // Cache the full result using configuration
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(_cacheSettings.BlockchainHistoryDurationMinutes));

        // Return only the requested limit
        return dtos.Take(limit);
    }

    public async Task<IEnumerable<BlockchainDataDto>> GetLatestDataAsync()
    {
        // we could use total records cache key here, but it's not necessary in case users are spamming the latest data endpoint
        var cacheKey = GetLatestDataCacheKey();
        
        // Try to get from cache first
        var cachedData = await _cacheService.GetAsync<IEnumerable<BlockchainDataDto>>(cacheKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Cached data found for {CacheKey}", cacheKey);
            return cachedData;
        }

        // If not in cache, get from database
        var entities = await _blockchainRepository.GetLatestDataAsync();
        var dtos = entities.Select(BlockchainMapper.MapToDto).ToList();

        // Cache the result using configuration
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(_cacheSettings.LatestDataDurationMinutes));

        return dtos;
    }

    public async Task<BlockchainDataDto> CreateBlockchainDataAsync(BlockchainDataDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new InvalidBlockchainDataException("Blockchain name is required");
        }

        if (dto.Height <= 0)
        {
            throw new InvalidBlockchainDataException("Blockchain height must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(dto.Hash))
        {
            throw new InvalidBlockchainDataException("Blockchain hash is required");
        }

        var entity = BlockchainMapper.MapToEntity(dto);
        entity.CreatedAt = DateTime.UtcNow;

        await _blockchainRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // Publish domain event for cache invalidation
        var @event = new BlockchainDataCreatedEvent(dto.Name);
        _eventPublisher.Publish(@event);

        _logger.LogInformation("Created blockchain data and published event for: {BlockchainName}", dto.Name);

        return BlockchainMapper.MapToDto(entity);
    }

    public async Task<int> GetTotalRecordsAsync()
    {
        var cacheKey = GetTotalRecordsCacheKey();
        
        // Try to get from cache first
        var cachedData = await _cacheService.GetAsync<int>(cacheKey);
        if (cachedData != 0)
        {
            return cachedData;
        }

        // If not in cache, get from database
        var total = await _blockchainRepository.GetTotalRecordsAsync();

        // Cache the result using configuration
        await _cacheService.SetAsync(cacheKey, total, TimeSpan.FromMinutes(_cacheSettings.TotalRecordsDurationMinutes));

        return total;
    }

    public async Task InvalidateRelatedCaches(string blockchainName)
    {
        // Remove caches that might be affected by new data
        await _cacheService.RemoveAsync(GetAllBlockchainDataCacheKey());
        await _cacheService.RemoveAsync(GetLatestDataCacheKey());
        await _cacheService.RemoveAsync(GetLatestBlockchainDataCacheKey(blockchainName));
        await _cacheService.RemoveAsync(GetBlockchainHistoryCacheKey(blockchainName));
        await _cacheService.RemoveAsync(GetTotalRecordsCacheKey());

        _logger.LogDebug("Invalidated caches for blockchain: {BlockchainName}", blockchainName);
    }

    // Cache key generation functions
    private static string GetAllBlockchainDataCacheKey() => "all_blockchain_data";
    private static string GetLatestDataCacheKey() => "latest_data_all_blockchains";
    private static string GetLatestBlockchainDataCacheKey(string blockchainName) => $"latest_blockchain_data_{blockchainName}";
    private static string GetBlockchainHistoryCacheKey(string blockchainName) => $"blockchain_history_{blockchainName}";
    private static string GetTotalRecordsCacheKey() => "total_blockchain_records";
} 