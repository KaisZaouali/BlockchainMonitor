# Multi-stage build for BlockchainMonitor services
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Build argument for target project
ARG TARGET_PROJECT

# Copy project files
COPY ["${TARGET_PROJECT}/${TARGET_PROJECT}.csproj", "${TARGET_PROJECT}/"]
COPY ["BlockchainMonitor.Application/BlockchainMonitor.Application.csproj", "BlockchainMonitor.Application/"]
COPY ["BlockchainMonitor.Domain/BlockchainMonitor.Domain.csproj", "BlockchainMonitor.Domain/"]
COPY ["BlockchainMonitor.Infrastructure/BlockchainMonitor.Infrastructure.csproj", "BlockchainMonitor.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "${TARGET_PROJECT}/${TARGET_PROJECT}.csproj"

# Copy source code
COPY . .
WORKDIR "/src/${TARGET_PROJECT}"

# Build the application
RUN dotnet build "${TARGET_PROJECT}.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
ARG TARGET_PROJECT
RUN dotnet publish "${TARGET_PROJECT}.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Pass the build argument to the final stage
ARG TARGET_PROJECT

# Install RabbitMQ client dependencies and curl for health checks
RUN apt-get update && apt-get install -y \
    libssl3 \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create entrypoint script with the target project name
RUN echo '#!/bin/bash' > /app/entrypoint.sh && \
    echo "exec dotnet ${TARGET_PROJECT}.dll \"\$@\"" >> /app/entrypoint.sh && \
    chmod +x /app/entrypoint.sh

# Create directory for database
RUN mkdir -p /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Health check (will be overridden by specific services)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Entrypoint that uses the script
ENTRYPOINT ["/app/entrypoint.sh"] 