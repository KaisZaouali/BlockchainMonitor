using System.Text.Json;
using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Infrastructure.Interfaces;
using BlockchainMonitor.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockchainMonitor.Infrastructure.Services;

public class BlockCypherService : IBlockCypherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlockCypherService> _logger;
    private readonly RetrySettings _retrySettings;

    public BlockCypherService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<BlockCypherService> logger,
        IOptions<RetrySettings> retrySettings)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _retrySettings = retrySettings.Value;
    }

    public async Task<BlockchainData?> FetchEthereumDataAsync()
    {
        return await FetchBlockchainDataAsync("eth/main");
    }

    public async Task<BlockchainData?> FetchDashDataAsync()
    {
        return await FetchBlockchainDataAsync("dash/main");
    }

    public async Task<BlockchainData?> FetchBitcoinDataAsync()
    {
        return await FetchBlockchainDataAsync("btc/main");
    }

    public async Task<BlockchainData?> FetchBitcoinTestDataAsync()
    {
        return await FetchBlockchainDataAsync("btc/test3");
    }

    public async Task<BlockchainData?> FetchLitecoinDataAsync()
    {
        return await FetchBlockchainDataAsync("ltc/main");
    }

    public async Task<BlockchainData?> FetchBlockchainDataAsync(string blockchainName)
    {
        for (int attempt = 1; attempt <= _retrySettings.MaxRetryAttempts; attempt++)
        {
            try
            {
                var baseUrl = _configuration["BlockCypherApi:BaseUrl"];
                var url = $"{baseUrl}/{blockchainName}";

                _logger.LogInformation("Base URL from config: {BaseUrl}", baseUrl);
                _logger.LogInformation("Full URL: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var delayMs = _retrySettings.RetryDelayMs * (int)Math.Pow(2, attempt - 1); // Exponential backoff
                    _logger.LogWarning("Rate limited for blockchain {BlockchainName}. Attempt {Attempt}/{MaxRetries}. Waiting {DelayMs}ms", 
                        blockchainName, attempt, _retrySettings.MaxRetryAttempts, delayMs);
                    
                    if (attempt < _retrySettings.MaxRetryAttempts)
                    {
                        await Task.Delay(delayMs);
                        continue;
                    }
                }
                
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var blockchainData = JsonSerializer.Deserialize<BlockchainData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (blockchainData != null)
                {
                    blockchainData.CreatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Successfully fetched data for {BlockchainName}: Height {Height}", 
                        blockchainData.Name, blockchainData.Height);
                }

                return blockchainData;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var delayMs = _retrySettings.RetryDelayMs * (int)Math.Pow(2, attempt - 1);
                _logger.LogWarning("Rate limited for blockchain {BlockchainName}. Attempt {Attempt}/{MaxRetries}. Waiting {DelayMs}ms", 
                    blockchainName, attempt, _retrySettings.MaxRetryAttempts, delayMs);
                
                if (attempt < _retrySettings.MaxRetryAttempts)
                {
                    await Task.Delay(delayMs);
                    continue;
                }
                
                _logger.LogError(ex, "HTTP request failed after {MaxRetries} attempts for blockchain {BlockchainName}", _retrySettings.MaxRetryAttempts, blockchainName);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for blockchain {BlockchainName}", blockchainName);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed for blockchain {BlockchainName}", blockchainName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching data for blockchain {BlockchainName}", blockchainName);
                return null;
            }
        }

        return null;
    }
} 