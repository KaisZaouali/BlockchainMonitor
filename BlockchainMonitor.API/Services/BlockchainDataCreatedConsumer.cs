using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BlockchainMonitor.API.Services;

/// <summary>
/// RabbitMQ event consumer for blockchain data creation events
/// </summary>
public class BlockchainDataCreatedConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BlockchainDataCreatedConsumer> _logger;
    private const string ExchangeName = "blockchain_events";
    private const string QueueName = "api_cache_invalidation";

    public BlockchainDataCreatedConsumer(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BlockchainDataCreatedConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
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

        // Declare queue
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);

        // Bind queue to exchange
        _channel.QueueBind(QueueName, ExchangeName, routingKey: string.Empty);

        _logger.LogInformation("BlockchainDataCreatedConsumer initialized");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message: {Message}", message);

                // Deserialize and handle the event
                var eventData = JsonSerializer.Deserialize<BlockchainDataCreatedEvent>(message);
                if (eventData != null)
                {
                    await HandleBlockchainDataCreatedEvent(eventData);
                }

                // Acknowledge the message
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                // Reject the message
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task HandleBlockchainDataCreatedEvent(BlockchainDataCreatedEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling BlockchainDataCreatedEvent for blockchain: {BlockchainName}", @event.BlockchainName);

            // Create a scope to resolve scoped services
            using var scope = _serviceScopeFactory.CreateScope();
            var blockchainService = scope.ServiceProvider.GetRequiredService<IBlockchainService>();

            // Invalidate related caches
            await blockchainService.InvalidateRelatedCaches(@event.BlockchainName);

            _logger.LogInformation("Successfully invalidated caches for blockchain: {BlockchainName}", @event.BlockchainName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling BlockchainDataCreatedEvent for blockchain: {BlockchainName}", @event.BlockchainName);
            throw;
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
