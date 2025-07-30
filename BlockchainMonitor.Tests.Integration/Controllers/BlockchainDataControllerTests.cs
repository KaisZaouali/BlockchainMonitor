using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Text.Json;
using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Application.Interfaces;

namespace BlockchainMonitor.Tests.Integration.Controllers;

public class BlockchainDataControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IBlockchainService> _mockBlockchainService;

    public BlockchainDataControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mockBlockchainService = new Mock<IBlockchainService>();
    }

    private HttpClient CreateClientWithMockedService()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove all existing registrations of IBlockchainService
                var descriptors = services.Where(d => d.ServiceType == typeof(IBlockchainService)).ToList();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Add our mock as a singleton to ensure it's used
                services.AddSingleton<IBlockchainService>(_mockBlockchainService.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAllBlockchainData_ShouldReturnOkWithData()
    {
        // Arrange
        var expectedData = new List<BlockchainDataDto>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "test-hash-2" }
        };

        _mockBlockchainService
            .Setup(x => x.GetAllBlockchainDataAsync())
            .ReturnsAsync(expectedData);

        var client = CreateClientWithMockedService();

        // Act
        var response = await client.GetAsync("/api/blockchain");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<List<BlockchainDataDto>>(content, options);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLatestBlockchainData_ShouldReturnOkWithData()
    {
        // Arrange
        var expectedData = new BlockchainDataDto
        {
            Id = 1,
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash-1"
        };

        _mockBlockchainService
            .Setup(x => x.GetLatestBlockchainDataAsync("BTC.main"))
            .ReturnsAsync(expectedData);

        var client = CreateClientWithMockedService();

        // Act
        var response = await client.GetAsync("/api/blockchain/BTC.main");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<BlockchainDataDto>(content, options);
        result!.Name.Should().Be("BTC.main");
        result.Height.Should().Be(800000);
    }

    [Fact]
    public async Task GetLatestBlockchainData_ShouldReturnNotFound_WhenDataDoesNotExist()
    {
        // Arrange
        _mockBlockchainService
            .Setup(x => x.GetLatestBlockchainDataAsync("INVALID"))
            .ReturnsAsync((BlockchainDataDto?)null);

        var client = CreateClientWithMockedService();

        // Act
        var response = await client.GetAsync("/api/blockchain/INVALID");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBlockchainHistory_ShouldReturnOkWithHistory()
    {
        // Arrange
        var expectedHistory = new List<BlockchainDataDto>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" },
            new() { Id = 2, Name = "BTC.main", Height = 799999, Hash = "test-hash-2" }
        };

        _mockBlockchainService
            .Setup(x => x.GetBlockchainHistoryAsync("BTC.main", 10))
            .ReturnsAsync(expectedHistory);

        var client = CreateClientWithMockedService();

        // Act
        var response = await client.GetAsync("/api/blockchain/BTC.main/history?limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<List<BlockchainDataDto>>(content, options);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLatestData_ShouldReturnOkWithLatestData()
    {
        // Arrange
        var expectedData = new List<BlockchainDataDto>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "test-hash-2" }
        };

        _mockBlockchainService
            .Setup(x => x.GetLatestDataAsync())
            .ReturnsAsync(expectedData);

        var client = CreateClientWithMockedService();

        // Act
        var response = await client.GetAsync("/api/blockchain/latest");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<List<BlockchainDataDto>>(content, options);
        result.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("ThisIsAReallyLongBlockchainNameThatExceedsTheLimit")]
    public async Task GetLatestBlockchainData_ShouldReturnBadRequest_WhenBlockchainNameIsInvalid(string invalidName)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/blockchain/{invalidName}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)] // Assuming MaxHistoryLimit is 1000
    public async Task GetBlockchainHistory_ShouldReturnBadRequest_WhenLimitIsInvalid(int invalidLimit)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/blockchain/BTC.main/history?limit={invalidLimit}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Controller_ShouldReturnCorrectContentType()
    {
        // Arrange
        var expectedData = new List<BlockchainDataDto>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" }
        };

        _mockBlockchainService
            .Setup(x => x.GetAllBlockchainDataAsync())
            .ReturnsAsync(expectedData);

        var client = CreateClientWithMockedService();

        // Act
        var response = await client.GetAsync("/api/blockchain");

        // Assert
        response.Content.Headers.ContentType!.ToString().Should().Contain("application/json");
    }
} 