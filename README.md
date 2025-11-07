# SmartScheduler

An intelligent contractor discovery and scheduling system for the flooring industry. SmartScheduler automates the process of matching contractors to jobs based on availability, proximity, and performance metrics.

## Features

- **Intelligent Contractor Matching** - Weighted scoring algorithm ranks contractors by availability, rating, and distance
- **Real-Time Updates** - SignalR broadcasts job assignments to all connected dispatchers
- **Availability Engine** - Calculates open time slots considering working hours, existing jobs, and travel time
- **Distance Integration** - OpenRouteService API for accurate driving distance and time calculations
- **DDD/CQRS Architecture** - Clean separation of concerns with domain-driven design patterns
- **React Dashboard** - Material-UI based dispatcher interface for managing jobs and contractors

## Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- Docker and Docker Compose
- Git

## Quick Start

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd underdressed-kami
   ```

2. **Set up environment variables:**
   ```bash
   cp .env.example .env
   # Edit .env and add your OpenRouteService API key
   ```

3. **Start PostgreSQL database:**
   ```bash
   docker-compose up -d
   ```

4. **Run backend API:**
   ```bash
   cd src/backend
   dotnet restore
   dotnet build
   cd SmartScheduler.WebApi
   dotnet run
   ```
   Backend will be available at `http://localhost:5000`

5. **Run frontend (in a new terminal):**
   ```bash
   cd src/frontend
   npm install
   npm run dev
   ```
   Frontend will be available at `http://localhost:5173`

## Project Structure

```
src/
├── backend/
│   ├── SmartScheduler.Domain/        # Core business entities and value objects
│   ├── SmartScheduler.Application/   # CQRS commands, queries, handlers
│   ├── SmartScheduler.Infrastructure/ # EF Core, external APIs, persistence
│   └── SmartScheduler.WebApi/        # REST controllers, SignalR hubs
└── frontend/
    ├── src/
    │   ├── pages/                    # Main application pages
    │   ├── components/               # Reusable React components
    │   ├── api/                      # API client and methods
    │   └── hooks/                    # React Query hooks
    └── package.json
```

## Technology Stack

**Backend:**
- .NET 8 with ASP.NET Core Web API
- Entity Framework Core 8 with PostgreSQL
- MediatR (CQRS pattern)
- FluentValidation
- SignalR (real-time communication)
- Serilog (structured logging)

**Frontend:**
- React 18 with TypeScript
- Material-UI (MUI)
- React Router v6
- TanStack Query (React Query)
- Axios
- SignalR JavaScript client

**Infrastructure:**
- PostgreSQL 17
- Docker & Docker Compose
- OpenRouteService API

## Development

### Running Tests
```bash
cd src/backend
dotnet test
```

### Building for Production
```bash
# Backend
cd src/backend
dotnet publish -c Release

# Frontend
cd src/frontend
npm run build
```

### Database Migrations
```bash
cd src/backend/SmartScheduler.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../SmartScheduler.WebApi
dotnet ef database update --startup-project ../SmartScheduler.WebApi
```

## Documentation

- [Product Requirements Document](docs/prd.md) - Complete feature specifications
- [Task List](docs/task-list.md) - Development PRs and status
- [Technical Documentation](docs/technical-writeup.md) - Architecture and design decisions (coming in PR-030)

## Agent Coordination

This project uses multi-agent Claude Code coordination. See `.claude/rules/` for coordination policies.

Key commands:
- `/work` - Start working on next available PR
- `/status` - Check project status
- `/qc` - Run quality control checks