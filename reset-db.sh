#!/bin/bash

# Script untuk reset database development
set -e

echo "🗑️  Resetting database..."

# Stop containers
echo "Stopping containers..."
docker-compose down

# Remove database volume
echo "Removing database volume..."
docker volume rm $(docker-compose config --volumes) 2>/dev/null || true

# Remove API image to force rebuild
echo "Removing API image..."
docker rmi $(docker-compose config | grep 'image:' | grep -v postgres | awk '{print $2}') 2>/dev/null || true

# Start fresh
echo "Starting fresh containers..."
docker-compose up --build -d

echo "✅ Database reset completed!"
echo "The application will initialize with fresh data."