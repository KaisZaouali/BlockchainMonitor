using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockchainMonitor.DataFetcher.Services;

public class BlockchainDataFetchingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BlockchainDataFetchingBackgroundService> _logger;
    private readonly BlockchainDataFetchingOptions _options;

    public BlockchainDataFetchingBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BlockchainDataFetchingBackgroundService> logger,
        IOptions<BlockchainDataFetchingOptions> options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Blockchain data fetching service is disabled");
            return;
        }

        _logger.LogInformation("Blockchain data fetching service started. Interval: {IntervalSeconds} seconds", _options.IntervalSeconds);

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
            await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Blockchain data fetching service stopped");
    }
}

public class BlockchainDataFetchingOptions
{
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 30;
} 