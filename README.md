# BlockchainMonitor - ICMarkets WEB API

A .NET 8 Web API application for monitoring and storing blockchain data from multiple cryptocurrencies using the BlockCypher API.

## 🚀 Project Overview

This application fetches and stores blockchain data from the following BlockCypher APIs:
- **Ethereum Mainnet**: https://api.blockcypher.com/v1/eth/main
- **Dash Mainnet**: https://api.blockcypher.com/v1/dash/main
- **Bitcoin Mainnet**: https://api.blockcypher.com/v1/btc/main
- **Bitcoin Testnet**: https://api.blockcypher.com/v1/btc/test3
- **Litecoin Mainnet**: https://api.blockcypher.com/v1/ltc/main

## 🏗️ Architecture

The project follows **Clean Architecture** principles with SOLID design patterns:

```
BlockchainMonitor/
├── BlockchainMonitor.API/              # Presentation Layer (Web API)
├── BlockchainMonitor.DataFetcher/      # Background Service (Console App)
├── BlockchainMonitor.Application/      # Application Layer (Use Cases)
├── BlockchainMonitor.Domain/           # Domain Layer (Entities, Interfaces)
└── BlockchainMonitor.Infrastructure/   # Infrastructure Layer (Repositories, External Services)
```

### Application Components

- **API Project**: Web service providing REST endpoints for blockchain data
- **DataFetcher Project**: Independent console application for background data fetching
- **Shared Infrastructure**: Both applications share the same Infrastructure layer

## 🎯 Features

### Core Functionality
- ✅ **Clean Architecture** with SOLID principles
- ✅ **Multiple Blockchain Support** (ETH, DASH, BTC, LTC)
- ✅ **Historical Data Storage** with timestamps
- ✅ **RESTful API Endpoints** with Swagger documentation
- ✅ **Health Checks** and CORS policy
- ✅ **Dependency Injection** and logging
- ✅ **Background Data Fetching** via separate console application
- ✅ **Global Exception Handling** middleware
- ✅ **Security Features** (Rate Limiting, CORS, Security Headers)
- ✅ **In-Memory Caching** with configurable durations
- ✅ **Database Indexing** for performance optimization
- ✅ **Shared Mapping Utilities** to eliminate code duplication
- ✅ **Configurable Retry Logic** with exponential backoff

### Design Patterns
- ✅ **Repository Pattern** for data access
- ✅ **Unit of Work Pattern** for transaction management
- ✅ **Background Service Pattern** for data fetching
- ✅ **Dependency Injection** with proper service lifetimes
- ✅ **Caching Pattern** with configurable strategies
- ✅ **Configuration Pattern** for external settings
- ✅ **Retry Pattern** with exponential backoff

### Technical Stack
- **Framework**: .NET 8 Web API
- **Database**: SQLite with Entity Framework Core
- **Caching**: In-Memory Cache with configurable durations
- **Testing**: xUnit, Moq, FluentAssertions
- **Documentation**: Swagger/OpenAPI
- **Deployment**: Docker with multi-stage builds
- **Monitoring**: Health Checks, Logging

## 🛠️ Getting Started

### Prerequisites
- .NET 8 SDK
- Docker (optional)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/KaisZaouali/BlockchainMonitor.git
   cd BlockchainMonitor
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the applications**

   **Start the API:**
   ```bash
   dotnet run --project BlockchainMonitor.API
   ```

   **Start the DataFetcher (in a separate terminal):**
   ```bash
   dotnet run --project BlockchainMonitor.DataFetcher
   ```

4. **Access the API**
   - API: http://localhost:5065
   - Swagger UI: http://localhost:5065/swagger

