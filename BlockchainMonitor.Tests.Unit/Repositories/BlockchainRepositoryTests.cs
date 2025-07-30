using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BlockchainMonitor.Infrastructure.Data;
using BlockchainMonitor.Infrastructure.Repositories;
using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Domain.Interfaces;

namespace BlockchainMonitor.Tests.Unit.Repositories;

public class BlockchainRepositoryTests
{
    private readonly DbContextOptions<BlockchainDbContext> _options;
    private readonly Mock<ILogger<BlockchainRepository>> _mockLogger;

    public BlockchainRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<BlockchainDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockLogger = new Mock<ILogger<BlockchainRepository>>();
    }

    private BlockchainDbContext CreateContext()
    {
        return new BlockchainDbContext(_options);
    }

    private BlockchainRepository CreateRepository(BlockchainDbContext context)
    {
        return new BlockchainRepository(context);
    }

    [Fact]
    public async Task GetLatestByNameAsync_WhenDataExists_ShouldReturnLatestData()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var blockchainName = "BTC.main";
        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = blockchainName, Height = 800000, Hash = "hash1", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 2, Name = blockchainName, Height = 800001, Hash = "hash2", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = 3, Name = blockchainName, Height = 800002, Hash = "hash3", CreatedAt = DateTime.UtcNow }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetLatestByNameAsync(blockchainName);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(3);
        result.Height.Should().Be(800002);
        result.Hash.Should().Be("hash3");
    }

    [Fact]
    public async Task GetLatestByNameAsync_WhenDataDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetLatestByNameAsync("INVALID.main");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetHistoryByNameAsync_WhenDataExists_ShouldReturnOrderedHistory()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var blockchainName = "BTC.main";
        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = blockchainName, Height = 800000, Hash = "hash1", CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new() { Id = 2, Name = blockchainName, Height = 800001, Hash = "hash2", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 3, Name = blockchainName, Height = 800002, Hash = "hash3", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = 4, Name = blockchainName, Height = 800003, Hash = "hash4", CreatedAt = DateTime.UtcNow }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetHistoryByNameAsync(blockchainName, 3);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(x => x.CreatedAt);
        result.First().Height.Should().Be(800003);
        result.Last().Height.Should().Be(800001);
    }

    [Fact]
    public async Task GetHistoryByNameAsync_WhenLimitExceedsData_ShouldReturnAllData()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var blockchainName = "BTC.main";
        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = blockchainName, Height = 800000, Hash = "hash1", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = 2, Name = blockchainName, Height = 800001, Hash = "hash2", CreatedAt = DateTime.UtcNow }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetHistoryByNameAsync(blockchainName, 10);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(x => x.CreatedAt);
    }

    [Fact]
    public async Task GetLatestDataAsync_WhenDataExists_ShouldReturnLatestForEachBlockchain()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "btc-hash1", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 2, Name = "BTC.main", Height = 800001, Hash = "btc-hash2", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = 3, Name = "ETH.main", Height = 18000000, Hash = "eth-hash1", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 4, Name = "ETH.main", Height = 18000001, Hash = "eth-hash2", CreatedAt = DateTime.UtcNow }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetLatestDataAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Name == "BTC.main" && x.Height == 800001);
        result.Should().Contain(x => x.Name == "ETH.main" && x.Height == 18000001);
    }

    [Fact]
    public async Task GetByDateRangeAsync_WhenDataExists_ShouldReturnDataInRange()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var now = DateTime.UtcNow;
        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "hash1", CreatedAt = now.AddHours(-3) },
            new() { Id = 2, Name = "BTC.main", Height = 800001, Hash = "hash2", CreatedAt = now.AddHours(-2) },
            new() { Id = 3, Name = "BTC.main", Height = 800002, Hash = "hash3", CreatedAt = now.AddHours(-1) },
            new() { Id = 4, Name = "BTC.main", Height = 800003, Hash = "hash4", CreatedAt = now }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        var startDate = now.AddHours(-2.5);
        var endDate = now.AddHours(-0.5);

        // Act
        var result = await repository.GetByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == 2);
        result.Should().Contain(x => x.Id == 3);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_WhenDataExists_ShouldReturnCorrectCount()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "hash1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "hash2" },
            new() { Id = 3, Name = "LTC.main", Height = 2500000, Hash = "hash3" }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetTotalRecordsAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_WhenNoData_ShouldReturnZero()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetTotalRecordsAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task AddAsync_WhenValidData_ShouldAddToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash",
            Time = DateTime.UtcNow
        };

        // Act
        var result = await repository.AddAsync(blockchainData);
        await context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("BTC.main");
        
        var savedData = await context.BlockchainData.FirstOrDefaultAsync(x => x.Name == "BTC.main");
        savedData.Should().NotBeNull();
        savedData!.Height.Should().Be(800000);
        savedData.Hash.Should().Be("test-hash");
    }

    [Fact]
    public async Task GetAllAsync_WhenDataExists_ShouldReturnAllData()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var data = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "hash1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "hash2" },
            new() { Id = 3, Name = "LTC.main", Height = 2500000, Hash = "hash3" }
        };

        context.BlockchainData.AddRange(data);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(x => x.Name == "BTC.main");
        result.Should().Contain(x => x.Name == "ETH.main");
        result.Should().Contain(x => x.Name == "LTC.main");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDataExists_ShouldReturnData()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        var blockchainData = new BlockchainData
        {
            Id = 42,
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash"
        };

        context.BlockchainData.Add(blockchainData);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(42);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Name.Should().Be("BTC.main");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDataDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
} 