using Xunit;
using FluentAssertions;
using BlockchainMonitor.Domain.Entities;

namespace BlockchainMonitor.Tests.Unit.Domain;

public class BlockchainDataTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var name = "BTC.main";
        var height = 800000L;
        var hash = "test-hash-123";
        var time = DateTime.UtcNow;

        // Act
        var blockchainData = new BlockchainData
        {
            Name = name,
            Height = height,
            Hash = hash,
            Time = time
        };

        // Assert
        blockchainData.Should().NotBeNull();
        blockchainData.Name.Should().Be(name);
        blockchainData.Height.Should().Be(height);
        blockchainData.Hash.Should().Be(hash);
        blockchainData.Time.Should().Be(time);
        blockchainData.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("BTC.main")]
    [InlineData("ETH.main")]
    [InlineData("LTC.main")]
    [InlineData("DASH.main")]
    [InlineData("BTC.test3")]
    public void Constructor_WithValidBlockchainNames_ShouldAccept(string validName)
    {
        // Act
        var blockchainData = new BlockchainData
        {
            Name = validName,
            Height = 800000L,
            Hash = "test-hash"
        };

        // Assert
        blockchainData.Name.Should().Be(validName);
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(1000000L)]
    [InlineData(long.MaxValue)]
    public void Constructor_WithValidHeights_ShouldAccept(long validHeight)
    {
        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = validHeight,
            Hash = "test-hash"
        };

        // Assert
        blockchainData.Height.Should().Be(validHeight);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("test-hash-123")]
    [InlineData("0000000000000000000000000000000000000000000000000000000000000000")]
    public void Constructor_WithValidHashes_ShouldAccept(string validHash)
    {
        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 800000L,
            Hash = validHash
        };

        // Assert
        blockchainData.Hash.Should().Be(validHash);
    }

    [Fact]
    public void Constructor_WithCustomTime_ShouldSetCustomTime()
    {
        // Arrange
        var customTime = DateTime.UtcNow.AddHours(-1);

        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 800000L,
            Hash = "test-hash",
            Time = customTime
        };

        // Assert
        blockchainData.Time.Should().Be(customTime);
    }

    [Fact]
    public void Constructor_WithId_ShouldSetId()
    {
        // Arrange
        var expectedId = 42;

        // Act
        var blockchainData = new BlockchainData
        {
            Id = expectedId,
            Name = "BTC.main",
            Height = 800000L,
            Hash = "test-hash"
        };

        // Assert
        blockchainData.Id.Should().Be(expectedId);
    }

    [Fact]
    public void Constructor_WithOptionalProperties_ShouldSetCorrectly()
    {
        // Arrange
        var latestUrl = "https://api.blockcypher.com/v1/btc/main";
        var previousHash = "previous-hash-123";
        var previousUrl = "https://api.blockcypher.com/v1/btc/main";
        var peerCount = 50;
        var unconfirmedCount = 1000;
        var highFeePerKb = 1000L;
        var mediumFeePerKb = 500L;
        var lowFeePerKb = 100L;

        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 800000L,
            Hash = "test-hash",
            LatestUrl = latestUrl,
            PreviousHash = previousHash,
            PreviousUrl = previousUrl,
            PeerCount = peerCount,
            UnconfirmedCount = unconfirmedCount,
            HighFeePerKb = highFeePerKb,
            MediumFeePerKb = mediumFeePerKb,
            LowFeePerKb = lowFeePerKb
        };

        // Assert
        blockchainData.LatestUrl.Should().Be(latestUrl);
        blockchainData.PreviousHash.Should().Be(previousHash);
        blockchainData.PreviousUrl.Should().Be(previousUrl);
        blockchainData.PeerCount.Should().Be(peerCount);
        blockchainData.UnconfirmedCount.Should().Be(unconfirmedCount);
        blockchainData.HighFeePerKb.Should().Be(highFeePerKb);
        blockchainData.MediumFeePerKb.Should().Be(mediumFeePerKb);
        blockchainData.LowFeePerKb.Should().Be(lowFeePerKb);
    }

    [Fact]
    public void Constructor_WithGasPriceProperties_ShouldSetCorrectly()
    {
        // Arrange
        var highGasPrice = 20000000000L; // 20 Gwei
        var mediumGasPrice = 15000000000L; // 15 Gwei
        var lowGasPrice = 10000000000L; // 10 Gwei
        var highPriorityFee = 2000000000L; // 2 Gwei
        var mediumPriorityFee = 1500000000L; // 1.5 Gwei
        var lowPriorityFee = 1000000000L; // 1 Gwei
        var baseFee = 10000000000L; // 10 Gwei

        // Act
        var blockchainData = new BlockchainData
        {
            Name = "ETH.main",
            Height = 18000000L,
            Hash = "test-hash",
            HighGasPrice = highGasPrice,
            MediumGasPrice = mediumGasPrice,
            LowGasPrice = lowGasPrice,
            HighPriorityFee = highPriorityFee,
            MediumPriorityFee = mediumPriorityFee,
            LowPriorityFee = lowPriorityFee,
            BaseFee = baseFee
        };

        // Assert
        blockchainData.HighGasPrice.Should().Be(highGasPrice);
        blockchainData.MediumGasPrice.Should().Be(mediumGasPrice);
        blockchainData.LowGasPrice.Should().Be(lowGasPrice);
        blockchainData.HighPriorityFee.Should().Be(highPriorityFee);
        blockchainData.MediumPriorityFee.Should().Be(mediumPriorityFee);
        blockchainData.LowPriorityFee.Should().Be(lowPriorityFee);
        blockchainData.BaseFee.Should().Be(baseFee);
    }

    [Fact]
    public void Constructor_WithForkInformation_ShouldSetCorrectly()
    {
        // Arrange
        var lastForkHeight = 800000L;
        var lastForkHash = "fork-hash-123";

        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 800001L,
            Hash = "test-hash",
            LastForkHeight = lastForkHeight,
            LastForkHash = lastForkHash
        };

        // Assert
        blockchainData.LastForkHeight.Should().Be(lastForkHeight);
        blockchainData.LastForkHash.Should().Be(lastForkHash);
    }

    [Fact]
    public void Constructor_WithNullOptionalProperties_ShouldAccept()
    {
        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 800000L,
            Hash = "test-hash",
            LatestUrl = null,
            PreviousHash = null,
            PreviousUrl = null,
            HighFeePerKb = null,
            MediumFeePerKb = null,
            LowFeePerKb = null,
            HighGasPrice = null,
            MediumGasPrice = null,
            LowGasPrice = null,
            HighPriorityFee = null,
            MediumPriorityFee = null,
            LowPriorityFee = null,
            BaseFee = null,
            LastForkHeight = null,
            LastForkHash = null
        };

        // Assert
        blockchainData.LatestUrl.Should().BeNull();
        blockchainData.PreviousHash.Should().BeNull();
        blockchainData.PreviousUrl.Should().BeNull();
        blockchainData.HighFeePerKb.Should().BeNull();
        blockchainData.MediumFeePerKb.Should().BeNull();
        blockchainData.LowFeePerKb.Should().BeNull();
        blockchainData.HighGasPrice.Should().BeNull();
        blockchainData.MediumGasPrice.Should().BeNull();
        blockchainData.LowGasPrice.Should().BeNull();
        blockchainData.HighPriorityFee.Should().BeNull();
        blockchainData.MediumPriorityFee.Should().BeNull();
        blockchainData.LowPriorityFee.Should().BeNull();
        blockchainData.BaseFee.Should().BeNull();
        blockchainData.LastForkHeight.Should().BeNull();
        blockchainData.LastForkHash.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithZeroValues_ShouldAccept()
    {
        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = 0L,
            Hash = "test-hash",
            PeerCount = 0,
            UnconfirmedCount = 0
        };

        // Assert
        blockchainData.Height.Should().Be(0L);
        blockchainData.PeerCount.Should().Be(0);
        blockchainData.UnconfirmedCount.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithNegativeValues_ShouldAccept()
    {
        // Act
        var blockchainData = new BlockchainData
        {
            Name = "BTC.main",
            Height = -1L,
            Hash = "test-hash",
            PeerCount = -1,
            UnconfirmedCount = -1
        };

        // Assert
        blockchainData.Height.Should().Be(-1L);
        blockchainData.PeerCount.Should().Be(-1);
        blockchainData.UnconfirmedCount.Should().Be(-1);
    }
} 