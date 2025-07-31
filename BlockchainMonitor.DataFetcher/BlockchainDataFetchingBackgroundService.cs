using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BlockchainMonitor.DataFetcher.Configuration;

namespace BlockchainMonitor.DataFetcher.Services;

/// <summary>
/// Background service for continuously fetching blockchain data.
/// Runs on a configurable interval to fetch data from multiple blockchain networks.
/// Integrates with metrics collection and handles errors gracefully.
/// </summary>
public class BlockchainDataFetchingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BlockchainDataFetchingBackgroundService> _logger;
    private readonly DataFetchingSettings _settings;

    public BlockchainDataFetchingBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BlockchainDataFetchingBackgroundService> logger,
        IOptions<DataFetchingSettings> settings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Blockchain data fetching service is disabled");
            return;
        }

        _logger.LogInformation("Blockchain data fetching service started. Interval: {IntervalSeconds} seconds", _settings.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scheduled blockchain data fetch");

                using var scope = _serviceScopeFactory.CreateScope();
                var dataFetchingService = scope.ServiceProvider.GetRequiredService<IDataFetchingService>();

                await dataFetchingService.FetchAndStoreAllBlockchainDataAsync();
                _logger.LogInformation("Completed scheduled blockchain data fetch");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled blockchain data fetch");
            }

            // Wait for the next interval
            await Task.Delay(TimeSpan.FromSeconds(_settings.IntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Blockchain data fetching service stopped");
    }
}
