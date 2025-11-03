# Docker Setup cho External Accounting Website

## Quick Start

### 1. Chuẩn bị Environment Variables

Copy file `.env.example` thành `.env` và điền thông tin:

```bash
cp .env.example .env
# Chỉnh sửa .env với thông tin của bạn
```

### 2. Build và Start Services

```bash
# Build tất cả images
docker-compose build

# Start tất cả services
docker-compose up -d

# Xem logs
docker-compose logs -f
```

### 3. Kiểm tra Services

```bash
# Health check
curl http://localhost/health

# Test routing
curl http://localhost/api/auth/login
curl http://localhost/api/customer/list
```

## Kiến trúc

```
Client → API Gateway (Port 80) → Services (Port 5001-5005)
```

### Services:

- **API Gateway** (Port 80): Routing và load balancing
- **Auth Service** (Port 5001): Authentication và authorization
- **Customer Service** (Port 5002): Customer management
- **Report Service** (Port 5003): Reports và analytics
- **Statement Service** (Port 5004): Financial statements
- **User Control Service** (Port 5005): User và permission management

## Routing

API Gateway sử dụng YARP để route requests:

| Path Pattern | Routes To | Service |
|-------------|-----------|---------|
| `/api/auth/*`, `/Account/*` | Auth Service | Port 5001 |
| `/api/customer/*`, `/Customer/*`, `/Home/*` | Customer Service | Port 5002 |
| `/api/report/*`, `/DailyWinloss/*`, `/WinlossDetail/*`, `/TotalBet/*` | Report Service | Port 5003 |
| `/api/statement/*`, `/Statement/*` | Statement Service | Port 5004 |
| `/api/usercontrol/*`, `/User/*`, `/Group/*`, `/SubGroup/*` | User Control Service | Port 5005 |

## Commands

```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# Restart một service
docker-compose restart auth-service

# Xem logs của một service
docker-compose logs -f auth-service

# Rebuild và restart
docker-compose up -d --build

# Remove tất cả (cẩn thận!)
docker-compose down -v
```

## Lưu ý quan trọng

⚠️ **Trước khi chạy Docker:**

1. **Nâng cấp codebase:** Dockerfiles này giả định bạn đã nâng cấp từ .NET Framework 4.7.2 lên .NET 9
2. **Tách services:** Cần tạo các service projects riêng biệt từ controllers hiện tại
3. **Database:** Đảm bảo database accessible từ Docker containers
4. **Configuration:** Update connection strings trong `.env` file

Xem `DOCKER_MIGRATION_GUIDE.md` để biết chi tiết về migration process.

## Troubleshooting

### Service không start
```bash
docker-compose logs <service-name>
```

### Check network connectivity
```bash
docker network inspect external-account-network
```

### Test từng service trực tiếp
```bash
# Test auth service
curl http://localhost:5001/health

# Test customer service  
curl http://localhost:5002/health
```

## Production Deployment

1. Update `.env` với production values
2. Set `ASPNETCORE_ENVIRONMENT=Production`
3. Configure SSL/TLS cho API Gateway
4. Setup monitoring và logging
5. Configure backup và disaster recovery

Xem `DOCKER_MIGRATION_GUIDE.md` để biết thêm chi tiết về production setup.

