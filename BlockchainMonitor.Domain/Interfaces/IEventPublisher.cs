namespace BlockchainMonitor.Domain.Interfaces;

/// <summary>
/// Interface for publishing domain events
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a domain event
    /// </summary>
    /// <param name="event">The domain event to publish</param>
    void Publish<TEvent>(TEvent @event) where TEvent : class;
} 