# SmartScheduler Deployment Guide

This guide covers deploying SmartScheduler to production environments using Docker containers.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Local Production Testing](#local-production-testing)
- [AWS Deployment Options](#aws-deployment-options)
  - [Option 1: AWS ECS with Fargate](#option-1-aws-ecs-with-fargate)
  - [Option 2: AWS App Runner](#option-2-aws-app-runner)
  - [Option 3: AWS Lightsail](#option-3-aws-lightsail)
- [Environment Configuration](#environment-configuration)
- [Database Migration Strategy](#database-migration-strategy)
- [Monitoring and Logging](#monitoring-and-logging)
- [Troubleshooting](#troubleshooting)

## Prerequisites

- Docker 24.0 or higher
- Docker Compose 2.20 or higher
- AWS CLI 2.0 or higher (for AWS deployment)
- OpenRouteService API key ([get one here](https://openrouteservice.org/dev/#/signup))
- PostgreSQL database (local Docker or AWS RDS)

## Local Production Testing

Before deploying to AWS, test the full stack locally using the production Docker configuration.

### Step 1: Configure Environment Variables

Copy the production environment template:

```bash
cp .env.production.example .env.production
```

Edit `.env.production` with your configuration:

```bash
# Database
POSTGRES_PASSWORD=your_secure_password_here

# OpenRouteService API
OPENROUTESERVICE_API_KEY=your_api_key_here

# Frontend URL (for local testing)
VITE_API_BASE_URL=http://localhost:8080
CORS_ORIGINS=http://localhost
```

### Step 2: Build Docker Images

Build all containers:

```bash
docker-compose -f docker-compose.prod.yml build
```

This will:
- Build the .NET 8 backend with multi-stage optimization
- Build the React frontend and package with nginx
- Pull PostgreSQL 17 Alpine image

**Expected build time:** 5-10 minutes on first build (faster on subsequent builds due to layer caching)

### Step 3: Start the Application

Run the full stack:

```bash
docker-compose -f docker-compose.prod.yml up
```

Or run in detached mode:

```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Step 4: Verify Health

Check that all services are healthy:

```bash
docker-compose -f docker-compose.prod.yml ps
```

All services should show `healthy` status.

Test health endpoints:
- Backend: http://localhost:8080/health
- Frontend: http://localhost/health
- Application: http://localhost

### Step 5: Apply Database Migrations

The backend container will automatically apply Entity Framework migrations on startup. To manually apply migrations:

```bash
docker-compose -f docker-compose.prod.yml exec backend dotnet ef database update
```

### Step 6: Stop the Application

```bash
docker-compose -f docker-compose.prod.yml down
```

To remove volumes (database data):

```bash
docker-compose -f docker-compose.prod.yml down -v
```

## AWS Deployment Options

SmartScheduler can be deployed to AWS using several services. Choose based on your requirements:

| Service | Best For | Complexity | Cost |
|---------|----------|------------|------|
| ECS Fargate | Production workloads, scalability | Medium | Medium |
| App Runner | Simple deployments, auto-scaling | Low | Medium-High |
| Lightsail | Small workloads, fixed pricing | Low | Low |

### Option 1: AWS ECS with Fargate

**Recommended for:** Production workloads with scaling requirements

#### Architecture

- **Frontend:** ECS Service with Application Load Balancer
- **Backend:** ECS Service with Application Load Balancer
- **Database:** RDS PostgreSQL with Multi-AZ
- **Container Registry:** Amazon ECR

#### Prerequisites

- AWS account with appropriate IAM permissions
- AWS CLI configured (`aws configure`)
- ECR repositories created

#### Step 1: Create ECR Repositories

```bash
# Create backend repository
aws ecr create-repository \
    --repository-name smartscheduler/backend \
    --region us-east-1

# Create frontend repository
aws ecr create-repository \
    --repository-name smartscheduler/frontend \
    --region us-east-1
```

#### Step 2: Authenticate Docker to ECR

```bash
aws ecr get-login-password --region us-east-1 | \
    docker login --username AWS --password-stdin <YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com
```

#### Step 3: Build and Tag Images

```bash
# Build and tag backend
cd src/backend
docker build -t smartscheduler-backend:latest .
docker tag smartscheduler-backend:latest \
    <YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/smartscheduler/backend:latest

# Build and tag frontend
cd ../frontend
docker build --build-arg VITE_API_BASE_URL=https://api.yourdomain.com -t smartscheduler-frontend:latest .
docker tag smartscheduler-frontend:latest \
    <YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/smartscheduler/frontend:latest
```

#### Step 4: Push Images to ECR

```bash
docker push <YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/smartscheduler/backend:latest
docker push <YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/smartscheduler/frontend:latest
```

#### Step 5: Create RDS PostgreSQL Database

```bash
aws rds create-db-instance \
    --db-instance-identifier smartscheduler-db \
    --db-instance-class db.t3.micro \
    --engine postgres \
    --engine-version 15.4 \
    --master-username postgres \
    --master-user-password <YOUR_SECURE_PASSWORD> \
    --allocated-storage 20 \
    --vpc-security-group-ids <YOUR_SECURITY_GROUP_ID> \
    --db-subnet-group-name <YOUR_DB_SUBNET_GROUP> \
    --backup-retention-period 7 \
    --multi-az \
    --publicly-accessible false
```

#### Step 6: Configure ECS Task Definitions

Create task definitions for backend and frontend services. See `deploy/aws/ecs-task-definition-*.json` for examples.

#### Step 7: Create ECS Cluster and Services

```bash
# Create ECS cluster
aws ecs create-cluster --cluster-name smartscheduler-cluster

# Create services (see AWS Console or CloudFormation templates in deploy/aws/)
```

#### Step 8: Configure Application Load Balancer

- Create ALB with HTTPS listener (port 443)
- Configure target groups for backend (port 8080) and frontend (port 80)
- Set up health check paths: `/health` for both services
- Configure SSL certificate (use AWS Certificate Manager)

### Option 2: AWS App Runner

**Recommended for:** Simplified deployments with automatic scaling

#### Prerequisites

- Backend and frontend images pushed to ECR (see Option 1, Steps 1-4)

#### Step 1: Create App Runner Services

**Backend:**

```bash
aws apprunner create-service \
    --service-name smartscheduler-backend \
    --source-configuration '{
        "ImageRepository": {
            "ImageIdentifier": "<YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/smartscheduler/backend:latest",
            "ImageRepositoryType": "ECR",
            "ImageConfiguration": {
                "Port": "8080",
                "RuntimeEnvironmentVariables": {
                    "ASPNETCORE_ENVIRONMENT": "Production",
                    "ConnectionStrings__DefaultConnection": "<RDS_CONNECTION_STRING>",
                    "OPENROUTESERVICE_API_KEY": "<YOUR_API_KEY>"
                }
            }
        }
    }' \
    --instance-configuration '{
        "Cpu": "1 vCPU",
        "Memory": "2 GB"
    }' \
    --health-check-configuration '{
        "Protocol": "HTTP",
        "Path": "/health",
        "Interval": 10,
        "Timeout": 5,
        "HealthyThreshold": 1,
        "UnhealthyThreshold": 5
    }'
```

**Frontend:**

```bash
aws apprunner create-service \
    --service-name smartscheduler-frontend \
    --source-configuration '{
        "ImageRepository": {
            "ImageIdentifier": "<YOUR_AWS_ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com/smartscheduler/frontend:latest",
            "ImageRepositoryType": "ECR",
            "ImageConfiguration": {
                "Port": "80"
            }
        }
    }' \
    --instance-configuration '{
        "Cpu": "0.25 vCPU",
        "Memory": "0.5 GB"
    }' \
    --health-check-configuration '{
        "Protocol": "HTTP",
        "Path": "/health",
        "Interval": 10,
        "Timeout": 5,
        "HealthyThreshold": 1,
        "UnhealthyThreshold": 5
    }'
```

### Option 3: AWS Lightsail

**Recommended for:** Small workloads with predictable traffic

Use Lightsail Container Services for a simplified deployment with fixed monthly pricing.

1. Push images to a public container registry (Docker Hub or public ECR)
2. Create Lightsail container service via AWS Console
3. Deploy backend and frontend containers
4. Set up Lightsail managed database for PostgreSQL

See [AWS Lightsail documentation](https://aws.amazon.com/lightsail/features/) for detailed steps.

## Environment Configuration

### Required Environment Variables

**Backend:**

| Variable | Description | Example |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=db;Port=5432;Database=smartscheduler;Username=postgres;Password=***` |
| `OPENROUTESERVICE_API_KEY` | OpenRouteService API key | `5b3ce3597851110001cf6248abcdef...` |
| `CORS__AllowedOrigins` | Frontend URL for CORS | `https://app.yourdomain.com` |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |
| `Logging__LogLevel__Default` | Minimum log level | `Warning` |

**Frontend:**

| Variable | Description | Example |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Backend API URL | `https://api.yourdomain.com` |

### Secrets Management

For production, use AWS Secrets Manager or Parameter Store instead of environment variables:

```bash
# Store database password
aws secretsmanager create-secret \
    --name smartscheduler/db-password \
    --secret-string "your_secure_password"

# Store API key
aws secretsmanager create-secret \
    --name smartscheduler/openrouteservice-key \
    --secret-string "your_api_key"
```

Update ECS task definition or App Runner service to reference secrets.

## Database Migration Strategy

### Automatic Migrations (Development/Staging)

The backend container applies EF Core migrations automatically on startup. This is suitable for development and staging environments.

### Manual Migrations (Production)

For production, apply migrations manually before deploying new backend versions:

**Option 1: Run migration from local machine**

```bash
# Set connection string
export ConnectionStrings__DefaultConnection="Host=<RDS_ENDPOINT>;Port=5432;Database=smartscheduler;Username=postgres;Password=***"

# Apply migrations
cd src/backend/SmartScheduler.WebApi
dotnet ef database update
```

**Option 2: Run migration from ECS task**

```bash
# Start one-off ECS task with migration command
aws ecs run-task \
    --cluster smartscheduler-cluster \
    --task-definition smartscheduler-backend \
    --overrides '{
        "containerOverrides": [{
            "name": "backend",
            "command": ["dotnet", "ef", "database", "update"]
        }]
    }'
```

## Monitoring and Logging

### Health Checks

Both backend and frontend expose `/health` endpoints:

- **Backend:** Returns JSON with status and timestamp
- **Frontend:** Returns "healthy" text

### Application Logs

Logs are written to stdout/stderr and can be viewed in:

- **ECS:** CloudWatch Logs
- **App Runner:** CloudWatch Logs (automatic)
- **Local Docker:** `docker-compose logs -f`

### Metrics

Consider setting up:
- CloudWatch Container Insights for ECS
- Application Performance Monitoring (APM) tools (New Relic, Datadog, etc.)
- Custom CloudWatch metrics for business KPIs

## Troubleshooting

### Backend container fails to start

**Check logs:**

```bash
docker-compose -f docker-compose.prod.yml logs backend
```

**Common issues:**
- Missing `OPENROUTESERVICE_API_KEY` environment variable
- Database connection failure (check `ConnectionStrings__DefaultConnection`)
- Port already in use (change `BACKEND_PORT` in `.env.production`)

### Frontend shows blank page

**Check console errors:**
- Verify `VITE_API_BASE_URL` matches backend URL
- Check CORS configuration in backend
- Ensure backend is healthy: `curl http://localhost:8080/health`

### Database migrations fail

**Check database connectivity:**

```bash
docker-compose -f docker-compose.prod.yml exec backend \
    dotnet ef database update --verbose
```

**Manual migration:**

```bash
cd src/backend/SmartScheduler.WebApi
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Container health check failing

**Check health endpoint:**

```bash
# Backend
curl http://localhost:8080/health

# Frontend
curl http://localhost/health
```

**Increase health check timeouts:**

Edit `docker-compose.prod.yml` and increase `start_period` values.

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS App Runner Documentation](https://docs.aws.amazon.com/apprunner/)
- [Entity Framework Core Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

## Support

For issues or questions, please refer to the main project README or create an issue in the repository.
