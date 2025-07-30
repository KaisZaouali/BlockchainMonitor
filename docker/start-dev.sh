#!/bin/bash

echo "🚀 Starting Blockchain Monitor (Development)"
echo "=========================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Start services
echo "📦 Starting services..."
docker-compose -f docker-compose.yml up -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 10

# Show status
echo "📊 Service Status:"
docker-compose -f docker-compose.yml ps

echo ""
echo "✅ Services started successfully!"
echo ""
echo "🌐 API: http://localhost:5001"
echo "📊 RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo ""
echo "📋 Useful commands:"
echo "  View logs: docker-compose -f docker-compose.yml logs -f"
echo "  Stop services: docker-compose -f docker-compose.yml down"
echo "  Rebuild: docker-compose -f docker-compose.yml up -d --build" 