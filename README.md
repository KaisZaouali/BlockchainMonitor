# 🏦 BlockchainMonitor

A comprehensive blockchain monitoring system built with .NET 8, featuring real-time data fetching, caching, event-driven architecture, and advanced monitoring capabilities.

## ✨ Features

- **🏗️ Clean Architecture**: Separation of concerns across layers
- **📊 Real-time Data**: Live blockchain data fetching and monitoring
- **💾 Intelligent Caching**: Memory-based caching with configurable expiration
- **🔄 Event-Driven**: RabbitMQ-based event publishing and consumption
- **🚪 API Gateway**: YARP-based reverse proxy with load balancing
- **⚡ Rate Limiting**: Built-in request throttling and protection
- **🔒 Security**: HTTPS redirection and security headers
- **📈 Redis-Based Metrics**: Real-time metrics collection with Redis backend
- **🧪 Comprehensive Testing**: Unit, integration, and E2E tests with high coverage
- **🐳 Docker Support**: Full containerization with Docker Compose
- **📊 Real-Time Dashboard**: Beautiful web-based monitoring interface with auto-refresh

## 📋 Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- RabbitMQ (included in Docker setup)

## 🚀 Quick Start

### 1. Clone the repository
```bash
git clone <repository-url>
cd BlockchainMonitor
```

### 2. Run with Docker Compose
```bash
# Development environment
cd docker
./start-dev.sh

# Or manually
docker-compose up --build
```

### 3. Access the application
- **API Gateway**: http://localhost:5003
- **Swagger UI**: http://localhost:5003/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Real-Time Dashboard**: http://localhost:8080 (Auto-refreshing metrics)

### 4. Health Checks
```bash
# Gateway health
curl -f http://localhost:5003/health

# API health (direct)
curl -f http://localhost:5001/health
```

## 🏗️ Project Structure

```
BlockchainMonitor/
├── BlockchainMonitor.API/          # Web API layer
├── BlockchainMonitor.Application/   # Business logic layer
├── BlockchainMonitor.Domain/       # Domain entities and interfaces
├── BlockchainMonitor.Infrastructure/ # Data access and external services
├── BlockchainMonitor.Gateway/      # API Gateway (YARP)
├── BlockchainMonitor.DataFetcher/  # Background data fetching service
├── BlockchainMonitor.Tests.*/      # Test projects
└── docker/                        # Docker configuration
```

## 📊 Redis-Based Monitoring

The application includes comprehensive monitoring with Redis-backed metrics collection:

### **Metrics Collected**
- **Total Requests**: Global request count across all endpoints
- **Response Times**: Average response time in milliseconds
- **Error Counts**: Total errors across all endpoints
- **Cache Performance**: Global hit/miss rates
- **Database Operations**: Average operation times
- **Blockchain Data Fetched**: Total fetch operations
- **Rate Limiting**: Rate limit exceeded events

### **Real-Time Dashboard**
Access http://localhost:8080 to see:
- **Auto-refreshing metrics** every 30 seconds
- **Beautiful, responsive UI** with gradient design
- **Real-time data** from Redis-backed metrics
- **Global metrics** across all service instances
- **Performance insights** with visual cards and icons

![Blockchain Monitor Dashboard](docs/images/dashboard.png)

*The dashboard displays real-time metrics including total requests, response times, cache performance, database operations, and blockchain data fetching with a modern purple gradient interface.*

### **Dashboard Features**
- **Total Requests**: Track API usage across all endpoints
- **Avg Response Time**: Monitor performance with measurement counts
- **Data Fetched**: Track blockchain data collection
- **Cache Hit Rate**: Monitor caching effectiveness
- **Avg DB Operation**: Database performance metrics
- **Rate Limited**: Track rate limiting events

### **Console Logging**
All metrics are also logged to console with emojis for easy identification:
```
📊 Request Count: Total requests across all endpoints
⏱️ Response Time: Average response time with measurement count
❌ Error Count: Total errors across all endpoints
💾 Cache Hit: Global cache hit/miss rates
🏦 Blockchain Data Fetched: Total fetch operations
```

## 🧪 Testing

### Run all tests
```bash
dotnet test
```

### Test Results
- **Unit Tests**: 12 tests covering domain, services, and repositories
- **Integration Tests**: 12 tests covering API endpoints and data flow
- **E2E Tests**: 5 tests covering full application flow with Playwright

### Test Coverage
The test suite provides comprehensive coverage across all layers:
- Domain entities and business logic
- Service layer with caching and metrics
- Repository layer with database operations
- API controllers with validation
- End-to-end scenarios with real HTTP requests

## 🐳 Docker Configuration

### **Development Environment**
```yaml
# Ports
- Gateway: 5003:80, 5004:443
- API: Internal only (scaled to 2 instances)
- RabbitMQ: 5672:5672, 15672:15672
- Redis: 6379:6379 (Metrics storage)
- Dashboard: 8080:8000 (Monitoring interface)
- Database: Shared volume (blockchain.db)
```

### **Production Environment**
```yaml
# Ports
- Gateway: 80:80, 443:443
- API: Internal only (scaled to 2 instances)
- RabbitMQ: Internal only
- Redis: 6379:6379 (Metrics storage)
- Dashboard: 8080:8000 (Monitoring interface)
- Database: Persistent volume
```

## 🔧 Configuration

### **Environment Variables**
```bash
# Development
Database: blockchain.db (shared volume)
Gateway Ports: 5003:80, 5004:443
API Ports: Internal only (scaled to 2 instances)
RabbitMQ Ports: 5672:5672, 15672:15672

# Production
Database: Persistent volume
Gateway Ports: 80:80, 443:443
API Ports: Internal only (scaled to 2 instances)
RabbitMQ Ports: Internal only
```

### **API Endpoints**
- `GET /api/blockchain` - Get blockchain history with pagination
- `GET /api/blockchain/latest` - Get latest blockchain data
- `GET /health` - Health check endpoint

## 📈 Performance Features

- **Load Balancing**: API Gateway distributes requests across multiple API instances
- **Caching**: Intelligent memory caching with configurable expiration
- **Rate Limiting**: Built-in request throttling (100 requests/minute)
- **Metrics Collection**: Real-time performance monitoring
- **Error Handling**: Comprehensive exception handling and logging

## 🔒 Security Features

- **HTTPS Redirection**: Automatic HTTP to HTTPS redirection
- **Security Headers**: XSS protection, clickjacking prevention
- **Rate Limiting**: Protection against abuse
- **Input Validation**: Comprehensive parameter validation
- **CORS Configuration**: Cross-origin request handling

## 🚀 Deployment

### **Development**
```bash
cd docker
./start-dev.sh
```

### **Production**
```bash
cd docker
./start-prod.sh
```

## 📝 API Documentation

Once the application is running, visit:
- **Swagger UI**: http://localhost:5003/swagger
- **API Documentation**: Available through Swagger interface

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**🎉 Ready to monitor blockchain data with style!** The application provides a complete monitoring solution with local metrics, beautiful dashboards, and comprehensive testing.
