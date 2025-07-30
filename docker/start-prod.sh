#!/bin/bash

echo "🚀 Starting Blockchain Monitor (Production)"
echo "=========================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Check for environment variables
if [ -z "$RABBITMQ_USER" ] || [ -z "$RABBITMQ_PASS" ]; then
    echo "⚠️  Warning: RABBITMQ_USER and RABBITMQ_PASS not set."
    echo "   Using default credentials (guest/guest)"
    echo "   Set these for production:"
    echo "   export RABBITMQ_USER=your_user"
    echo "   export RABBITMQ_PASS=your_password"
    echo ""
fi

# Start services
echo "📦 Starting services..."
docker-compose -f docker-compose.prod.yml up -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 15

# Show status
echo "📊 Service Status:"
docker-compose -f docker-compose.prod.yml ps

echo ""
echo "✅ Services started successfully!"
echo ""
echo "🌐 API: http://localhost"
echo "📊 RabbitMQ Management: http://localhost:15672"
echo ""
echo "📋 Useful commands:"
echo "  View logs: docker-compose -f docker-compose.prod.yml logs -f"
echo "  Stop services: docker-compose -f docker-compose.prod.yml down"
echo "  Rebuild: docker-compose -f docker-compose.prod.yml up -d --build" 