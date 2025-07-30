# Docker Setup

This folder contains all Docker-related files for the Blockchain Monitor application.

## 📁 Files

- `docker-compose.yml` - Development environment configuration
- `docker-compose.prod.yml` - Production environment configuration
- `Dockerfile` - Unified container definition for both API and DataFetcher services
- `.dockerignore` - Files to exclude from Docker builds
- `start-dev.sh` - Convenience script for development
- `start-prod.sh` - Convenience script for production

## 🚀 Quick Start

### Development
```bash
./start-dev.sh
```

### Production
```bash
./start-prod.sh
```

## 🔧 Manual Commands

### Development
```bash
# Start services
docker-compose -f docker-compose.yml up -d

# View logs
docker-compose -f docker-compose.yml logs -f

# Stop services
docker-compose -f docker-compose.yml down

# Rebuild
docker-compose -f docker-compose.yml up -d --build
```

### Production
```bash
# Set environment variables (optional)
export RABBITMQ_USER=your_user
export RABBITMQ_PASS=your_password

# Start services
docker-compose -f docker-compose.prod.yml up -d

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Stop services
docker-compose -f docker-compose.prod.yml down

# Rebuild
docker-compose -f docker-compose.prod.yml up -d --build
```

## 🏗️ Architecture

### Stack Names
- **Development**: `BlockchainMonitor`
- **Production**: `BlockchainMonitor-prod`

### Container Naming
- **Development**: `BlockchainMonitor-{service}` (e.g., `BlockchainMonitor-api`)
- **Production**: `BlockchainMonitor-prod-{service}` (e.g., `BlockchainMonitor-prod-api`)

### Unified Dockerfile
The project uses a single `Dockerfile` with build arguments to create both API and DataFetcher containers:

```yaml
# API Service
api:
  build:
    context: ..
    dockerfile: docker/Dockerfile
    args:
      TARGET_PROJECT: BlockchainMonitor.API

# DataFetcher Service  
datafetcher:
  build:
    context: ..
    dockerfile: docker/Dockerfile
    args:
      TARGET_PROJECT: BlockchainMonitor.DataFetcher
```

### Services
1. **RabbitMQ** - Message broker for event-driven architecture
2. **Migrate** - Database migration service (runs once)
3. **API** - REST API service
4. **DataFetcher** - Background blockchain data fetching

### Volumes
- `BlockchainMonitor_blockchain_data` - Database persistence (development)
- `BlockchainMonitor_rabbitmq_data` - RabbitMQ persistence (development)
- `BlockchainMonitor-prod_blockchain_data` - Database persistence (production)
- `BlockchainMonitor-prod_rabbitmq_data` - RabbitMQ persistence (production)

### Networks
- `BlockchainMonitor_blockchainmonitor-network` - Internal service communication (development)
- `BlockchainMonitor-prod_blockchainmonitor-network` - Internal service communication (production)

## 🔍 Troubleshooting

### View Service Logs
```bash
# All services
docker-compose -f docker-compose.yml logs

# Specific service
docker-compose -f docker-compose.yml logs api
docker-compose -f docker-compose.yml logs datafetcher
docker-compose -f docker-compose.yml logs migrate
```

### Reset Database
```bash
docker-compose -f docker-compose.yml down -v
docker-compose -f docker-compose.yml up -d
```

### Check Service Health
```bash
docker-compose -f docker-compose.yml ps
```

### Access Container Shell
```bash
# API container
docker exec -it BlockchainMonitor-api /bin/bash

# DataFetcher container
docker exec -it BlockchainMonitor-datafetcher /bin/bash

# Production containers
docker exec -it BlockchainMonitor-prod-api /bin/bash
docker exec -it BlockchainMonitor-prod-datafetcher /bin/bash
```

## 📊 Monitoring

### Health Checks
- **API**: `curl -f http://localhost/health`
- **DataFetcher**: Process monitoring
- **RabbitMQ**: Connection ping

### Ports
- **API**: 5001 (dev) / 80 (prod)
- **RabbitMQ**: 5672 (AMQP), 15672 (Management UI)

## 🔒 Security

### Production Considerations
1. Change default RabbitMQ credentials
2. Use environment variables for sensitive data
3. Enable HTTPS in production
4. Regular security updates
5. Network isolation

### Environment Variables
```bash
# Required for production
export RABBITMQ_USER=your_secure_user
export RABBITMQ_PASS=your_secure_password
``` 