## 📋 API Documentation

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/blockchain` | Get all blockchain data |
| `GET` | `/api/blockchain/{name}` | Get latest data for specific blockchain |
| `GET` | `/api/blockchain/{name}/history?limit={limit}` | Get historical data for blockchain |
| `GET` | `/api/blockchain/latest` | Get latest data from all blockchains |
| `GET` | `/api/blockchain/total` | Get total number of records |

### Query Parameters

- **`limit`** (optional): Number of historical records to return (1-1000, default: 100)

### Response Format

```json
{
  "name": "ETH.main",
  "height": 23025819,
  "hash": "1b68410803c88fd872035e388f8f878ad21557663a036110ecc141de469d11e9",
  "time": "2025-07-29T15:55:37.89831688Z",
  "latestUrl": "https://api.blockcypher.com/v1/eth/main/blocks/...",
  "previousHash": "00712a677a1c08d602af6c65524e2ea8f06a99f74969b62d5b06a502bd9c8bc9",
  "previousUrl": "https://api.blockcypher.com/v1/eth/main/blocks/...",
  "peerCount": 26,
  "unconfirmedCount": 0,
  "highGasPrice": 15728119101,
  "mediumGasPrice": 11049128640,
  "lowGasPrice": 5750945813,
  "highPriorityFee": 1909740098,
  "mediumPriorityFee": 1223313460,
  "lowPriorityFee": 683834093,
  "baseFee": 2863711524,
  "lastForkHeight": 23020776,
  "lastForkHash": "603730e09af9e3b33a9dff39263e7189745cdca6380ca6f7d826811649a12682",
  "createdAt": "2025-07-29T15:55:37.89831688Z"
}
```

### Security Features

#### **Rate Limiting**
- **API Level**: Built-in ASP.NET Core rate limiting
- **Response**: 429 (Too Many Requests) with proper error message
- **Configuration**: Configurable limits per endpoint

#### **CORS Policy**
- **Allowed Origins**: Specific origins only
- **Methods**: GET, POST, PUT, DELETE
- **Headers**: Content-Type, Authorization
- **Credentials**: Supported

#### **Security Headers**
- **X-Content-Type-Options**: nosniff
- **X-Frame-Options**: DENY
- **X-XSS-Protection**: 1; mode=block
- **Referrer-Policy**: strict-origin-when-cross-origin
- **Permissions-Policy**: geolocation=(), microphone=()

## 📊 Usage Limits & Rate Limiting

### BlockCypher API Limits
BlockCypher provides different rate limits based on your plan:

- **Free Tier**: 3 requests/second, 200 requests/hour
- **Starter**: $0.10 per 1,000 requests
- **Growth**: $0.05 per 1,000 requests
- **Enterprise**: Custom pricing

#### **Application-Level Rate Limiting**
To respect BlockCypher's rate limits, the application implements:

**DataFetcher Service:**
- **Sequential Processing**: Fetches blockchain data one at a time
- **Delay Between Requests**: Configurable delay (default: 1 second)
- **Retry Logic**: Exponential backoff for 429 (Too Many Requests) errors
  - 1st retry: 2 seconds delay
  - 2nd retry: 4 seconds delay
  - 3rd retry: 8 seconds delay
- **Default Interval**: 10 minutes (600 seconds) between fetch cycles

**Configuration:**
```json
{
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
  }
}
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test BlockchainMonitor.Tests.Unit
dotnet test BlockchainMonitor.Tests.Integration
dotnet test BlockchainMonitor.Tests.Functional
```

## 🐳 Docker

```bash
# Build and run with Docker
docker build -t blockchainmonitor .
docker run -p 5065:5065 blockchainmonitor

