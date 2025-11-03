# Hướng dẫn Migration từ IIS sang Docker với Routing

## Tổng quan

Tài liệu này hướng dẫn cách tách ứng dụng External Accounting Website hiện đang chạy trên IIS thành các microservices độc lập chạy trong Docker containers và sử dụng API Gateway để routing.

## Kiến trúc mới

```
┌─────────────────┐
│   Client/Browser│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   API Gateway   │  (Port 80/443)
│   (YARP)        │
└────────┬────────┘
         │
    ┌────┴────┬──────────┬──────────┬──────────┬──────────┐
    │         │          │          │          │          │
    ▼         ▼          ▼          ▼          ▼          ▼
┌─────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│  Auth   │ │Customer  │ │  Report  │ │Statement ││UserControl│ │  WebApp  │
│ Service │ │ Service  │ │ Service  │ │ Service  │ │ Service  │ │   (UI)   │
│ :5001   │ │ :5002    │ │ :5003    │ │ :5004    │ │ :5005    │ │ :5006    │
└─────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘
```

## Các bước Migration

### Bước 1: Chuẩn bị môi trường

#### Yêu cầu:
- Docker Desktop hoặc Docker Engine
- Docker Compose v2.0+
- .NET 9 SDK (để build)
- Visual Studio 2022 hoặc VS Code

#### Cài đặt Docker:
```bash
# Kiểm tra Docker đã cài đặt
docker --version
docker-compose --version
```

### Bước 2: Nâng cấp từ .NET Framework lên .NET Core/.NET 9

**LƯU Ý QUAN TRỌNG:** Các Dockerfile này giả định bạn đã nâng cấp codebase từ .NET Framework 4.7.2 lên .NET 9.

#### Các thay đổi cần thiết:

1. **Thay đổi Project File:**
   ```xml
   <!-- Từ -->
   <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
   
   <!-- Thành -->
   <TargetFramework>net9.0</TargetFramework>
   ```

2. **Thay đổi Configuration:**
   - `Web.config` → `appsettings.json`
   - `Global.asax.cs` → `Program.cs`
   - Connection Strings → `appsettings.json`

3. **Cập nhật Dependencies:**
   - System.Web.* → Microsoft.AspNetCore.*
   - Enterprise Library → .NET Core equivalents

### Bước 3: Tách các Service

#### 3.1. Authentication Service

**Nguồn gốc:**
- `Fanex.ExternalAccount.Controllers/Authentication/`

**Tạo mới:**
- Tạo project mới: `AuthService/AuthService.csproj`
- Copy controllers từ `Authentication/` folder
- Chuyển đổi sang Web API

**Dockerfile:** `Dockerfile.AuthService`

**Routes trong Gateway:**
- `/api/auth/*` → Auth Service
- `/Account/*` → Auth Service

#### 3.2. Customer Service

**Nguồn gốc:**
- `Fanex.ExternalAccount.Controllers/Customer/`
- `Fanex.ExternalAccount.Controllers/Home/`

**Tạo mới:**
- Tạo project: `CustomerService/CustomerService.csproj`
- Copy controllers và business logic

**Routes:**
- `/api/customer/*` → Customer Service
- `/Customer/*` → Customer Service
- `/Home/*` → Customer Service
- `/DataRange/*` → Customer Service

#### 3.3. Report Service

**Nguồn gốc:**
- `Fanex.ExternalAccount.Controllers/Report/`
- `Fanex.ExternalAccount.Controllers/TotalBet/`

**Routes:**
- `/api/report/*` → Report Service
- `/DailyWinloss/*` → Report Service
- `/WinlossDetail/*` → Report Service
- `/TotalBet/*` → Report Service

#### 3.4. Statement Service

**Nguồn gốc:**
- `Fanex.ExternalAccount.Controllers/Statement/`

**Routes:**
- `/api/statement/*` → Statement Service
- `/Statement/*` → Statement Service

#### 3.5. User Control Service

**Nguồn gốc:**
- `Fanex.ExternalAccount.Controllers/UserControl/`

**Routes:**
- `/api/usercontrol/*` → User Control Service
- `/User/*` → User Control Service
- `/Group/*` → User Control Service
- `/SubGroup/*` → User Control Service
- `/Function/*` → User Control Service
- `/Menu/*` → User Control Service

#### 3.6. WebApp (Fanex.ExternalAccount.UI)

**Nguồn gốc:**
- `Fanex.ExternalAccount.UI/` - Main web application with MVC views

**Tạo mới:**
- Project đã migrate: `netcore/src/Fanex.ExternalAccount.UI/`
- ASP.NET Core MVC application với Views, Areas, Controllers

**Routes:**
- `/` → WebApp (Home page)
- `/Home/*` → WebApp
- Tất cả MVC routes → WebApp (nếu không match với API routes)
- Static files (CSS, JS, Images) → WebApp

### Bước 4: Cấu hình API Gateway

API Gateway sử dụng **YARP (Yet Another Reverse Proxy)** để routing.

#### Cài đặt YARP:
```bash
dotnet add package Yarp.ReverseProxy
```

#### Cấu hình routing trong `appsettings.json`:
Xem file `ApiGateway/appsettings.json` để biết chi tiết cấu hình routing.

