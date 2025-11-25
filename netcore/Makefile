# Fanex External Account Reports - Docker Commands

.PHONY: help build up down logs clean dev prod test

# Default target
help: ## Show this help message
	@echo "Fanex External Account Reports - Docker Commands"
	@echo ""
	@echo "Available commands:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-20s %s\n", $$1, $$2}'

# Development commands
dev-build: ## Build development environment
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml build

dev-up: ## Start development environment
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d

dev-down: ## Stop development environment
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml down

dev-logs: ## View development logs
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml logs -f

dev-restart: ## Restart development environment
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml restart

# Production commands
prod-build: ## Build production environment
	docker-compose -f docker-compose.yml -f docker-compose.prod.yml build

prod-up: ## Start production environment
	docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

prod-down: ## Stop production environment
	docker-compose -f docker-compose.yml -f docker-compose.prod.yml down

prod-logs: ## View production logs
	docker-compose -f docker-compose.yml -f docker-compose.prod.yml logs -f

# General commands
build: ## Build all services
	docker-compose build

up: ## Start all services
	docker-compose up -d

down: ## Stop all services
	docker-compose down

logs: ## View all logs
	docker-compose logs -f

restart: ## Restart all services
	docker-compose restart

status: ## Show status of all services
	docker-compose ps

clean: ## Remove all containers, volumes, and images
	docker-compose down -v --rmi all

# Database commands
db-connect: ## Connect to SQL Server container
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd

db-backup: ## Backup database (usage: make db-backup DB_NAME=your_db)
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U sa -P YourStrong!Passw0rd \
		-Q "BACKUP DATABASE [$(DB_NAME)] TO DISK = '/var/opt/mssql/backup/$(DB_NAME).bak'"

# Utility commands
shell: ## Access application container shell
	docker-compose exec fanex-external-account-reports /bin/bash

redis-cli: ## Access Redis CLI
	docker-compose exec redis redis-cli -a yourpassword

health: ## Check health of all services
	docker-compose ps
	@echo ""
	@echo "Container Health Status:"
	@docker-compose exec fanex-external-account-reports curl -f http://localhost/health 2>/dev/null && echo "✅ Web App: Healthy" || echo "❌ Web App: Unhealthy"
	@docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -Q "SELECT 1" 2>/dev/null && echo "✅ SQL Server: Healthy" || echo "❌ SQL Server: Unhealthy"
	@docker-compose exec redis redis-cli -a yourpassword ping 2>/dev/null && echo "✅ Redis: Healthy" || echo "❌ Redis: Unhealthy"

# Testing commands
test-unit: ## Run unit tests
	dotnet test --logger "console;verbosity=detailed"

test-integration: ## Run integration tests
	dotnet test --filter "Category=Integration" --logger "console;verbosity=detailed"

# Deployment commands
deploy-dev: dev-build dev-up ## Build and deploy to development
	@echo "Development environment deployed successfully!"
	@echo "Application URL: http://localhost:5000"

deploy-prod: prod-build prod-up ## Build and deploy to production
	@echo "Production environment deployed successfully!"
	@echo "Application URL: http://localhost:8080"

# Maintenance commands
update-images: ## Update all Docker images
	docker-compose pull

backup-db: ## Create database backup
	@echo "Creating database backup..."
	@mkdir -p backups
	@docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U sa -P YourStrong!Passw0rd \
		-Q "BACKUP DATABASE [bodb02] TO DISK = '/var/opt/mssql/backup/bodb02_$(shell date +%Y%m%d_%H%M%S).bak'"
	@docker cp $$(docker-compose ps -q sqlserver):/var/opt/mssql/backup/ ./backups/
	@echo "Database backup completed!"

# Information commands
info: ## Show environment information
	@echo "=== Docker Environment Information ==="
	@echo "Docker version: $$(docker --version)"
	@echo "Docker Compose version: $$(docker-compose --version)"
	@echo ""
	@echo "=== Container Status ==="
	@docker-compose ps
	@echo ""
	@echo "=== Volume Status ==="
	@docker volume ls | grep fanex
	@echo ""
	@echo "=== Network Status ==="
	@docker network ls | grep fanex

# Security commands
security-scan: ## Run security scan on containers
	@echo "Running security scan..."
	@docker-compose exec fanex-external-account-reports apt-get update && apt-get install -y clamav
	@docker-compose exec fanex-external-account-reports freshclam
	@docker-compose exec fanex-external-account-reports clamscan --recursive --infected /app
	@echo "Security scan completed!"
