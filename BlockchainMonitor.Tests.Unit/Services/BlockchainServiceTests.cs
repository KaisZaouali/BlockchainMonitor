using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BlockchainMonitor.Application.Services;
using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Domain.Interfaces;
using BlockchainMonitor.Application.DTOs;
using BlockchainMonitor.Domain.Entities;
using BlockchainMonitor.Infrastructure.Configuration;
using BlockchainMonitor.Domain.Events;
using BlockchainMonitor.Application.Exceptions;
using BlockchainMonitor.Application.Constants;
using BlockchainMonitor.Infrastructure.Interfaces;

namespace BlockchainMonitor.Tests.Unit.Services;

public class BlockchainServiceTests
{
    private readonly Mock<IBlockchainRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<ILogger<BlockchainService>> _mockLogger;
    private readonly Mock<IOptions<CacheSettings>> _mockCacheSettings;
    private readonly BlockchainService _service;

    public BlockchainServiceTests()
    {
        _mockRepository = new Mock<IBlockchainRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCacheService = new Mock<ICacheService>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockLogger = new Mock<ILogger<BlockchainService>>();
        _mockCacheSettings = new Mock<IOptions<CacheSettings>>();

        _mockCacheSettings.Setup(x => x.Value).Returns(new CacheSettings
        {
            AllBlockchainDataDurationMinutes = 5,
            LatestBlockchainDataDurationMinutes = 2,
            BlockchainHistoryDurationMinutes = 10,
            LatestDataDurationMinutes = 3,
            TotalRecordsDurationMinutes = 1
        });

        _service = new BlockchainService(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockCacheService.Object,
            _mockEventPublisher.Object,
            _mockLogger.Object,
            _mockCacheSettings.Object);
    }

