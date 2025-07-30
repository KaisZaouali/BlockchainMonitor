# Docker Configuration for BlockchainMonitor

This document provides comprehensive instructions for running the BlockchainMonitor application using Docker and Docker Compose.

## 🐳 Overview

The application consists of five main services:
- **RabbitMQ**: Message broker for event sourcing
- **Migrate**: Database migration service (runs once)
- **API Gateway**: Reverse proxy with load balancing and rate limiting
- **API**: Web API service with health checks (scaled to 2 instances)
- **DataFetcher**: Background service for data fetching

### Architecture
- **API Gateway**: Routes traffic to scaled API instances
- **Load Balancing**: Round-robin distribution across API instances
- **Rate Limiting**: 100 requests per minute per client
- **Health Checks**: Active monitoring of all services

## 📋 Prerequisites

- Docker Desktop (Windows/macOS) or Docker Engine (Linux)
- Docker Compose
- At least 2GB of available RAM
- At least 1GB of available disk space

## 🚀 Quick Start

### Development Environment

1. **Clone and navigate to the project:**
   ```bash
   git clone <repository-url>
   cd BlockchainMonitor
   ```

2. **Start all services:**
   ```bash
   # Using convenience script (recommended)
   cd docker && ./start-dev.sh
   
   # Or manually
   docker-compose -f docker/docker-compose.yml up -d
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
   docker-compose -f docker/docker-compose.yml logs -f api
   docker-compose -f docker/docker-compose.yml logs -f datafetcher
   ```

5. **Access the application:**
   - **API Gateway**: http://localhost:5003
   - **API (Direct)**: http://localhost:5001 (for debugging)
   - **Swagger UI**: http://localhost:5003/swagger
   - **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### Production Environment

1. **Set environment variables:**
   ```bash
   export RABBITMQ_USER=your_username
   export RABBITMQ_PASS=your_password
   ```

2. **Start production services:**
   ```bash
   # Using convenience script (recommended)
   cd docker && ./start-prod.sh
   
   # Or manually
   docker-compose -f docker/docker-compose.prod.yml up -d
   ```

3. **Access the application:**
   - **API Gateway**: http://localhost:80
   - **RabbitMQ Management**: http://localhost:15672

## 🏗️ Architecture

### Stack Names
- **Development**: `BlockchainMonitor`
- **Production**: `BlockchainMonitor-prod`

### Container Naming
- **Development**: `BlockchainMonitor-{service}` (e.g., `BlockchainMonitor-gateway`)
- **Production**: `BlockchainMonitor-prod-{service}` (e.g., `BlockchainMonitor-prod-gateway`)

### Service Dependencies

```
RabbitMQ (5672, 15672)
    ↓
Migrate (runs once)
    ↓
API (scaled to 2 instances)
    ↓
Gateway (5000) → Load Balancer → API instances
```

### Network Configuration

- **Development Network**: `BlockchainMonitor_blockchainmonitor-network` (bridge)
- **Production Network**: `BlockchainMonitor-prod_blockchainmonitor-network` (bridge)
- **Internal Communication**: Services communicate via container names
- **External Access**: Port mappings for Gateway and RabbitMQ Management

### Volume Mounts

- **Database**: `BlockchainMonitor_blockchain_data:/app/data` (development) / `BlockchainMonitor-prod_blockchain_data:/app/data` (production)
- **RabbitMQ Data**: `BlockchainMonitor_rabbitmq_data:/var/lib/rabbitmq` (development) / `BlockchainMonitor-prod_rabbitmq_data:/var/lib/rabbitmq` (production)

### Database Sharing

Both the API and DataFetcher services share the same `blockchain_data` volume, ensuring:
- **Data Consistency**: Both services access the same database file
- **No Conflicts**: Eliminates the risk of having separate database instances
- **Persistent Storage**: Database survives container restarts and updates
- **Backup Simplicity**: Single database file to backup and restore

## 🔧 Configuration

### Environment Variables

#### API Gateway Service
```bash
ASPNETCORE_ENVIRONMENT=Development
```

#### API Service
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Data Source=/app/data/blockchain.db
RABBITMQ__HOSTNAME=rabbitmq
RABBITMQ__USERNAME=guest
RABBITMQ__PASSWORD=guest
RABBITMQ__PORT=5672
```

#### DataFetcher Service
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Data Source=/app/data/blockchain.db
RABBITMQ__HOSTNAME=rabbitmq
RABBITMQ__USERNAME=guest
RABBITMQ__PASSWORD=guest
RABBITMQ__PORT=5672
```