# Using docker-compose
docker-compose up -d
```

## 📁 Project Structure

```
BlockchainMonitor/
├── BlockchainMonitor.API/                    # Presentation Layer (Web API)
│   ├── Controllers/                          # API Controllers
│   │   └── BlockchainController.cs           # Blockchain data endpoints
│   ├── Middleware/                           # Custom Middleware
│   │   └── ExceptionHandlingMiddleware.cs    # Global exception handling
│   ├── Program.cs                            # Application Entry Point
│   ├── appsettings.json                      # API Configuration
│   └── appsettings.Development.json          # Development Configuration
├── BlockchainMonitor.DataFetcher/            # Background Service (Console App)
│   ├── Services/                             # Data Fetching Services
│   │   ├── DataFetchingService.cs            # Main data fetching logic
│   │   └── BlockchainDataFetchingBackgroundService.cs  # Background service
│   ├── Configuration/                        # Configuration Classes
│   │   └── DataFetchingSettings.cs           # Data fetching configuration
│   ├── Program.cs                            # Console Application Entry Point
│   └── appsettings.json                      # DataFetcher Configuration
├── BlockchainMonitor.Application/            # Application Layer (Use Cases)
│   ├── Services/                             # Application Services
│   │   └── BlockchainService.cs              # Business logic implementation
│   ├── DTOs/                                 # Data Transfer Objects
│   │   └── BlockchainDataDto.cs              # Blockchain data DTO
│   ├── Interfaces/                           # Application Interfaces
│   │   └── IBlockchainService.cs             # Application service contract
│   ├── Exceptions/                           # Custom Exceptions
│   │   └── BlockchainException.cs            # Domain-specific exceptions
│   ├── Configuration/                        # Configuration Classes
│   │   └── CacheSettings.cs                  # Cache configuration
│   ├── Constants/                            # Shared Constants
│   │   └── BlockchainConstants.cs            # Blockchain-related constants
│   ├── Mappers/                              # Shared Mapping Utilities
│   │   └── BlockchainMapper.cs               # Entity-DTO mapping utilities
│   └── DependencyInjection.cs                # Application DI configuration
├── BlockchainMonitor.Domain/                 # Domain Layer (Entities, Interfaces)
│   ├── Entities/                             # Domain Entities
│   │   └── BlockchainData.cs                 # Core blockchain data entity
│   └── Interfaces/                           # Domain Interfaces
│       ├── IRepository.cs                    # Generic repository interface
│       ├── IBlockchainRepository.cs          # Blockchain-specific repository
│       └── IUnitOfWork.cs                    # Unit of Work interface
└── BlockchainMonitor.Infrastructure/         # Infrastructure Layer (Data Access, External Services)
    ├── Data/                                 # Database Context
    │   └── BlockchainDbContext.cs            # EF Core DbContext with indexes
    ├── Repositories/                         # Data Access Implementation
    │   ├── Repository.cs                     # Generic repository implementation
    │   ├── BlockchainRepository.cs           # Blockchain repository implementation
    │   └── UnitOfWork.cs                     # Unit of Work implementation
    ├── Services/                             # External Services
    │   └── BlockCypherService.cs             # BlockCypher API client with retry logic
    ├── Interfaces/                           # Infrastructure Interfaces
    │   ├── IBlockCypherService.cs            # BlockCypher service contract
    │   └── IDataFetchingService.cs           # Data fetching service contract
    ├── Configuration/                        # Configuration Classes
    │   └── RetrySettings.cs                  # Retry configuration settings
    ├── Migrations/                           # EF Core Migrations
    │   ├── BlockchainDbContextModelSnapshot.cs
    │   ├── 20250729220540_InitialCreate.cs   # Initial database migration
    │   └── 20250729232313_AddCompositeIndexForNameAndCreatedAt.cs  # Performance indexes
    └── DependencyInjection.cs                # Infrastructure DI configuration
```

## 🔧 Configuration

### Database Location
The SQLite database (`blockchain.db`) is stored in the **solution root directory** for easy access and management.

### JSON Property Mapping
The application uses `JsonPropertyName` attributes to map BlockCypher API's snake_case JSON properties to C# PascalCase properties:

```csharp
[JsonPropertyName("latest_url")]
public string? LatestUrl { get; set; }

