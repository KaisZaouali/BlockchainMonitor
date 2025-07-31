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

namespace BlockchainMonitor.Tests.Integration.Services;

public class BlockchainServiceIntegrationTests
{
    private readonly Mock<IBlockchainRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<ILogger<BlockchainService>> _mockLogger;
    private readonly Mock<IOptions<CacheSettings>> _mockCacheSettings;
    private readonly BlockchainService _service;

    public BlockchainServiceIntegrationTests()
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
    public async Task GetAllBlockchainDataAsync_ShouldReturnMappedData()
    {
        // Arrange
        var entities = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "test-hash-2" }
        };

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(entities);

        // Mock cache to return null (cache miss)
        _mockCacheService
            .Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<BlockchainDataDto>?)null);

        // Mock cache SetAsync method
        _mockCacheService
            .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<BlockchainDataDto>>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetAllBlockchainDataAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("BTC.main");
        result.First().Height.Should().Be(800000);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLatestBlockchainDataAsync_ShouldReturnMappedData()
    {
        // Arrange
        var entity = new BlockchainData
        {
            Id = 1,
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash-1"
        };

        _mockRepository
            .Setup(x => x.GetLatestByNameAsync("BTC.main"))
            .ReturnsAsync(entity);

        // Mock cache to return null (cache miss)
        _mockCacheService
            .Setup(x => x.GetAsync<BlockchainDataDto>(It.IsAny<string>()))
            .ReturnsAsync((BlockchainDataDto?)null);

        // Mock cache SetAsync method
        _mockCacheService
            .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<BlockchainDataDto>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetLatestBlockchainDataAsync("BTC.main");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("BTC.main");
        result.Height.Should().Be(800000);
        _mockRepository.Verify(x => x.GetLatestByNameAsync("BTC.main"), Times.Once);
    }

    [Fact]
    public async Task GetLatestBlockchainDataAsync_ShouldReturnNull_WhenDataDoesNotExist()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetLatestByNameAsync("INVALID"))
            .ReturnsAsync((BlockchainData?)null);

        // Act
        var result = await _service.GetLatestBlockchainDataAsync("INVALID");

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetLatestByNameAsync("INVALID"), Times.Once);
    }

    [Fact]
    public async Task GetBlockchainHistoryAsync_ShouldReturnMappedHistory()
    {
        // Arrange
        var entities = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" },
            new() { Id = 2, Name = "BTC.main", Height = 799999, Hash = "test-hash-2" }
        };

        _mockRepository
            .Setup(x => x.GetHistoryByNameAsync("BTC.main", BlockchainConstants.MaxHistoryLimit))
            .ReturnsAsync(entities);

        // Mock cache to return null (cache miss)
        _mockCacheService
            .Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<BlockchainDataDto>?)null);

        // Mock cache SetAsync method
        _mockCacheService
            .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<BlockchainDataDto>>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetBlockchainHistoryAsync("BTC.main", 10);

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("BTC.main");
        _mockRepository.Verify(x => x.GetHistoryByNameAsync("BTC.main", BlockchainConstants.MaxHistoryLimit), Times.Once);
    }

    [Fact]
    public async Task GetLatestDataAsync_ShouldReturnMappedData()
    {
        // Arrange
        var entities = new List<BlockchainData>
        {
            new() { Id = 1, Name = "BTC.main", Height = 800000, Hash = "test-hash-1" },
            new() { Id = 2, Name = "ETH.main", Height = 18000000, Hash = "test-hash-2" }
        };

        _mockRepository
            .Setup(x => x.GetLatestDataAsync())
            .ReturnsAsync(entities);

        // Mock cache to return null (cache miss)
        _mockCacheService
            .Setup(x => x.GetAsync<IEnumerable<BlockchainDataDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<BlockchainDataDto>?)null);

        // Mock cache SetAsync method
        _mockCacheService
            .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<BlockchainDataDto>>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetLatestDataAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("BTC.main");
        result.Last().Name.Should().Be("ETH.main");
        _mockRepository.Verify(x => x.GetLatestDataAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBlockchainDataAsync_ShouldMapAndSaveData()
    {
        // Arrange
        var dto = new BlockchainDataDto
        {
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash-1"
        };

        var entity = new BlockchainData
        {
            Id = 1,
            Name = "BTC.main",
            Height = 800000,
            Hash = "test-hash-1"
        };

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<BlockchainData>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _service.CreateBlockchainDataAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("BTC.main");
        result.Height.Should().Be(800000);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<BlockchainData>()), Times.Once);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnRepositoryCount()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetTotalRecordsAsync())
            .ReturnsAsync(42);

        // Act
        var result = await _service.GetTotalRecordsAsync();

        // Assert
        result.Should().Be(42);
        _mockRepository.Verify(x => x.GetTotalRecordsAsync(), Times.Once);
    }
} 