    [Fact]
    public async Task GetAllBlockchainDataAsync_WhenCacheHit_ShouldReturnCachedData()
    {
        // Arrange
        var expectedData = new List<BlockchainDataDto>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "hash1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "hash2" }
        };

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>("all_blockchain_data"))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _service.GetAllBlockchainDataAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedData);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
        _mockCacheService.Verify(x => x.SetAsync("all_blockchain_data", It.IsAny<IEnumerable<BlockchainDataDto>>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public async Task GetAllBlockchainDataAsync_WhenCacheMiss_ShouldFetchFromRepositoryAndCache()
    {
        // Arrange
        var repositoryData = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "hash1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "hash2" }
        };

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>("all_blockchain_data"))
            .ReturnsAsync((IEnumerable<BlockchainDataDto>?)null);
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(repositoryData);

        // Act
        var result = await _service.GetAllBlockchainDataAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Name == "BTC.main");
        result.Should().Contain(x => x.Name == "ETH.main");
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        _mockCacheService.Verify(x => x.SetAsync("all_blockchain_data", It.IsAny<IEnumerable<BlockchainDataDto>>(), TimeSpan.FromMinutes(5)), Times.Once);
    }

    [Fact]
    public async Task GetLatestBlockchainDataAsync_WhenDataExists_ShouldReturnData()
    {
        // Arrange
        var blockchainName = "BTC.main";
        var expectedData = new BlockchainDataDto
        {
            Id = 1,
            Name = blockchainName,
            Height = 800000,
            Hash = "test-hash"
        };

        _mockRepository.Setup(x => x.GetLatestByNameAsync(blockchainName))
            .ReturnsAsync(new BlockchainData
            {
                Id = 1,
                Name = blockchainName,
                Height = 800000,
                Hash = "test-hash"
            });

        // Act
        var result = await _service.GetLatestBlockchainDataAsync(blockchainName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(blockchainName);
        result.Height.Should().Be(800000);
        result.Hash.Should().Be("test-hash");
    }

    [Fact]
    public async Task GetLatestBlockchainDataAsync_WhenDataDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var blockchainName = "INVALID.main";

        _mockRepository.Setup(x => x.GetLatestByNameAsync(blockchainName))
            .ReturnsAsync((BlockchainData?)null);

        // Act
        var result = await _service.GetLatestBlockchainDataAsync(blockchainName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBlockchainHistoryAsync_WhenDataExists_ShouldReturnHistory()
    {
        // Arrange
        var blockchainName = "BTC.main";
        var limit = 5;
        var repositoryData = new List<BlockchainData>
        {
            new() { Id = 1, Name = blockchainName, Height = 800000, Hash = "hash1" },
            new() { Id = 2, Name = blockchainName, Height = 799999, Hash = "hash2" },
            new() { Id = 3, Name = blockchainName, Height = 799998, Hash = "hash3" }
        };

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>($"blockchain_history_{blockchainName}"))
            .ReturnsAsync((IEnumerable<BlockchainDataDto>?)null);
        _mockRepository.Setup(x => x.GetHistoryByNameAsync(blockchainName, 1000)) // MaxHistoryLimit
            .ReturnsAsync(repositoryData);

        // Act
        var result = await _service.GetBlockchainHistoryAsync(blockchainName, limit);

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(x => x.Name == blockchainName);
        result.Should().BeInDescendingOrder(x => x.Height);
        _mockRepository.Verify(x => x.GetHistoryByNameAsync(blockchainName, 1000), Times.Once);
        _mockCacheService.Verify(x => x.SetAsync($"blockchain_history_{blockchainName}", It.IsAny<IEnumerable<BlockchainDataDto>>(), TimeSpan.FromMinutes(10)), Times.Once);
    }

    [Fact]
    public async Task GetLatestDataAsync_WhenDataExists_ShouldReturnLatestData()
    {
        // Arrange
        var repositoryData = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "hash1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "hash2" }
        };

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>("latest_data_all_blockchains"))
            .ReturnsAsync((IEnumerable<BlockchainDataDto>?)null);
        _mockRepository.Setup(x => x.GetLatestDataAsync())
            .ReturnsAsync(repositoryData);

        // Act
        var result = await _service.GetLatestDataAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Name == "BTC.main");
        result.Should().Contain(x => x.Name == "ETH.main");
        _mockRepository.Verify(x => x.GetLatestDataAsync(), Times.Once);
        _mockCacheService.Verify(x => x.SetAsync("latest_data_all_blockchains", It.IsAny<IEnumerable<BlockchainDataDto>>(), TimeSpan.FromMinutes(3)), Times.Once);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var expectedCount = 42;

        _mockRepository.Setup(x => x.GetTotalRecordsAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _service.GetTotalRecordsAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task CreateBlockchainDataAsync_WhenValidData_ShouldCreateAndReturnData()
    {
        // Arrange
        var dto = new BlockchainDataDto
        {
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash",
            Time = DateTime.UtcNow
        };

        var createdEntity = new BlockchainData
        {
            Id = 1,
            Name = dto.Name,
            Height = dto.Height,
            Hash = dto.Hash,
            Time = dto.Time
        };

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<BlockchainData>()))
            .ReturnsAsync(createdEntity);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateBlockchainDataAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Height.Should().Be(dto.Height);
        result.Hash.Should().Be(dto.Hash);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<BlockchainData>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockEventPublisher.Verify(x => x.Publish(It.IsAny<BlockchainDataCreatedEvent>()), Times.Once);
        // Note: Cache invalidation is handled by the event handler, not directly in the service
    }

    [Fact]
    public async Task CreateBlockchainDataAsync_WhenSaveFails_ShouldNotThrowException()
    {
        // Arrange
        var dto = new BlockchainDataDto
        {
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash"
        };

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<BlockchainData>()))
            .ReturnsAsync(new BlockchainData());
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(0); // No rows affected

        // Act & Assert - The service doesn't throw an exception on save failure
        var result = await _service.CreateBlockchainDataAsync(dto);
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task CreateBlockchainDataAsync_WithInvalidName_ShouldThrowInvalidBlockchainDataException(string? invalidName)
    {
        // Arrange
        var dto = new BlockchainDataDto
        {
            Name = invalidName!,
            Height = 800000,
            Hash = "test-hash"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidBlockchainDataException>(() =>
            _service.CreateBlockchainDataAsync(dto));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateBlockchainDataAsync_WithInvalidHeight_ShouldThrowInvalidBlockchainDataException(int invalidHeight)
    {
        // Arrange
        var dto = new BlockchainDataDto
        {
            Name = "BTC.main",
            Height = invalidHeight,
            Hash = "test-hash"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidBlockchainDataException>(() =>
            _service.CreateBlockchainDataAsync(dto));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task CreateBlockchainDataAsync_WithInvalidHash_ShouldThrowInvalidBlockchainDataException(string? invalidHash)
    {
        // Arrange
        var dto = new BlockchainDataDto
        {
            Name = "BTC.main",
            Height = 800000,
            Hash = invalidHash!
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidBlockchainDataException>(() =>
            _service.CreateBlockchainDataAsync(dto));
    }
}
