using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Application.Mappers;
using BlockchainMonitor.Infrastructure.Interfaces;
using BlockchainMonitor.DataFetcher.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockchainMonitor.DataFetcher.Services;

public class DataFetchingService : IDataFetchingService
{
    private readonly IBlockCypherService _blockCypherService;
    private readonly IBlockchainService _blockchainService;
    private readonly ILogger<DataFetchingService> _logger;
    private readonly DataFetchingSettings _settings;

    public DataFetchingService(
        IBlockCypherService blockCypherService,
        IBlockchainService blockchainService,
        ILogger<DataFetchingService> logger,
        IOptions<DataFetchingSettings> settings)
    {
        _blockCypherService = blockCypherService;
        _blockchainService = blockchainService;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task FetchAndStoreAllBlockchainDataAsync()
    {
        _logger.LogInformation("Starting to fetch and store all blockchain data");

        // Execute all blockchain operations sequentially
        await FetchAndStoreEthereumDataAsync();
        await Task.Delay(_settings.RequestDelayMs);
        await FetchAndStoreDashDataAsync();
        await Task.Delay(_settings.RequestDelayMs);
        await FetchAndStoreBitcoinDataAsync();
        await Task.Delay(_settings.RequestDelayMs);
        await FetchAndStoreBitcoinTestDataAsync();
        await Task.Delay(_settings.RequestDelayMs);
        await FetchAndStoreLitecoinDataAsync();

        _logger.LogInformation("Completed fetching and storing all blockchain data");
    }

    public async Task FetchAndStoreEthereumDataAsync()
    {
        try
        {
            var ethereumData = await _blockCypherService.FetchEthereumDataAsync();
            if (ethereumData != null)
            {
                var dto = BlockchainMapper.MapToDto(ethereumData);
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
                var dto = BlockchainMapper.MapToDto(dashData);
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
                var dto = BlockchainMapper.MapToDto(bitcoinData);
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
                var dto = BlockchainMapper.MapToDto(bitcoinTestData);
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
                var dto = BlockchainMapper.MapToDto(litecoinData);
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