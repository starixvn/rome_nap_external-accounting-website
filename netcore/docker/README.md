# Fanex External Account Reports - Docker Setup

This document provides instructions for running the Fanex External Account Reports application using Docker and Docker Compose.

## Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+
- At least 4GB RAM available for containers
- At least 10GB free disk space

## Quick Start

### Development Environment

1. **Clone the repository and navigate to the project root**

2. **Start the development environment:**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
   ```

3. **Access the application:**
   - Web App: http://localhost:5000
   - SQL Server: localhost:1433 (SA password: YourStrong!Passw0rd)
   - Redis: localhost:6379 (password: yourpassword)

### Production Environment

1. **Set environment variables:**
   ```bash
   export DB_ONUSER_CONNECTION="your_production_db_connection_string"
   export DB_TXN_CONNECTION="your_production_db_connection_string"
   export CUSTOMER_API_URL="https://customer-api.yourdomain.com/api/"
   export CUSTOMER_CDN_URL="https://cdn.yourdomain.com/"
   export REDIS_CONNECTION="redis:6379,password=your_secure_password"
   export REDIS_PASSWORD="your_secure_password"
   ```

2. **Create secrets directory and files:**
   ```bash
   mkdir -p secrets
   echo "your_db_password" > secrets/db_password.txt
   echo "your_redis_password" > secrets/redis_password.txt
   ```

3. **Start production environment:**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
   ```

## Configuration

### Environment Variables

The application supports the following environment variables:

#### Database Connections
- `ConnectionStrings__DBONUSER.ON_USER.wsv_xnap`
- `ConnectionStrings__DBTXN.bodb02.wsv_xnap`
- `ConnectionStrings__DBSITE01.bodb02.wsv_xnap`
- And other database connection strings from `appsettings.json`

#### Application Settings
- `ASPNETCORE_ENVIRONMENT`: Development/Production/Staging
- `ASPNETCORE_URLS`: URLs the application will listen on
- `Environment`: UAT/Production
- `EnvTheme`: dev/prod
- `IsTest`: 0/1
- `IsCache`: 0/1
- `SystemId`: System identifier

#### External Services
- `Fanex.Customer:ServiceUri`: Customer API endpoint
- `Fanex.Customer:CdnUri`: Customer CDN URL
- `Redis:ConnectionString`: Redis connection string

## Docker Commands

### Build and Run
```bash
# Build images
docker-compose build

# Start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Development Workflow
```bash
# Rebuild after code changes
docker-compose up --build -d fanex-external-account-reports

# View specific service logs
docker-compose logs -f fanex-external-account-reports

# Execute commands in running container
docker-compose exec fanex-external-account-reports bash
```

### Database Management
```bash
# Connect to SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd

# Backup database
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -Q "BACKUP DATABASE [your_db] TO DISK = '/var/opt/mssql/backup/your_db.bak'"
```

### Monitoring
```bash
# Check container health
docker-compose ps

# View resource usage
docker-compose stats

# Check container logs
docker-compose logs fanex-external-account-reports
```

## Troubleshooting

### Common Issues

1. **Port conflicts:**
   - Change port mappings in docker-compose files
   - Example: Change `"8080:80"` to `"8081:80"`

2. **Database connection issues:**
   - Ensure SQL Server container is healthy: `docker-compose ps`
   - Check connection strings in environment variables
   - Verify database exists and user has permissions

3. **Memory issues:**
   - Increase Docker memory limit in Docker Desktop settings
   - Reduce SQL Server memory allocation if needed

4. **Build failures:**
   - Clear Docker cache: `docker system prune -a`
   - Rebuild without cache: `docker-compose build --no-cache`

### Logs and Debugging

```bash
# View application logs
docker-compose logs -f fanex-external-account-reports

# View SQL Server logs
docker-compose logs -f sqlserver

# Access container shell
docker-compose exec fanex-external-account-reports /bin/bash

# Check container resource usage
docker-compose stats
```

## Security Considerations

1. **Change default passwords** in production
2. **Use Docker secrets** for sensitive data
3. **Enable SSL/TLS** with proper certificates
4. **Regularly update** base images
5. **Use private networks** between containers
6. **Implement proper logging** and monitoring

## Backup and Restore

### Database Backup
```bash
# Create backup
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong!Passw0rd \
  -Q "BACKUP DATABASE [your_db] TO DISK = '/var/opt/mssql/backup/your_db.bak'"

# Copy backup to host
docker cp $(docker-compose ps -q sqlserver):/var/opt/mssql/backup/your_db.bak ./backups/
```

### Database Restore
```bash
# Copy backup to container
docker cp ./backups/your_db.bak $(docker-compose ps -q sqlserver):/var/opt/mssql/backup/

# Restore database
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong!Passw0rd \
  -Q "RESTORE DATABASE [your_db] FROM DISK = '/var/opt/mssql/backup/your_db.bak'"
```

## Performance Tuning

1. **SQL Server:**
   - Adjust memory limits in docker-compose
   - Use faster storage (SSD)
   - Configure proper logging

2. **Application:**
   - Set appropriate environment variables
   - Configure connection pooling
   - Enable caching where appropriate

3. **Docker:**
   - Use resource limits appropriately
   - Monitor container resource usage
   - Optimize Docker images

## Support

For issues with this Docker setup, please check:
1. Docker and Docker Compose documentation
2. Application logs
3. Container health checks
4. Network connectivity between containers
