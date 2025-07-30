# Docker Configuration for BlockchainMonitor

This document provides comprehensive instructions for running the BlockchainMonitor application using Docker and Docker Compose.

## 🐳 Overview

The application consists of four main services:
- **RabbitMQ**: Message broker for event sourcing
- **Migrate**: Database migration service (runs once)
- **API**: Web API service with health checks
- **DataFetcher**: Background service for data fetching

### Unified Dockerfile
The project uses a single `Dockerfile` with build arguments to create both API and DataFetcher containers, reducing duplication and improving maintainability.

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
   docker-compose -f docker/docker-compose.yml logs -f api
   docker-compose -f docker/docker-compose.yml logs -f datafetcher
   docker-compose -f docker/docker-compose.yml logs -f migrate
   ```

5. **Access the application:**
   - **API**: http://localhost:5001
   - **Swagger UI**: http://localhost:5001/swagger
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
   - **API**: http://localhost:80
   - **RabbitMQ Management**: http://localhost:15672

## ��️ Architecture

### Stack Names
- **Development**: `BlockchainMonitor`
- **Production**: `BlockchainMonitor-prod`

### Container Naming
- **Development**: `BlockchainMonitor-{service}` (e.g., `BlockchainMonitor-api`)
- **Production**: `BlockchainMonitor-prod-{service}` (e.g., `BlockchainMonitor-prod-api`)

### Service Dependencies

```
RabbitMQ (5672, 15672)
    ↓
Migrate (runs once)
    ↓
API (5001, 5002) & DataFetcher
```

### Network Configuration

- **Development Network**: `BlockchainMonitor_blockchainmonitor-network` (bridge)
- **Production Network**: `BlockchainMonitor-prod_blockchainmonitor-network` (bridge)
- **Internal Communication**: Services communicate via container names
- **External Access**: Port mappings for API and RabbitMQ Management

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
| API Ports | 5000:80, 5001:443 | 80:80, 443:443 |
| Data Fetch Interval | 60 seconds | 300 seconds |
| Request Delay | 1000ms | 2000ms |
| Retry Attempts | 3 | 5 |
| Retry Delay | 2000ms | 5000ms |
| Resource Limits | None | 512MB RAM, 0.5 CPU |

## 🏥 Health Checks

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
docker-compose -f docker/docker-compose.yml ps api
docker-compose -f docker/docker-compose.yml ps datafetcher
docker-compose -f docker/docker-compose.yml ps migrate
```

### Logs
```bash
# All services
docker-compose -f docker/docker-compose.yml logs -f

# Specific service
docker-compose -f docker/docker-compose.yml logs -f api
docker-compose -f docker/docker-compose.yml logs -f datafetcher
docker-compose -f docker/docker-compose.yml logs -f migrate

# Last 100 lines
docker-compose -f docker/docker-compose.yml logs --tail=100 api
```

### Resource Usage
```bash
# Container resource usage
docker stats

# Specific container
docker stats BlockchainMonitor-api
docker stats BlockchainMonitor-datafetcher
docker stats BlockchainMonitor-rabbitmq
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

#### 2. Database Connection Issues
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

#### 3. RabbitMQ Connection Issues
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

#### 4. Memory Issues
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
docker-compose -f docker/docker-compose.yml exec api sh
docker-compose -f docker/docker-compose.yml exec datafetcher sh
docker-compose -f docker/docker-compose.yml exec rabbitmq bash

# Check container logs
docker-compose -f docker/docker-compose.yml logs api

# Check container health
docker inspect BlockchainMonitor-api | grep Health -A 10

# Check network connectivity
docker-compose -f docker/docker-compose.yml exec api ping rabbitmq
docker-compose -f docker/docker-compose.yml exec datafetcher ping rabbitmq
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
docker-compose build api
docker-compose up -d api
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
- **API**: No resource limits
- **DataFetcher**: No resource limits
- **RabbitMQ**: Default settings

### Production Environment
- **API**: 512MB RAM, 0.5 CPU
- **DataFetcher**: 256MB RAM, 0.25 CPU
- **RabbitMQ**: Default settings

### Optimization Tips
1. **Use production compose file** for better performance
2. **Monitor resource usage** with `docker stats`
3. **Adjust intervals** based on your needs
4. **Use volume mounts** for persistent data
5. **Enable health checks** for automatic recovery

## 🔒 Security

### Best Practices
1. **Change default passwords** in production
2. **Use environment variables** for secrets
3. **Limit port exposure** to necessary ports only
4. **Regular security updates** for base images
5. **Monitor logs** for suspicious activity

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