#### RabbitMQ Service
```bash
RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest
```

### Production vs Development

| Setting | Development | Production |
|---------|-------------|------------|
| Gateway Ports | 5003:80, 5004:443 | 80:80, 443:443 |
| API Ports | Internal only | Internal only |
| API Instances | 2 replicas | 2 replicas |
| Data Fetch Interval | 60 seconds | 300 seconds |
| Request Delay | 1000ms | 2000ms |
| Retry Attempts | 3 | 5 |
| Retry Delay | 2000ms | 5000ms |
| Resource Limits | None | 512MB RAM, 0.5 CPU |

## 🏥 Health Checks

### Gateway Health Checks
- **Endpoint**: `GET /health`
- **Docker Health Check**: `curl -f http://localhost/health`

### API Health Checks
- **Endpoint**: `GET /health`
- **Detailed**: `GET /health/detailed`
- **Docker Health Check**: `curl -f http://localhost/health`

### DataFetcher Health Checks
- **Docker Health Check**: `ps aux | grep "BlockchainMonitor.DataFetcher"`

### RabbitMQ Health Checks
- **Docker Health Check**: `rabbitmq-diagnostics ping`

## 🔍 Monitoring

### Service Status
```bash
# Check all services
docker-compose -f docker/docker-compose.yml ps

# Check specific service
docker-compose -f docker/docker-compose.yml ps gateway
docker-compose -f docker/docker-compose.yml ps api
docker-compose -f docker/docker-compose.yml ps datafetcher
```

### Logs
```bash
# All services
docker-compose -f docker/docker-compose.yml logs -f

# Specific service
docker-compose -f docker/docker-compose.yml logs -f gateway
docker-compose -f docker/docker-compose.yml logs -f api
docker-compose -f docker/docker-compose.yml logs -f datafetcher

# Last 100 lines
docker-compose -f docker/docker-compose.yml logs --tail=100 gateway
```

### Resource Usage
```bash
# Container resource usage
docker stats

# Specific container
docker stats BlockchainMonitor-gateway
docker stats BlockchainMonitor-api
docker stats BlockchainMonitor-datafetcher
```

## 🛠️ Troubleshooting

### Common Issues

#### 1. Port Conflicts
**Problem**: Port 5000, 5001, 5672, or 15672 already in use
**Solution**: 
```bash
# Check what's using the port
lsof -i :5000

# Stop conflicting service or change ports in docker-compose.yml
```

#### 2. Gateway Connection Issues
**Problem**: Gateway can't connect to API instances
**Solution**:
```bash
# Check API instances are running
docker-compose -f docker/docker-compose.yml ps api

# Check gateway logs
docker-compose -f docker/docker-compose.yml logs gateway

# Test gateway health
curl -f http://localhost:5003/health
```

#### 3. Database Connection Issues
**Problem**: API can't connect to database
**Solution**:
```bash
# Check database file permissions
ls -la ./data/

# Recreate database volume
docker-compose -f docker/docker-compose.yml down
rm -rf ./data
docker-compose -f docker/docker-compose.yml up -d
```

#### 4. RabbitMQ Connection Issues
**Problem**: Services can't connect to RabbitMQ
**Solution**:
```bash
# Check RabbitMQ status
docker-compose -f docker/docker-compose.yml logs rabbitmq

# Restart RabbitMQ
docker-compose -f docker/docker-compose.yml restart rabbitmq

# Check RabbitMQ management UI
# http://localhost:15672 (guest/guest)
```

#### 5. Memory Issues
**Problem**: Containers running out of memory
**Solution**:
```bash
# Check memory usage
docker stats

# Increase Docker memory limit in Docker Desktop
# Or add resource limits in docker-compose.yml
```

### Debug Commands

```bash
# Enter container shell
docker-compose -f docker/docker-compose.yml exec gateway sh
docker-compose -f docker/docker-compose.yml exec api sh
docker-compose -f docker/docker-compose.yml exec datafetcher sh
docker-compose -f docker/docker-compose.yml exec rabbitmq bash

# Check container logs
docker-compose -f docker/docker-compose.yml logs gateway

# Check container health
docker inspect BlockchainMonitor-gateway | grep Health -A 10

# Check network connectivity
docker-compose -f docker/docker-compose.yml exec gateway ping api
docker-compose -f docker/docker-compose.yml exec api ping rabbitmq
```

