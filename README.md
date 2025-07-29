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
├── BlockchainMonitor.Application/       # Application Layer (Use Cases)
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

### Design Patterns
- ✅ **Repository Pattern** for data access
- ✅ **Unit of Work Pattern** for transaction management
- ✅ **Background Service Pattern** for data fetching
- ✅ **Dependency Injection** with proper service lifetimes

### Technical Stack
- **Framework**: .NET 8 Web API
- **Database**: SQLite with Entity Framework Core
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
   - Health Check: http://localhost:5065/health

## 📚 API Documentation

The API provides the following endpoints:

### Blockchain Data Endpoints
- `GET /api/blockchain` - Get all blockchain data (latest entries)
- `GET /api/blockchain/latest` - Get latest blockchain data for all types
- `GET /api/blockchain/{blockchainName}` - Get latest data for specific blockchain (e.g., "ETH.main", "BTC.main")
- `GET /api/blockchain/{blockchainName}/history` - Get historical data for specific blockchain

### System Endpoints
- `GET /health` - Health check endpoint
- `GET /` - Root endpoint

### Security Features
- **Rate Limiting**: 10 requests per minute per client (429 error when exceeded)
- **CORS Policy**: Restricted to specific origins (localhost:3000, 4200, 8080)
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
- **HTTPS Redirection**: Automatic HTTP to HTTPS redirection
- **Input Validation**: Model validation and parameter sanitization

### Example Usage
```bash
# Get all blockchain data
curl http://localhost:5065/api/blockchain

# Get latest Ethereum data
curl http://localhost:5065/api/blockchain/ETH.main

# Get Ethereum history
curl http://localhost:5065/api/blockchain/ETH.main/history
```

## ⚠️ Usage Limits & Rate Limiting

### BlockCypher API Limits
The application integrates with BlockCypher's free API tier, which has the following limitations:

#### **Rate Limits**
- **Free Tier**: 3 requests per second (3 req/sec)
- **Burst Limit**: Up to 10 requests in a short burst
- **Daily Limit**: 200 requests per day per IP address

#### **Supported Blockchains**
- ✅ **Ethereum Mainnet** (`eth/main`)
- ✅ **Dash Mainnet** (`dash/main`)
- ✅ **Bitcoin Mainnet** (`btc/main`)
- ✅ **Bitcoin Testnet** (`btc/test3`)
- ✅ **Litecoin Mainnet** (`ltc/main`)

#### **Application-Level Rate Limiting**
To respect BlockCypher's rate limits, the application implements:

**DataFetcher Service:**
- **Sequential Processing**: Fetches blockchain data one at a time
- **Delay Between Requests**: 1-second delay between each blockchain fetch
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
    "IntervalSeconds": 300  // 5 minutes
  }
}
```

#### **Error Handling**
The application handles rate limiting gracefully:
- **429 Errors**: Automatic retry with exponential backoff
- **Logging**: All rate limit events are logged for monitoring
- **Graceful Degradation**: Continues operation even if some requests fail

#### **Monitoring Usage**
To monitor your API usage:
1. Check the application logs for rate limit warnings
2. Monitor the `BlockchainDataFetching` service logs
3. Review BlockCypher's dashboard (if you have an account)

#### **Best Practices**
- **Production Use**: Consider upgrading to BlockCypher's paid tiers for higher limits
- **Monitoring**: Set up alerts for rate limit warnings
- **Backup Plans**: Implement fallback data sources for critical applications
- **Caching**: The application stores historical data to reduce API calls

#### **Upgrade Options**
For higher usage limits, consider BlockCypher's paid tiers:
- **Starter**: $0.10 per 1,000 requests
- **Growth**: $0.05 per 1,000 requests
- **Enterprise**: Custom pricing

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
    │   └── BlockchainDbContext.cs            # EF Core DbContext
    ├── Repositories/                         # Data Access Implementation
    │   ├── Repository.cs                     # Generic repository implementation
    │   ├── BlockchainRepository.cs           # Blockchain repository implementation
    │   └── UnitOfWork.cs                     # Unit of Work implementation
    ├── Services/                             # External Services
    │   └── BlockCypherService.cs             # BlockCypher API client
    ├── Interfaces/                           # Infrastructure Interfaces
    │   ├── IBlockCypherService.cs            # BlockCypher service contract
    │   └── IDataFetchingService.cs           # Data fetching service contract
    ├── Migrations/                           # EF Core Migrations
    │   ├── BlockchainDbContextModelSnapshot.cs
    │   └── 20250729220540_InitialCreate.cs   # Initial database migration
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
    "DefaultConnection": "Data Source=blockchain.db"
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
    "BaseUrl": "https://api.blockcypher.com/v1"
  },
  "BlockchainDataFetching": {
    "Enabled": true,
    "IntervalSeconds": 600
  }
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

### 🚧 Pending Features
- ⏳ Unit, Integration, and Functional tests
- ⏳ Docker containerization
- ⏳ API Gateway implementation
- ⏳ Performance optimizations
- ⏳ Monitoring and logging enhancements

---

**Note**: This project was developed as part of the ICMarkets WEB API Developer Project to demonstrate clean architecture, SOLID principles, and best practices in .NET development.
