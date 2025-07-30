using BlockchainMonitor.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BlockchainMonitor.Infrastructure.Services;

/// <summary>
/// RabbitMQ-based event publisher implementation
/// </summary>
public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private const string ExchangeName = "blockchain_events";

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        var rabbitMqConfig = configuration.GetSection("RabbitMQ");
        var hostName = rabbitMqConfig["HostName"] ?? "localhost";
        var userName = rabbitMqConfig["UserName"] ?? "guest";
        var password = rabbitMqConfig["Password"] ?? "guest";
        var port = int.TryParse(rabbitMqConfig["Port"], out var portValue) ? portValue : 5672;

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = port
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Declare exchange
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: true);
        
        _logger.LogInformation("RabbitMQ Event Publisher initialized");
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType}", typeof(TEvent).Name);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
} 