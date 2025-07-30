# Blockchain Monitor

A comprehensive blockchain monitoring system built with .NET 8, featuring real-time data fetching, caching, and event-driven architecture.

## 🚀 Features

- **Multi-Blockchain Support**: Monitor Ethereum, Dash, Bitcoin, Bitcoin Testnet, and Litecoin
- **Real-Time Data Fetching**: Background service fetches data from BlockCypher API
- **API Gateway**: YARP-based reverse proxy with load balancing and rate limiting
- **Scaled Architecture**: API instances scaled to 2 for high availability
- **Intelligent Caching**: In-memory caching with configurable durations
- **Event-Driven Architecture**: RabbitMQ-based event sourcing for cache invalidation
- **RESTful API**: Clean, documented endpoints with Swagger
- **Rate Limiting**: Built-in protection against API abuse
- **Global Exception Handling**: Centralized error management
- **Database Indexing**: Optimized queries with composite indexes
- **Clean Architecture**: Separation of concerns across layers
- **Comprehensive Testing**: Unit, integration, and E2E tests with high coverage

## 📋 Prerequisites

- .NET 8.0 SDK
- SQLite (included)
- RabbitMQ Server (for event sourcing)
- Docker & Docker Compose (for containerized deployment)

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

## 🐳 Docker Deployment

The application is fully containerized with Docker and Docker Compose. All Docker-related files are organized in the `docker/` folder.

> 📖 **For detailed Docker documentation, see [docker/DOCKER.md](docker/DOCKER.md)**

### Quick Start

**Development Environment:**
```bash
# Quick start (recommended)
cd docker && ./start-dev.sh

# Or manually:
docker-compose -f docker/docker-compose.yml up -d

# View logs
docker-compose -f docker/docker-compose.yml logs -f

# Stop services
docker-compose -f docker/docker-compose.yml down
```

**Production Environment:**
```bash
# Quick start (recommended)
cd docker && ./start-prod.sh

# Or manually:
docker-compose -f docker/docker-compose.prod.yml up -d

# View logs
docker-compose -f docker/docker-compose.prod.yml logs -f

# Stop services
docker-compose -f docker/docker-compose.prod.yml down
```

### Services Overview

The Docker setup includes:

1. **RabbitMQ**: Message broker for event-driven architecture
2. **Migrate**: Database migration service (runs once on startup)
3. **API Gateway**: YARP-based reverse proxy with load balancing
4. **API**: REST API service with health checks (scaled to 2 instances)
5. **DataFetcher**: Background service for blockchain data fetching

### Environment Variables

**Development:**
- Database: `blockchain.db` (shared volume)
- Gateway Ports: `5003:80`, `5004:443`
- API Ports: Internal only (scaled to 2 instances)
- RabbitMQ Ports: `5672:5672`, `15672:15672`

**Production:**
- Database: `blockchain-prod.db` (shared volume)
- Gateway Ports: `80:80`, `443:443`
- API Ports: Internal only (scaled to 2 instances)
- RabbitMQ Ports: `5672:5672`, `15672:15672`

### Configuration

**Environment Variables (docker-compose.yml):**
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Data Source=/app/data/blockchain.db
  - RABBITMQ__HOSTNAME=rabbitmq
  - RABBITMQ__USERNAME=guest
  - RABBITMQ__PASSWORD=guest
  - RABBITMQ__PORT=5672
```

**Production Environment Variables:**
```bash
# Set these before running production
export RABBITMQ_USER=your_rabbitmq_user
export RABBITMQ_PASS=your_rabbitmq_password
```

### Health Checks

All services include health checks:
- **Gateway**: `curl -f http://localhost:5003/health`
- **API**: `curl -f http://localhost/health`
- **DataFetcher**: Process monitoring
- **RabbitMQ**: Connection ping

### Data Persistence

- **Database**: Stored in Docker volume `blockchain_data`
- **RabbitMQ**: Stored in Docker volume `rabbitmq_data`

### Troubleshooting

**View service logs:**
```bash
# All services
docker-compose -f docker/docker-compose.yml logs

# Specific service
docker-compose -f docker/docker-compose.yml logs gateway
docker-compose -f docker/docker-compose.yml logs api
docker-compose -f docker/docker-compose.yml logs datafetcher
```

