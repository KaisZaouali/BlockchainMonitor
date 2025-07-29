using System.Text.Json;
using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlockchainMonitor.Infrastructure.Services;

public class BlockCypherService : IBlockCypherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlockCypherService> _logger;

    public BlockCypherService(HttpClient httpClient, IConfiguration configuration, ILogger<BlockCypherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
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
        const int maxRetries = 3;
        const int baseDelayMs = 2000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
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
                    var delayMs = baseDelayMs * (int)Math.Pow(2, attempt - 1); // Exponential backoff
                    _logger.LogWarning("Rate limited for blockchain {BlockchainName}. Attempt {Attempt}/{MaxRetries}. Waiting {DelayMs}ms", 
                        blockchainName, attempt, maxRetries, delayMs);
                    
                    if (attempt < maxRetries)
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
                var delayMs = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                _logger.LogWarning("Rate limited for blockchain {BlockchainName}. Attempt {Attempt}/{MaxRetries}. Waiting {DelayMs}ms", 
                    blockchainName, attempt, maxRetries, delayMs);
                
                if (attempt < maxRetries)
                {
                    await Task.Delay(delayMs);
                    continue;
                }
                
                _logger.LogError(ex, "HTTP request failed after {MaxRetries} attempts for blockchain {BlockchainName}", maxRetries, blockchainName);
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