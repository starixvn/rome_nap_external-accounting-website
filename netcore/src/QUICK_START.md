# Quick Start Guide - Docker Migration

## Current Status

✅ **Completed:**
- Docker Compose configuration (`docker-compose.yml`)
- All Dockerfiles created
- API Gateway routing configuration (`ApiGateway/appsettings.json`)
- .env template created

⚠️ **Required Before Building:**
1. Service projects need to be created (AuthService, CustomerService, etc.)
2. Update `.env` file with your actual database connection strings

## Quick Commands

### 1. Update Environment Variables

Edit `.env` file:
```bash
# Update these values
DB_CONNECTION_STRING=Server=your-db-server;Database=ExternalAccount;User Id=your-user;Password=your-password;
REDIS_CONNECTION_STRING=your-redis-server:6379
```

### 2. Build Services (After Creating Projects)

```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build api-gateway
```

### 3. Start Services

```bash
# Start all services in background
docker-compose up -d

# Start and view logs
docker-compose up

# View logs
docker-compose logs -f
```

### 4. Check Status

```bash
# List running containers
docker-compose ps

# Check specific service logs
docker-compose logs api-gateway

# Check health
docker-compose ps --format "table {{.Service}}\t{{.Status}}"
```

### 5. Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## Service Ports

| Service | Port | URL |
|---------|------|-----|
| API Gateway | 80 | http://localhost |
| Auth Service | 5001 | http://localhost:5001 |
| Customer Service | 5002 | http://localhost:5002 |
| Report Service | 5003 | http://localhost:5003 |
| Statement Service | 5004 | http://localhost:5004 |
| User Control Service | 5005 | http://localhost:5005 |
| WebApp (UI) | 5006 | http://localhost:5006 |
| Redis | 6379 | localhost:6379 |

## Testing

```bash
# Test API Gateway
curl http://localhost/health

# Test Auth Service
curl http://localhost:5001/health

# Test routing
curl http://localhost/api/auth/login
curl http://localhost/Account/Login

# Test webapp
curl http://localhost:5006
curl http://localhost:5006/Home/Index
```

## Troubleshooting

### Build Fails - Service Projects Missing
If you see errors like "project file not found":
- You need to create the service projects first
- Use the migration scripts or create manually

### Port Already in Use
```bash
# Check what's using port 80
netstat -ano | findstr :80

# Change ports in docker-compose.yml if needed
```

### Container Won't Start
```bash
# Check logs
docker-compose logs [service-name]

# Rebuild
docker-compose build --no-cache [service-name]
docker-compose up -d [service-name]
```

## Next Steps

1. **Create Service Projects** (if not done):
   - AuthService
   - CustomerService
   - ReportService
   - StatementService
   - UserControlService
   - WebApp (Fanex.ExternalAccount.UI) - Already migrated

2. **Update Configuration**:
   - Edit `.env` with real database credentials
   - Update `ApiGateway/appsettings.json` if needed

3. **Build and Deploy**:
   ```bash
   docker-compose build
   docker-compose up -d
   ```

4. **Test and Validate**:
   - Test health endpoints
   - Test routing
   - Verify all services are running

## Documentation

- `DOCKER_MIGRATION_GUIDE.md` - Full migration guide
- `README.Docker.md` - Docker setup details
- `scripts/README.md` - Scripts documentation

