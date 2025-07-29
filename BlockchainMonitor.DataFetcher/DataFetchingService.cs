using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlockchainMonitor.DataFetcher.Services;

public class DataFetchingService : IDataFetchingService
{
    private readonly IBlockCypherService _blockCypherService;
    private readonly IBlockchainService _blockchainService;
    private readonly ILogger<DataFetchingService> _logger;

    public DataFetchingService(
        IBlockCypherService blockCypherService,
        IBlockchainService blockchainService,
        ILogger<DataFetchingService> logger)
    {
        _blockCypherService = blockCypherService;
        _blockchainService = blockchainService;
        _logger = logger;
    }

    public async Task FetchAndStoreAllBlockchainDataAsync()
    {
        _logger.LogInformation("Starting to fetch and store all blockchain data");

        const int delay = 1000;

        // Run operations sequentially to avoid DbContext concurrency issues
        await FetchAndStoreEthereumDataAsync();
        await Task.Delay(delay); // Increase delay to 5 seconds to avoid rate limiting
        
        await FetchAndStoreDashDataAsync();
        await Task.Delay(delay);
        
        await FetchAndStoreBitcoinDataAsync();
        await Task.Delay(delay);
        
        await FetchAndStoreBitcoinTestDataAsync();
        await Task.Delay(delay);
        
        await FetchAndStoreLitecoinDataAsync();

        _logger.LogInformation("Completed fetching and storing all blockchain data");
    }

    public async Task FetchAndStoreBlockchainDataAsync(string blockchainName)
    {
        try
        {
            _logger.LogInformation("Fetching and storing data for blockchain: {BlockchainName}", blockchainName);

            var blockchainData = await _blockCypherService.FetchBlockchainDataAsync(blockchainName);
            
            if (blockchainData != null)
            {
                var dto = MapToDto(blockchainData);
                await _blockchainService.CreateBlockchainDataAsync(dto);
                _logger.LogInformation("Successfully stored data for blockchain: {BlockchainName}", blockchainName);
            }
            else
            {
                _logger.LogWarning("Failed to fetch data for blockchain: {BlockchainName}", blockchainName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and storing data for blockchain: {BlockchainName}", blockchainName);
        }
    }

    private static BlockchainDataDto MapToDto(BlockchainData entity)
    {
        return new BlockchainDataDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Height = entity.Height,
            Hash = entity.Hash,
            Time = entity.Time,
            LatestUrl = entity.LatestUrl,
            PreviousHash = entity.PreviousHash,
            PreviousUrl = entity.PreviousUrl,
            PeerCount = entity.PeerCount,
            UnconfirmedCount = entity.UnconfirmedCount,
            HighFeePerKb = entity.HighFeePerKb,
            MediumFeePerKb = entity.MediumFeePerKb,
            LowFeePerKb = entity.LowFeePerKb,
            HighGasPrice = entity.HighGasPrice,
            MediumGasPrice = entity.MediumGasPrice,
            LowGasPrice = entity.LowGasPrice,
            HighPriorityFee = entity.HighPriorityFee,
            MediumPriorityFee = entity.MediumPriorityFee,
            LowPriorityFee = entity.LowPriorityFee,
            BaseFee = entity.BaseFee,
            LastForkHeight = entity.LastForkHeight,
            LastForkHash = entity.LastForkHash,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task FetchAndStoreEthereumDataAsync()
    {
        try
        {
            var ethereumData = await _blockCypherService.FetchEthereumDataAsync();
            if (ethereumData != null)
            {
                var dto = MapToDto(ethereumData);
                await _blockchainService.CreateBlockchainDataAsync(dto);
                _logger.LogInformation("Successfully stored Ethereum data: Height {Height}", ethereumData.Height);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and storing Ethereum data");
        }
    }

    public async Task FetchAndStoreDashDataAsync()
    {
        try
        {
            var dashData = await _blockCypherService.FetchDashDataAsync();
            if (dashData != null)
            {
                var dto = MapToDto(dashData);
                await _blockchainService.CreateBlockchainDataAsync(dto);
                _logger.LogInformation("Successfully stored Dash data: Height {Height}", dashData.Height);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and storing Dash data");
        }
    }

    public async Task FetchAndStoreBitcoinDataAsync()
    {
        try
        {
            var bitcoinData = await _blockCypherService.FetchBitcoinDataAsync();
            if (bitcoinData != null)
            {
                var dto = MapToDto(bitcoinData);
                await _blockchainService.CreateBlockchainDataAsync(dto);
                _logger.LogInformation("Successfully stored Bitcoin data: Height {Height}", bitcoinData.Height);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and storing Bitcoin data");
        }
    }

    public async Task FetchAndStoreBitcoinTestDataAsync()
    {
        try
        {
            var bitcoinTestData = await _blockCypherService.FetchBitcoinTestDataAsync();
            if (bitcoinTestData != null)
            {
                var dto = MapToDto(bitcoinTestData);
                await _blockchainService.CreateBlockchainDataAsync(dto);
                _logger.LogInformation("Successfully stored Bitcoin Test data: Height {Height}", bitcoinTestData.Height);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and storing Bitcoin Test data");
        }
    }

    public async Task FetchAndStoreLitecoinDataAsync()
    {
        try
        {
            var litecoinData = await _blockCypherService.FetchLitecoinDataAsync();
            if (litecoinData != null)
            {
                var dto = MapToDto(litecoinData);
                await _blockchainService.CreateBlockchainDataAsync(dto);
                _logger.LogInformation("Successfully stored Litecoin data: Height {Height}", litecoinData.Height);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and storing Litecoin data");
        }
    }
} 