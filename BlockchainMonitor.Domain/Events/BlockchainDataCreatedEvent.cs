namespace BlockchainMonitor.Domain.Events;

/// <summary>
/// Domain event raised when new blockchain data is created
/// </summary>
public class BlockchainDataCreatedEvent
{
    public string BlockchainName { get; }
    public DateTime OccurredOn { get; }

    public BlockchainDataCreatedEvent(string blockchainName)
    {
        BlockchainName = blockchainName ?? throw new ArgumentNullException(nameof(blockchainName));
        OccurredOn = DateTime.UtcNow;
    }
} 