## 🔄 Maintenance

### Database Backup
```bash
# Backup database from the shared volume
docker-compose -f docker/docker-compose.yml exec api cp /app/data/blockchain.db /app/data/blockchain.db.backup

# Copy from container to host
docker cp BlockchainMonitor-api:/app/data/blockchain.db ./backup/

# Alternative: Backup the entire volume
docker run --rm -v BlockchainMonitor_blockchain_data:/data -v $(pwd):/backup alpine tar czf /backup/blockchain_data_backup.tar.gz -C /data .
```

### RabbitMQ Data Backup
```bash
# Backup RabbitMQ data
docker-compose -f docker/docker-compose.yml exec rabbitmq rabbitmqctl export_definitions > rabbitmq_backup.json
```

### Service Updates
```bash
# Rebuild and restart services
docker-compose down
docker-compose build --no-cache
docker-compose up -d

# Update specific service
docker-compose build gateway
docker-compose up -d gateway
```

### Cleanup
```bash
# Remove all containers and volumes
docker-compose down -v

# Remove all images
docker-compose down --rmi all

# Clean up unused resources
docker system prune -a
```

## 📊 Performance Tuning

### Development Environment
- **Gateway**: No resource limits
- **API**: No resource limits (2 instances)
- **DataFetcher**: No resource limits
- **RabbitMQ**: Default settings

### Production Environment
- **Gateway**: 256MB RAM, 0.25 CPU
- **API**: 512MB RAM, 0.5 CPU (2 instances)
- **DataFetcher**: 256MB RAM, 0.25 CPU
- **RabbitMQ**: Default settings

### Load Balancing
- **Algorithm**: Round-robin
- **Health Checks**: Active monitoring
- **Failover**: Automatic failover to healthy instances

### Optimization Tips
1. **Use production compose file** for better performance
2. **Monitor resource usage** with `docker stats`
3. **Adjust intervals** based on your needs
4. **Use volume mounts** for persistent data
5. **Enable health checks** for automatic recovery
6. **Scale API instances** based on load

## 🔒 Security

### Best Practices
1. **Change default passwords** in production
2. **Use environment variables** for secrets
3. **Limit port exposure** to necessary ports only
4. **Regular security updates** for base images
5. **Monitor logs** for suspicious activity
6. **Use API Gateway** for centralized security

### Production Security
```bash
# Set strong passwords
export RABBITMQ_USER=secure_username
export RABBITMQ_PASS=secure_password

# Use production compose file
docker-compose -f docker-compose.prod.yml up -d
```

## 📝 Useful Commands

### Development
```bash
# Start development environment
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose up -d --build
```

### Production
```bash
# Start production environment
docker-compose -f docker-compose.prod.yml up -d

# View production logs
docker-compose -f docker-compose.prod.yml logs -f

# Stop production services
docker-compose -f docker-compose.prod.yml down
```

### Maintenance
```bash
# Check service health
docker-compose ps

# View resource usage
docker stats

# Backup database
docker-compose exec api cp /app/data/blockchain.db /app/data/backup.db

# Clean up
docker system prune -a
```

## 🚀 API Gateway Features

### Routing
- **API Routes**: `/api/*` → API instances
- **Health Routes**: `/health` → API instances
- **Gateway Health**: `/health` → Gateway health

### Load Balancing
- **Algorithm**: Round-robin
- **Health Checks**: Active monitoring
- **Failover**: Automatic failover

### Rate Limiting
- **Limit**: 100 requests per minute per client
- **Window**: 1 minute sliding window
- **Headers**: Rate limit headers included

### Monitoring
- **Request Logging**: All requests logged with timing
- **Error Tracking**: Detailed error logging
- **Performance Metrics**: Response time monitoring

## 📁 Files

- `docker-compose.yml` - Development environment configuration
- `docker-compose.prod.yml` - Production environment configuration
- `Dockerfile` - Unified container definition for API and DataFetcher services
- `BlockchainMonitor.Gateway/Dockerfile` - API Gateway container definition
- `.dockerignore` - Files to exclude from Docker builds
- `start-dev.sh` - Convenience script for development
- `start-prod.sh` - Convenience script for production 