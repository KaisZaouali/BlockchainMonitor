using BlockchainMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlockchainMonitor.Infrastructure.Data;

public class BlockchainDbContext : DbContext
{
    public BlockchainDbContext(DbContextOptions<BlockchainDbContext> options) : base(options)
    {
    }

    public DbSet<BlockchainData> BlockchainData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure BlockchainData entity (single table for all historical data)
        modelBuilder.Entity<BlockchainData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Height).IsRequired();
            entity.Property(e => e.Hash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Time).IsRequired();
            entity.Property(e => e.LatestUrl).HasMaxLength(200);
            entity.Property(e => e.PreviousHash).HasMaxLength(100);
            entity.Property(e => e.PreviousUrl).HasMaxLength(200);
            entity.Property(e => e.PeerCount).IsRequired();
            entity.Property(e => e.UnconfirmedCount).IsRequired();
            
            // Fee-related fields
            entity.Property(e => e.HighFeePerKb).HasColumnType("bigint");
            entity.Property(e => e.MediumFeePerKb).HasColumnType("bigint");
            entity.Property(e => e.LowFeePerKb).HasColumnType("bigint");
            
            // Gas-related fields (for Ethereum)
            entity.Property(e => e.HighGasPrice).HasColumnType("bigint");
            entity.Property(e => e.MediumGasPrice).HasColumnType("bigint");
            entity.Property(e => e.LowGasPrice).HasColumnType("bigint");
            entity.Property(e => e.HighPriorityFee).HasColumnType("bigint");
            entity.Property(e => e.MediumPriorityFee).HasColumnType("bigint");
            entity.Property(e => e.LowPriorityFee).HasColumnType("bigint");
            entity.Property(e => e.BaseFee).HasColumnType("bigint");
            
            // Fork information
            entity.Property(e => e.LastForkHeight).HasColumnType("bigint");
            entity.Property(e => e.LastForkHash).HasMaxLength(100);
            
            // Historical timestamp
            entity.Property(e => e.CreatedAt).IsRequired();

            // Indexes for faster queries
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.Name, e.CreatedAt }); // Composite index for name + time queries (most common)
        });
    }
} 