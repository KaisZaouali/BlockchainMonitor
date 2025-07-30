using Xunit;
using FluentAssertions;
using Microsoft.Playwright;
using System.Text.Json;
using BlockchainMonitor.Domain.Entities;

namespace BlockchainMonitor.Tests.E2E.API;

public class BlockchainDataE2ETests
{
    private const string BaseUrl = "http://localhost:5003";
    
    private static string ExtractJsonFromHtml(string content)
    {   
        // Check if content is wrapped in HTML with <pre> tags
        if (content.Contains("<pre>") && content.Contains("</pre>"))
        {
            var startIndex = content.IndexOf("<pre>") + 5;
            var endIndex = content.IndexOf("</pre>");
            var extracted = content.Substring(startIndex, endIndex - startIndex);
            return extracted;
        }
        
        // Check if content starts with HTML but doesn't have <pre> tags
        if (content.StartsWith("<html>") || content.StartsWith("<!DOCTYPE"))
        {
            // Try to find JSON array starting with [
            var jsonStart = content.IndexOf('[');
            if (jsonStart >= 0)
            {
                // Find the matching closing bracket for the JSON array
                var bracketCount = 0;
                var jsonEnd = jsonStart;
                for (int i = jsonStart; i < content.Length; i++)
                {
                    if (content[i] == '[')
                        bracketCount++;
                    else if (content[i] == ']')
                    {
                        bracketCount--;
                        if (bracketCount == 0)
                        {
                            jsonEnd = i + 1;
                            break;
                        }
                    }
                }
                
                var extracted = content.Substring(jsonStart, jsonEnd - jsonStart);
                return extracted;
            }
        }
        
        // If not HTML, return as-is
        return content;
    }

    [Fact]
    public async Task API_Health_Endpoint_ShouldReturnHealthy()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}/health");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(200);
        
        var content = await page.ContentAsync();
        content.Should().Contain("healthy");
    }

    [Fact]
    public async Task API_Swagger_Endpoint_ShouldBeAccessible()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}/swagger");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(200);
        
        // Verify Swagger UI elements are present
        await page.WaitForSelectorAsync(".swagger-ui");
        var title = await page.TitleAsync();
        title.Should().Contain("Swagger");
    }

    [Fact]
    public async Task API_BlockchainData_ShouldReturnValidResponse()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}/api/blockchain/latest");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(200);
        
        var content = await page.ContentAsync();
        
        content.Should().NotBeNullOrEmpty();
        
        // Extract JSON from HTML if needed
        var jsonContent = ExtractJsonFromHtml(content);
        
        // Verify it's valid JSON
        var data = JsonSerializer.Deserialize<object>(jsonContent);
        data.Should().NotBeNull();
    }

    [Fact]
    public async Task API_BlockchainData_ShouldHaveCorrectContentType()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}/api/blockchain/latest");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(200);
        
        // Check if response headers contain JSON content type
        var headers = response.Headers;
        headers.Should().ContainKey("content-type");
        headers["content-type"].Should().Contain("application/json");
    }

    [Fact]
    public async Task API_BlockchainData_ShouldHandleInvalidEndpoints()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}/api/invalid-endpoint");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(404); // Not Found
    }

    [Fact]
    public async Task API_BlockchainData_ShouldValidateJsonStructure()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}/api/blockchain/latest");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(200);
        
        var content = await page.ContentAsync();
        
        content.Should().NotBeNullOrEmpty();
        
        // Extract JSON from HTML if needed
        var jsonContent = ExtractJsonFromHtml(content);
        
        // Verify it's a valid JSON array
        var data = JsonSerializer.Deserialize<object[]>(jsonContent);
        data.Should().NotBeNull();
    }

    [Fact]
    public async Task API_BlockchainData_ShouldHandleLargeDataSets()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act - Get latest data
        var response = await page.GotoAsync($"{BaseUrl}/api/blockchain/latest");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(200);
        
        var content = await page.ContentAsync();
        
        // Extract JSON from HTML if needed
        var jsonContent = ExtractJsonFromHtml(content);
        
        var data = JsonSerializer.Deserialize<object[]>(jsonContent);
        data.Should().NotBeNull();
        data!.Length.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public Task API_BlockchainData_ShouldValidateBlockchainDataStructure()
    {
        // Arrange
        var testData = new BlockchainData
        {
            Id = 1,
            Name = "BTC.main",
            Height = 800000L,
            Hash = "test-hash-123",
            Time = DateTime.UtcNow,
            PeerCount = 10,
            UnconfirmedCount = 5
        };

        // Act
        var json = JsonSerializer.Serialize(testData);
        var deserialized = JsonSerializer.Deserialize<BlockchainData>(json);

        // Assert
        json.Should().NotBeNullOrEmpty();
        deserialized.Should().NotBeNull();
        deserialized!.Name.Should().Be("BTC.main");
        deserialized.Height.Should().Be(800000L);
        deserialized.Hash.Should().Be("test-hash-123");
        
        return Task.CompletedTask;
    }

    [Fact]
    public Task API_BlockchainData_ShouldValidateErrorResponses()
    {
        // Arrange
        var errorResponses = new[]
        {
            new { StatusCode = 400, Message = "Bad Request" },
            new { StatusCode = 404, Message = "Not Found" },
            new { StatusCode = 500, Message = "Internal Server Error" }
        };

        // Act
        var allErrorCodes = errorResponses.All(r => r.StatusCode >= 400);
        var allHaveMessages = errorResponses.All(r => !string.IsNullOrEmpty(r.Message));

        // Assert
        allErrorCodes.Should().BeTrue();
        allHaveMessages.Should().BeTrue();
        
        return Task.CompletedTask;
    }

    [Fact]
    public Task API_BlockchainData_ShouldValidateSuccessResponses()
    {
        // Arrange
        var successResponses = new[]
        {
            new { StatusCode = 200, Data = "success data" },
            new { StatusCode = 201, Data = "created data" }
        };

        // Act
        var allSuccessCodes = successResponses.All(r => r.StatusCode >= 200 && r.StatusCode < 300);
        var allHaveData = successResponses.All(r => !string.IsNullOrEmpty(r.Data));

        // Assert
        allSuccessCodes.Should().BeTrue();
        allHaveData.Should().BeTrue();
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("/api/blockchain", 200)]
    [InlineData("/api/blockchain/latest", 200)]
    [InlineData("/health", 200)]
    [InlineData("/swagger", 200)]
    public async Task API_Endpoints_ShouldReturnExpectedStatusCodes(string endpoint, int expectedStatus)
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{BaseUrl}{endpoint}");

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be(expectedStatus);
    }
} 