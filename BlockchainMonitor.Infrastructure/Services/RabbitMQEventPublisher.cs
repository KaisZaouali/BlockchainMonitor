using BlockchainMonitor.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BlockchainMonitor.Infrastructure.Services;

/// <summary>
/// RabbitMQ-based event publisher implementation with lazy connection
/// </summary>
public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly ConnectionFactory _factory;
    private const string ExchangeName = "blockchain_events";
    private readonly object _lock = new object();
    private bool _disposed = false;

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        var rabbitMqConfig = configuration.GetSection("RabbitMQ");
        var hostName = rabbitMqConfig["HostName"] ?? "localhost";
        var userName = rabbitMqConfig["UserName"] ?? "guest";
        var password = rabbitMqConfig["Password"] ?? "guest";
        var port = int.TryParse(rabbitMqConfig["Port"], out var portValue) ? portValue : 5672;

        _factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = port,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            RequestedHeartbeat = TimeSpan.FromSeconds(30)
        };

        _logger.LogInformation("RabbitMQ Event Publisher initialized (lazy connection)");
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        if (_disposed) return;

        try
        {
            EnsureConnection();
            
            if (_channel?.IsOpen == true)
            {
                var eventType = typeof(TEvent).Name;
                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: string.Empty,
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("Published event {EventType}: {Message}", eventType, message);
            }
            else
            {
                _logger.LogWarning("RabbitMQ not connected, skipping event publication for {EventType}", typeof(TEvent).Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType}", typeof(TEvent).Name);
            // Don't throw to avoid breaking the data fetching process
        }
    }

    private void EnsureConnection()
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
        {
            return; // Already connected
        }

        lock (_lock)
        {
            if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            {
                return; // Double-check after lock
            }

            try
            {
                // Dispose old connection if exists
                _channel?.Dispose();
                _connection?.Dispose();

                // Create new connection
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                // Declare exchange
                _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: true);
                
                _logger.LogInformation("RabbitMQ connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ. Will retry on next publish attempt.");
                
                // Clean up failed connection
                _channel?.Dispose();
                _connection?.Dispose();
                _channel = null;
                _connection = null;
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;

            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
} 