**Rebuild containers:**
```bash
docker-compose -f docker/docker-compose.yml down
docker-compose -f docker/docker-compose.yml up -d --build
```

**Reset database:**
```bash
docker-compose -f docker/docker-compose.yml down -v
docker-compose -f docker/docker-compose.yml up -d
```

### Production Considerations

1. **Security**: Change default RabbitMQ credentials
2. **Resources**: Production compose includes resource limits
3. **Monitoring**: Enable logging and monitoring
4. **Backup**: Regular database backups from Docker volumes

## 🧪 Testing

The application includes comprehensive testing across three levels:

### **Unit Tests** (`BlockchainMonitor.Tests.Unit`)
- **Purpose**: Test individual components in isolation
- **Coverage**: Services, repositories, business logic
- **Tools**: xUnit, Moq, FluentAssertions
- **Run**: `dotnet test BlockchainMonitor.Tests.Unit`

### **Integration Tests** (`BlockchainMonitor.Tests.Integration`)
- **Purpose**: Test component interactions and API endpoints
- **Coverage**: Controllers, database operations, service integration
- **Tools**: xUnit, Microsoft.AspNetCore.Mvc.Testing, EF Core InMemory
- **Run**: `dotnet test BlockchainMonitor.Tests.Integration`

### **End-to-End Tests** (`BlockchainMonitor.Tests.E2E`)
- **Purpose**: Test complete user workflows and system behavior
- **Coverage**: Full API workflows, browser interactions
- **Tools**: xUnit, Playwright, Selenium WebDriver
- **Run**: `dotnet test BlockchainMonitor.Tests.E2E`

### **Running Tests**

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test BlockchainMonitor.Tests.Unit
dotnet test BlockchainMonitor.Tests.Integration
# Docker containers for Api and Datafetcher should be already running and have collected some data
dotnet test BlockchainMonitor.Tests.E2E

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run with specific filter
dotnet test --filter "FullyQualifiedName~BlockchainData"
```

### **Test Structure**

```
Tests/
├── Unit/
│   ├── Services/           # Service layer tests
│   ├── Repositories/       # Repository tests
│   └── TestBase.cs         # Base test class
├── Integration/
│   ├── Controllers/        # API controller tests
│   ├── Services/           # Integration service tests
│   └── TestBase.cs         # Base test class
└── E2E/
    ├── API/               # API E2E tests
    ├── UI/                # UI E2E tests
    └── Workflows/         # Complete workflow tests
```

### **Test Configuration**

- **Unit Tests**: Use mocking for external dependencies
- **Integration Tests**: Use in-memory database and test containers
- **E2E Tests**: Use real browser automation and API testing

## 🏗️ Project Structure

```
BlockchainMonitor/
├── docker/                          # Docker configuration
│   ├── docker-compose.yml          # Development environment
│   ├── docker-compose.prod.yml     # Production environment
│   ├── Dockerfile                   # Unified container definition
│   ├── BlockchainMonitor.Gateway/  # API Gateway Dockerfile
│   ├── start-dev.sh                # Development startup script
│   ├── start-prod.sh               # Production startup script
│   └── docker.md                   # Comprehensive Docker documentation
├── BlockchainMonitor.Gateway/       # API Gateway layer
│   ├── Controllers/                 # Gateway health endpoints
│   ├── Middleware/                  # Gateway middleware
│   └── Program.cs                   # Gateway configuration
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
└── BlockchainMonitor.Tests.Unit/    # Unit tests
    ├── Services/                    # Service layer tests
    └── TestBase.cs                  # Base test class
└── BlockchainMonitor.Tests.Integration/ # Integration tests
    ├── Controllers/                 # API controller tests
    └── TestBase.cs                  # Base test class
└── BlockchainMonitor.Tests.E2E/    # End-to-end tests
    └── API/                        # API E2E tests

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
    "DefaultConnection": "Data Source=../blockchain.db"
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
  "DataFetching": {
    "RequestDelayMs": 1000,
    "Enabled": true,
    "IntervalSeconds": 60
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

- **API Gateway**: http://localhost:5003
- **Swagger UI**: http://localhost:5003/swagger
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