### Bước 5: Build và Deploy với Docker

#### 5.1. Build Images:

```bash
# Build tất cả services
docker-compose build

# Hoặc build từng service
docker build -f Dockerfile.AuthService -t external-account-auth:latest .
docker build -f Dockerfile.CustomerService -t external-account-customer:latest .
docker build -f Dockerfile.ReportService -t external-account-report:latest .
docker build -f Dockerfile.StatementService -t external-account-statement:latest .
docker build -f Dockerfile.UserControlService -t external-account-usercontrol:latest .
docker build -f Dockerfile.WebApp -t external-account-webapp:latest .
docker build -f Dockerfile.ApiGateway -t external-account-gateway:latest .
```

#### 5.2. Chạy với Docker Compose:

```bash
# Tạo file .env với các biến môi trường
cat > .env << EOF
DB_CONNECTION_STRING=Server=your-db-server;Database=ExternalAccount;User Id=user;Password=pass;
REDIS_CONNECTION_STRING=your-redis-connection-string
EOF

# Chạy tất cả services
docker-compose up -d

# Xem logs
docker-compose logs -f

# Kiểm tra status
docker-compose ps
```

#### 5.3. Health Checks:

```bash
# Check gateway
curl http://localhost/health

# Check auth service
curl http://localhost:5001/health

# Check customer service
curl http://localhost:5002/health

# Check webapp
curl http://localhost:5006/health
```

### Bước 6: Testing

#### Test Routing:

```bash
# Test Auth Service routing
curl http://localhost/api/auth/login
curl http://localhost/Account/Login

# Test Customer Service routing
curl http://localhost/api/customer/list
curl http://localhost/Customer/CustomerList

# Test Report Service routing
curl http://localhost/api/report/daily
curl http://localhost/DailyWinloss/Index

# Test WebApp routing
curl http://localhost/
curl http://localhost/Home/Index
curl http://localhost:5006
```

### Bước 7: Migration từ IIS (Từng bước)

#### Phase 1: Parallel Running (IIS + Docker)

1. Chạy Docker containers trên ports khác (5001-5005)
2. Test từng service độc lập
3. Verify functionality tương đương với IIS

#### Phase 2: Gradual Migration

1. Bắt đầu với service ít quan trọng nhất (ví dụ: User Control)
2. Route một phần traffic từ IIS sang Docker
3. Monitor và compare performance
4. Nếu ổn định, chuyển toàn bộ

#### Phase 3: Full Migration

1. Setup API Gateway trên port 80
2. Stop IIS application
3. Start Docker containers
4. Update DNS/load balancer để point tới API Gateway

### Bước 8: Production Considerations

#### 8.1. Environment Variables

Tạo file `.env.production`:
```bash
DB_CONNECTION_STRING=Server=prod-db;Database=ExternalAccount;Integrated Security=true;
REDIS_CONNECTION_STRING=prod-redis:6379
ASPNETCORE_ENVIRONMENT=Production
```

#### 8.2. Security

1. **HTTPS:**
   - Cấu hình SSL certificates cho API Gateway
   - Sử dụng reverse proxy với SSL termination

2. **Network Isolation:**
   - Services chỉ communicate qua internal network
   - Chỉ API Gateway expose ra ngoài

3. **Authentication:**
   - JWT tokens được validate tại Gateway
   - Pass tokens xuống các services

#### 8.3. Monitoring

Thêm vào `docker-compose.yml`:
```yaml
services:
  prometheus:
    image: prom/prometheus
    # ... config
  
  grafana:
    image: grafana/grafana
    # ... config
```

#### 8.4. Logging

Configure centralized logging:
```yaml
services:
  api-gateway:
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

## Troubleshooting

### Lỗi thường gặp:

1. **Service không start:**
   ```bash
   docker-compose logs <service-name>
   ```

2. **Routing không hoạt động:**
   - Kiểm tra `appsettings.json` routing config
   - Verify service URLs trong clusters
   - Check network connectivity: `docker network inspect`

3. **Database connection:**
   - Verify connection string
   - Check network access từ containers
   - Test với `docker exec -it <container> bash`

4. **Port conflicts:**
   - Đổi ports trong `docker-compose.yml`
   - Hoặc stop services đang dùng ports

## Tài liệu tham khảo

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [ASP.NET Core Migration Guide](https://docs.microsoft.com/en-us/aspnet/core/migration/)
- [Microservices Patterns](https://microservices.io/patterns/)

## Kết luận

Migration từ IIS sang Docker với microservices architecture mang lại:
- ✅ Scalability: Scale từng service độc lập
- ✅ Maintainability: Dễ maintain và deploy
- ✅ Technology Flexibility: Sử dụng công nghệ phù hợp
- ✅ Fault Isolation: Lỗi không ảnh hưởng toàn bộ
- ✅ Development Speed: Teams phát triển độc lập

Tuy nhiên cần lưu ý:
- ⚠️ Complexity tăng lên
- ⚠️ Network latency giữa services
- ⚠️ Data consistency challenges
- ⚠️ Testing complexity

Khuyến nghị: Migration từng bước, test kỹ lưỡng trước khi chuyển toàn bộ production traffic.

