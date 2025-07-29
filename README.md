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
├── BlockchainMonitor.API/          # Presentation Layer
├── BlockchainMonitor.Application/   # Application Layer (Use Cases)
├── BlockchainMonitor.Domain/       # Domain Layer (Entities, Interfaces)
└── BlockchainMonitor.Infrastructure/ # Infrastructure Layer (Repositories, External Services)
```

## 🎯 Features

### Core Functionality
- ✅ **Clean Architecture** with SOLID principles
- ✅ **Multiple Blockchain Support** (ETH, DASH, BTC, LTC)
- ✅ **Historical Data Storage** with timestamps
- ✅ **RESTful API Endpoints** with Swagger documentation
- ✅ **Health Checks** and CORS policy
- ✅ **Dependency Injection** and logging

### Design Patterns
- ✅ **Repository Pattern** for data access
- ✅ **Unit of Work Pattern** for transaction management
- ✅ **CQRS Pattern** for command/query separation
- ✅ **Event Sourcing** for audit trails

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

3. **Run the application**
   ```bash
   dotnet run --project BlockchainMonitor.API
   ```

4. **Access the API**
   - API: http://localhost:5065
   - Swagger UI: http://localhost:5065/swagger

## 📚 API Documentation

The API provides the following endpoints:

- `GET /api/blockchain/eth` - Get Ethereum blockchain data
- `GET /api/blockchain/dash` - Get Dash blockchain data
- `GET /api/blockchain/btc` - Get Bitcoin blockchain data
- `GET /api/blockchain/btc-test` - Get Bitcoin testnet data
- `GET /api/blockchain/ltc` - Get Litecoin blockchain data
- `GET /health` - Health check endpoint

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
├── BlockchainMonitor.API/              # Web API Controllers
│   ├── Controllers/                   # API Controllers
│   ├── Middleware/                    # Custom Middleware
│   └── Program.cs                     # Application Entry Point
├── BlockchainMonitor.Application/      # Application Services
│   ├── Services/                      # Application Services
│   ├── DTOs/                         # Data Transfer Objects
│   └── Interfaces/                    # Application Interfaces
├── BlockchainMonitor.Domain/          # Domain Models
│   ├── Entities/                      # Domain Entities
│   ├── Interfaces/                    # Domain Interfaces
│   └── ValueObjects/                  # Value Objects
├── BlockchainMonitor.Infrastructure/  # Infrastructure Layer
│   ├── Repositories/                  # Data Access
│   ├── Services/                      # External Services
│   └── Data/                         # Database Context
└── Tests/                            # Test Projects
    ├── BlockchainMonitor.Tests.Unit/
    ├── BlockchainMonitor.Tests.Integration/
    └── BlockchainMonitor.Tests.Functional/
```

## 🔧 Configuration

### Database Location
The SQLite database (`blockchain.db`) is stored in the **solution root directory** for easy access and management.

### Application Settings
The application uses `appsettings.json` for configuration:

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

---

**Note**: This project was developed as part of the ICMarkets WEB API Developer Project to demonstrate clean architecture, SOLID principles, and best practices in .NET development.