[JsonPropertyName("peer_count")]
public int PeerCount { get; set; }
```

This ensures proper deserialization of all BlockCypher API fields including:
- `latest_url`, `previous_hash`, `previous_url`
- `peer_count`, `unconfirmed_count`
- `high_fee_per_kb`, `medium_fee_per_kb`, `low_fee_per_kb`
- `high_gas_price`, `medium_gas_price`, `low_gas_price`
- `high_priority_fee`, `medium_priority_fee`, `low_priority_fee`
- `base_fee`, `last_fork_height`, `last_fork_hash`

### Application Settings

**API Configuration** (`BlockchainMonitor.API/appsettings.json`):
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
  "RetrySettings": {
    "MaxRetryAttempts": 3,
    "RetryDelayMs": 2000
  }
}
```

**DataFetcher Configuration** (`BlockchainMonitor.DataFetcher/appsettings.json`):
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
  }
}
```

### Performance Optimizations

#### **Database Indexes**
The application includes optimized database indexes for better query performance:

```sql
-- Single column indexes
CREATE INDEX "IX_BlockchainData_Name" ON "BlockchainData" ("Name");
CREATE INDEX "IX_BlockchainData_CreatedAt" ON "BlockchainData" ("CreatedAt");

-- Composite index for common queries
CREATE INDEX "IX_BlockchainData_Name_CreatedAt" ON "BlockchainData" ("Name", "CreatedAt");
```

#### **Caching Strategy**
The application implements intelligent caching with configurable durations:

- **All Blockchain Data**: 2 minutes (frequently accessed)
- **Latest Blockchain Data**: 1 minute (real-time data)
- **Blockchain History**: 5 minutes (historical data)
- **Latest Data (All)**: 2 minutes (dashboard data)
- **Total Records**: 10 minutes (count data)

#### **Retry Strategy**
The application implements configurable retry logic with exponential backoff:

- **MaxRetryAttempts**: Configurable retry count (default: 3)
- **RetryDelayMs**: Base delay between retries (default: 2000ms)
- **Exponential Backoff**: Delay increases with each retry attempt
- **429 Handling**: Special handling for rate limit errors

### Shared Utilities

#### **BlockchainMapper**
Centralized mapping utilities to eliminate code duplication:

```csharp
// Shared mapping methods
BlockchainMapper.MapToDto(entity)    // Entity → DTO
BlockchainMapper.MapToEntity(dto)    // DTO → Entity
```

#### **BlockchainConstants**
Shared constants for consistent behavior:

```csharp
public static class BlockchainConstants
{
    public const int MaxHistoryLimit = 1000;
    public const int DefaultHistoryLimit = 100;
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [BlockCypher](https://www.blockcypher.com/) for providing the blockchain APIs
- [Microsoft](https://dotnet.microsoft.com/) for .NET framework
- [Entity Framework](https://docs.microsoft.com/en-us/ef/) for data access

## 📋 Current Status

### ✅ Completed Features
- ✅ Clean Architecture implementation
- ✅ Repository and Unit of Work patterns
- ✅ Background data fetching service
- ✅ RESTful API with Swagger documentation
- ✅ Health checks and CORS policy
- ✅ Global exception handling
- ✅ JSON property mapping for BlockCypher API
- ✅ SQLite database with EF Core
- ✅ Dependency injection with proper service lifetimes
- ✅ **In-Memory Caching** with configurable durations
- ✅ **Database Indexing** for performance optimization
- ✅ **Shared Mapping Utilities** to eliminate code duplication
- ✅ **Configuration Management** for cache and processing settings
- ✅ **Input Validation** with model validation attributes
- ✅ **Security Headers** and HTTPS redirection
- ✅ **Rate Limiting** with proper 429 error responses
- ✅ **Configurable Retry Logic** with exponential backoff

### 🚧 Pending Features
- ⏳ Unit, Integration, and Functional tests
- ⏳ Docker containerization
- ⏳ API Gateway implementation
- ⏳ Advanced monitoring and logging
- ⏳ Performance benchmarking

---

**Note**: This project was developed as part of the ICMarkets WEB API Developer Project to demonstrate clean architecture, SOLID principles, and best practices in .NET development.
