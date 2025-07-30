# Blockchain Monitor

A comprehensive blockchain monitoring system built with .NET 8, featuring real-time data fetching, caching, and event-driven architecture.

## 🚀 Features

- **Multi-Blockchain Support**: Monitor Ethereum, Dash, Bitcoin, Bitcoin Testnet, and Litecoin
- **Real-Time Data Fetching**: Background service fetches data from BlockCypher API
- **Intelligent Caching**: In-memory caching with configurable durations
- **Event-Driven Architecture**: RabbitMQ-based event sourcing for cache invalidation
- **RESTful API**: Clean, documented endpoints with Swagger
- **Rate Limiting**: Built-in protection against API abuse
- **Global Exception Handling**: Centralized error management
- **Database Indexing**: Optimized queries with composite indexes
- **Clean Architecture**: Separation of concerns across layers

## 📋 Prerequisites

- .NET 8.0 SDK
- SQLite (included)
- RabbitMQ Server (for event sourcing)

### Installing RabbitMQ

**macOS:**
```bash
brew install rabbitmq
brew services start rabbitmq
```

**Windows:**
Download from [RabbitMQ Downloads](https://www.rabbitmq.com/download.html)

**Linux:**
```bash
sudo apt-get install rabbitmq-server
sudo systemctl start rabbitmq-server
```

## 🏗️ Project Structure

```
BlockchainMonitor/
├── BlockchainMonitor.API/           # Web API layer
│   ├── Controllers/                 # REST API endpoints
│   ├── Services/                    # Background services
│   │   └── BlockchainDataCreatedConsumer.cs  # RabbitMQ event consumer
│   ├── Middleware/                  # Global exception handling
│   └── Program.cs                   # API configuration
├── BlockchainMonitor.Application/    # Application layer
│   ├── DTOs/                       # Data transfer objects
│   ├── Interfaces/                  # Service contracts
│   ├── Services/                    # Business logic
│   ├── Configuration/               # Settings classes
│   ├── Constants/                   # Shared constants
│   ├── Mappers/                     # Entity-DTO mapping
│   └── DependencyInjection.cs       # DI configuration
├── BlockchainMonitor.Domain/        # Domain layer
│   ├── Entities/                    # Domain entities
│   ├── Events/                      # Domain events
│   │   └── BlockchainDataCreatedEvent.cs
│   └── Interfaces/                  # Domain contracts
│       └── IEventPublisher.cs
├── BlockchainMonitor.Infrastructure/ # Infrastructure layer
│   ├── Data/                       # Entity Framework context
│   ├── Repositories/                # Data access
│   ├── Services/                    # External services
│   │   └── RabbitMQEventPublisher.cs
│   └── DependencyInjection.cs       # DI configuration
└── BlockchainMonitor.DataFetcher/   # Background data fetching
    └── Program.cs                   # Console application
```

## 🔧 Configuration

### API Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../blockchain.db"
  },
  "CacheSettings": {
    "DefaultDurationMinutes": 5,
    "AllBlockchainDataDurationMinutes": 2,
    "LatestBlockchainDataDurationMinutes": 1,
    "BlockchainHistoryDurationMinutes": 5,
    "LatestDataDurationMinutes": 2,
    "TotalRecordsDurationMinutes": 10
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```

### DataFetcher Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blockchain.db"
  },
  "BlockCypherApi": {
    "BaseUrl": "https://api.blockcypher.com/v1",
    "Endpoints": {
      "Ethereum": "/eth/main",
      "Dash": "/dash/main",
      "Bitcoin": "/btc/main",
      "BitcoinTest": "/btc/test3",
      "Litecoin": "/ltc/main"
    }
  },
  "BlockchainDataFetching": {
    "Enabled": true,
    "IntervalSeconds": 60
  },
  "DataFetching": {
    "RequestDelayMs": 1000
  },
  "RetrySettings": {
    "MaxRetryAttempts": 3,
    "RetryDelayMs": 2000
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```

## 🚀 Getting Started

### 1. Start RabbitMQ

```bash
# macOS
brew services start rabbitmq

# Verify it's running
rabbitmqctl status
```

### 2. Run Database Migrations

```bash
cd BlockchainMonitor.API
dotnet ef database update
```

### 3. Start the DataFetcher (Background Service)

```bash
cd BlockchainMonitor.DataFetcher
dotnet run
```

### 4. Start the API

```bash
cd BlockchainMonitor.API
dotnet run
```

### 5. Access the API

- **Swagger UI**: http://localhost:5000/swagger
- **RabbitMQ Management**: http://localhost:15672

## 📡 API Endpoints

### Blockchain Data

- `GET /api/blockchain` - Get all blockchain data
- `GET /api/blockchain/{name}` - Get latest data for specific blockchain
- `GET /api/blockchain/{name}/history?limit={limit}` - Get historical data
- `GET /api/blockchain/latest` - Get latest data for all blockchains

### Parameters

- `name`: Blockchain name (eth, dash, btc, btc-test3, ltc)
- `limit`: Number of historical records (default: 100, max: 1000)

## 🔄 Event Sourcing Architecture

### Overview

The system uses RabbitMQ for event-driven cache invalidation between the DataFetcher and API processes.

### Event Flow

```
DataFetcher → BlockchainService → RabbitMQEventPublisher → RabbitMQ Exchange
                                                                        ↓
API → BlockchainDataCreatedConsumer → IBlockchainService → Cache Invalidation
```

### Components

#### Event Publisher (`RabbitMQEventPublisher`)
- **Location**: `BlockchainMonitor.Infrastructure/Services/`
- **Purpose**: Publishes `BlockchainDataCreatedEvent` to RabbitMQ
- **Exchange**: `blockchain_events` (Fanout)
- **Triggered**: When new blockchain data is created

#### Event Consumer (`BlockchainDataCreatedConsumer`)
- **Location**: `BlockchainMonitor.API/Services/`
- **Purpose**: Consumes events and invalidates related caches
- **Queue**: `api_cache_invalidation`
- **Behavior**: Background service that listens for events

#### Domain Event (`BlockchainDataCreatedEvent`)
- **Location**: `BlockchainMonitor.Domain/Events/`
- **Purpose**: Represents blockchain data creation
- **Properties**: `BlockchainName`, `OccurredOn`

### RabbitMQ Configuration

- **Exchange**: `blockchain_events` (Fanout, Durable)
- **Queue**: `api_cache_invalidation` (Durable, Non-exclusive)
- **Protocol**: AMQP 0-9-1
- **Port**: 5672 (default)

### Cache Invalidation Strategy

When new blockchain data is created:

1. **DataFetcher** creates new record in database
2. **BlockchainService** publishes `BlockchainDataCreatedEvent`
3. **RabbitMQEventPublisher** sends event to exchange
4. **BlockchainDataCreatedConsumer** receives event
5. **IBlockchainService.InvalidateRelatedCaches** removes stale cache entries:
   - All blockchain data cache
   - Latest data cache
   - Specific blockchain latest data cache
   - Blockchain history cache

### Benefits

- **Cross-Process Communication**: Events work across separate processes
- **Reliable**: RabbitMQ provides message persistence and acknowledgment
- **Scalable**: Multiple consumers can handle events
- **Decoupled**: DataFetcher doesn't need to know about API cache
- **Fault-Tolerant**: Messages are acknowledged and can be retried

## 🗄️ Database Schema

### BlockchainData Entity

```csharp
public class BlockchainData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Height { get; set; }
    public string Hash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### Indexes

- **Primary Key**: `Id` (Auto-increment)
- **Composite Index**: `Name + CreatedAt` (for efficient history queries)

## 🔧 Performance Optimizations

### Caching Strategy

- **In-Memory Caching**: Using `IMemoryCache`
- **Configurable Durations**: Different cache times for different data types
- **Smart Invalidation**: Event-driven cache invalidation
- **Cache Key Functions**: Centralized key generation

### Database Optimizations

- **Composite Indexes**: Optimized for common query patterns
- **Efficient Queries**: Single queries for latest data per blockchain
- **Connection Pooling**: Entity Framework connection management

### API Optimizations

- **Rate Limiting**: 10 requests per minute per user
- **Global Exception Handling**: Centralized error management
- **Security Headers**: Enhanced security with HTTP headers
- **CORS Policy**: Configured for specific origins

## 🛡️ Security Features

- **Rate Limiting**: Protection against API abuse
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, etc.
- **CORS Policy**: Configured with specific allowed origins
- **Input Validation**: Comprehensive parameter validation
- **Exception Handling**: Secure error responses

## 📊 Monitoring

### RabbitMQ Management UI

- **URL**: http://localhost:15672
- **Default Credentials**: guest/guest
- **Features**: Monitor queues, exchanges, connections, and message rates

### Logging

- **Structured Logging**: Using Microsoft.Extensions.Logging
- **Event Tracking**: Log all cache invalidations and data creation
- **Error Tracking**: Comprehensive error logging with stack traces

## 🔄 Usage Limits

### BlockCypher API

- **Rate Limits**: Varies by endpoint and plan
- **Retry Strategy**: Exponential backoff with configurable attempts
- **Error Handling**: Graceful degradation on API failures

### Application Limits

- **History Limit**: Maximum 1000 records per request
- **Cache Duration**: Configurable per data type
- **Rate Limiting**: 10 requests per minute per user

## 🚀 Deployment

### Prerequisites

1. **RabbitMQ Server**: Running and accessible
2. **Database**: SQLite file with proper permissions
3. **Configuration**: Valid appsettings.json files

### Environment Variables

```bash
# RabbitMQ Configuration
RABBITMQ__HOSTNAME=localhost
RABBITMQ__USERNAME=guest
RABBITMQ__PASSWORD=guest
RABBITMQ__PORT=5672

# Database Connection
CONNECTIONSTRINGS__DEFAULTCONNECTION=Data Source=blockchain.db
```

### Docker Support

```dockerfile
# RabbitMQ
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
