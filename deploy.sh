#!/bin/bash

# Deploy script untuk CafeMobile API
set -e

echo "🚀 Starting CafeMobile API Deployment..."

# Colors untuk output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function untuk print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Check if .env file exists
if [ ! -f .env ]; then
    print_warning ".env file not found. Creating from template..."
    cp .env.example .env 2>/dev/null || echo "Please create .env file manually"
    print_warning "Please update .env file with your production values!"
    exit 1
fi

# Create necessary directories
print_status "Creating necessary directories..."
mkdir -p logs/nginx
mkdir -p nginx/ssl

# Check if nginx config exists
if [ ! -f nginx/nginx.conf ]; then
    print_error "nginx/nginx.conf not found. Please create the nginx configuration file."
    exit 1
fi

# Backup existing containers (if any)
print_status "Stopping existing containers..."
docker-compose down 2>/dev/null || true

# Pull latest images
print_status "Pulling latest images..."
docker-compose pull

# Build and start services
print_status "Building and starting services..."
docker-compose up --build -d

# Wait for services to be healthy
print_status "Waiting for services to be healthy..."
sleep 30

# Check if services are running
if docker-compose ps | grep -q "Up"; then
    print_status "✅ Deployment successful!"
    echo ""
    print_status "Services status:"
    docker-compose ps
    echo ""
    print_status "API should be available at: http://localhost:8080"
    print_status "Through Nginx at: http://localhost"
    echo ""
    print_status "To check logs:"
    echo "  docker-compose logs api"
    echo "  docker-compose logs nginx"
    echo "  docker-compose logs db"
    echo ""
    print_status "To stop services:"
    echo "  docker-compose down"
else
    print_error "❌ Deployment failed!"
    print_error "Check logs with: docker-compose logs"
    exit 1
fi

# Optional: Run database migrations
read -p "Do you want to run database migrations? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    print_status "Running database migrations..."
    docker-compose exec api dotnet ef database update || print_warning "Migration failed or not configured"
fi

print_status "🎉 Deployment completed!"