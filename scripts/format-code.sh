#!/bin/bash

# Format and lint code script
# This script formats code, runs analyzers, and checks for issues

set -e

echo "🔧 Formatting and linting code..."

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed or not in PATH"
    exit 1
fi

# Restore packages
echo "📦 Restoring packages..."
dotnet restore

# Format code
echo "🎨 Formatting code..."
dotnet format --verbosity normal

# Build with analyzers
echo "🔍 Building with analyzers..."
dotnet build --verbosity normal

echo "✅ Code formatting and linting completed successfully!"
