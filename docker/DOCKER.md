# Docker Configuration for BlockchainMonitor

This document provides comprehensive instructions for running the BlockchainMonitor application using Docker and Docker Compose.

## 🐳 Overview

The application consists of seven main services:
- **RabbitMQ**: Message broker for event sourcing
- **Migrate**: Database migration service (runs once)
- **API Gateway**: Reverse proxy with load balancing and rate limiting
- **API**: Web API service with health checks (scaled to 2 instances)
- **DataFetcher**: Background service for data fetching
- **Dashboard**: Simple HTTP server serving the monitoring dashboard
- **Redis**: Redis server to store metrics used in dashboard

### Architecture
- **API Gateway**: Routes traffic to scaled API instances
- **Load Balancing**: Round-robin distribution across API instances
- **Rate Limiting**: 100 requests per minute per client
- **Health Checks**: Active monitoring of all services
- **Metrics System**: Redis-backed metrics collection across all services
- **Dashboard**: Real-time monitoring dashboard with auto-refresh

## 🚀 Quick Start

### Development Environment

1. **Clone and navigate to the project:**
   ```bash
   git clone https://github.com/KaisZaouali/BlockchainMonitor
   cd BlockchainMonitor
   ```

2. **Start all services:**
   ```bash
   docker-compose -f docker/docker-compose.yml up -d --build
   ```

3. **Check service status:**
   ```bash
   docker-compose -f docker/docker-compose.yml ps
   ```

4. **View logs:**
   ```bash
   # All services
   docker-compose -f docker/docker-compose.yml logs -f
   
   # Specific service
   docker-compose -f docker/docker-compose.yml logs -f gateway
   ```

5. **Access the application:**
   - **API Gateway**: http://localhost:5003
   - **Swagger UI**: http://localhost:5003/swagger
   - **RabbitMQ Management**: http://localhost:15672 (guest/guest)
   - **Monitoring Dashboard**: http://localhost:8080 (Real-time metrics with auto-refresh)

### Production Environment

1. **Set environment variables:**
   ```bash
   export RABBITMQ_USER=your_username
   export RABBITMQ_PASS=your_password
   ```

2. **Start production services:**
   ```bash
   docker-compose -f docker/docker-compose.prod.yml up -d --build
   ```

3. **Access the application:**
   - **API Gateway**: http://localhost:80
   - **RabbitMQ Management**: http://localhost:15672
   - **Monitoring Dashboard**: http://localhost:8080 (Real-time metrics with auto-refresh)

## 🏗️ Architecture

### Stack Names
- **Development**: `BlockchainMonitor`
- **Production**: `BlockchainMonitor-prod`

### Container Naming
- **Development**: `BlockchainMonitor-{service}` (e.g., `BlockchainMonitor-gateway`)
- **Production**: `BlockchainMonitor-prod-{service}` (e.g., `BlockchainMonitor-prod-gateway`)

### Service Dependencies

```
migrate → api, datafetcher
rabbitmq → api, datafetcher
redis → api, datafetcher, gateway
api → gateway
gateway → dashboard
```

### Port Mappings

#### Development
- **Gateway**: 5003:80, 5004:443
- **API**: Internal only (scaled to 2 instances)
- **RabbitMQ**: 5672:5672, 15672:15672
- **Redis**: 6379:6379
- **Dashboard**: 8080:8000

#### Production
- **Gateway**: 80:80, 443:443
- **API**: Internal only (scaled to 2 instances)
- **RabbitMQ**: Internal only
- **Redis**: 6379:6379
- **Dashboard**: 8080:8000

## 🔧 Configuration

### Environment Variables

#### Development
```bash
# Database
ConnectionStrings__DefaultConnection=Data Source=/app/data/blockchain.db

# Redis
ConnectionStrings__Redis=redis:6379

# RabbitMQ
RABBITMQ__HOSTNAME=rabbitmq
RABBITMQ__USERNAME=guest
RABBITMQ__PASSWORD=guest
RABBITMQ__PORT=5672
```

#### Production
```bash
# Database
ConnectionStrings__DefaultConnection=Data Source=/app/data/blockchain-prod.db

# Redis
ConnectionStrings__Redis=redis:6379

# RabbitMQ
RABBITMQ__HOSTNAME=rabbitmq
RABBITMQ__USERNAME=${RABBITMQ_USER:-guest}
RABBITMQ__PASSWORD=${RABBITMQ_PASS:-guest}
RABBITMQ__PORT=5672
```

## 🛠️ Troubleshooting

### Common Issues

1. **Port conflicts**: Ensure ports 5003, 5004, 8080, 5672, 15672, 6379 are available
2. **Memory issues**: Increase Docker memory allocation to at least 2GB
3. **Service startup failures**: Check logs with `docker-compose logs [service-name]`
4. **Database connection**: Ensure migrate service completes before starting API services

### Health Checks

```bash
# Gateway health
curl -f http://localhost:5003/health

# Api health. Internal in docker-compose, you can run it via dotnet run
curl -f http://localhost:5001/health

# Api detailed health. Internal in docker-compose, you can run it via dotnet run
curl -f http://localhost:5001/health/detailed

# Redis health
docker exec blockchainmonitor-redis redis-cli ping

# RabbitMQ health
curl -f http://localhost:15672/api/overview
```

### Logs and Debugging

```bash
# View all logs
docker-compose -f docker/docker-compose.yml logs -f

# View specific service logs
docker-compose -f docker/docker-compose.yml logs -f gateway
docker-compose -f docker/docker-compose.yml logs -f api
docker-compose -f docker/docker-compose.yml logs -f datafetcher

# Access service containers
docker exec -it blockchainmonitor-gateway sh
docker exec -it blockchainmonitor-api sh
```

## 📚 Additional Resources

- [Main README](../README.md) - Project overview and features
- [API Documentation](http://localhost:5003/swagger) - Interactive API docs
- [Metrics Dashboard](http://localhost:8080) - Real-time monitoring
- [RabbitMQ Management](http://localhost:15672) - Message broker management 