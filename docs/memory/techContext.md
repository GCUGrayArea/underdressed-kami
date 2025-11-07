# Technical Context - SmartScheduler

**Last Updated:** PR-001
**Purpose:** Architectural decisions, tech stack constraints, and setup details discovered during implementation

## Architectural Decisions

### UI Component Library
**Decision:** Material-UI (MUI)
**Rationale:** More popular than Ant Design, better documentation, larger ecosystem
**Impact:** All UI components will use MUI conventions

### Schema Design
**Decision:** Job Types as extensible database table (not enum)
**Rationale:** Allows adding new job types without code deployment
**Impact:** JobType entity with seeding for initial types (tile installer, carpet installer, hardwood specialist)

**Decision:** Human-readable IDs (CTR-001, JOB-001) alongside GUIDs
**Rationale:** Better UX for dispatchers while maintaining GUID benefits
**Implementation:**
- Primary key: GUID (Id)
- Display field: string FormattedId (e.g., "CTR-001")
- Auto-increment on insert using database sequence

**Decision:** Working hours as separate WeeklySchedule table
**Rationale:** Supports multiple time ranges per day, fully queryable
**Schema:**
```sql
WeeklySchedule {
  Id: GUID,
  ContractorId: GUID (FK),
  DayOfWeek: int (0=Sunday, 6=Saturday),
  StartTime: TimeOnly,
  EndTime: TimeOnly
}
```
**Example:** Contractor available Mon 9-12 and 2-5 = 2 rows

### AWS Deployment
**Decision:** Deferred - focus on local Docker development
**Rationale:** Core functionality first, deployment configuration later
**Current:** Docker Compose for PostgreSQL only
**Future:** Can add backend/frontend containers when ready to deploy

## Tech Stack Details

### Backend (.NET 8)
**Projects Created:**
- SmartScheduler.Domain (class library)
- SmartScheduler.Application (class library)
- SmartScheduler.Infrastructure (class library)
- SmartScheduler.WebApi (web API)

**Key Packages:**
- Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4
- Microsoft.EntityFrameworkCore.Design 9.0.10
- MediatR 13.1.0
- FluentValidation 12.1.0
- Serilog.AspNetCore 9.0.0

**Project References:**
- Application → Domain
- Infrastructure → Domain
- WebApi → Application, Infrastructure

### Frontend (React 18 + TypeScript)
**Build Tool:** Vite 7.1.7
**UI Library:** Material-UI (@mui/material, @mui/icons-material)
**State Management:**
- Server state: @tanstack/react-query
- UI state: React Context API
**Routing:** react-router-dom
**HTTP Client:** axios
**Real-time:** @microsoft/signalr

**Note:** npm install requires `--ignore-scripts` flag due to post-install script issues on Windows

### Database
**PostgreSQL 17** via Docker Compose
**Connection Details:**
- Host: localhost
- Port: 5432
- Database: smartscheduler
- User/Password: postgres/postgres (dev only)

## Development Environment Setup

### .NET CLI Path
Full path required on this system: `C:\Program Files\dotnet\dotnet.exe`
Standard PATH not working in Git Bash context

### Docker Compose
Located at project root: `docker-compose.yml`
Single service: PostgreSQL 15-alpine with persistent volume

### Environment Variables
Template: `.env.example`
**Critical vars:**
- DATABASE_URL: PostgreSQL connection string
- OPENROUTE_SERVICE_API_KEY: Must be obtained from openrouteservice.org
- VITE_API_BASE_URL: Frontend → Backend communication

## Default Values & Conventions

### Location Precision
**Default:** 6 decimal places for lat/lon (~0.1m precision)

### Rating Range
**Default:** Decimal(3,1) for 0.0-5.0 star ratings
**New contractors:** Default rating = 3.0

### Customer Reference
**Schema:** Simple string fields on Job entity
- CustomerId: string (could be GUID or external system ID)
- CustomerName: string

### Logging
**Framework:** Serilog with structured logging
**Minimum Level:** Information (configurable in appsettings.json)
**Sinks:** Console, File, Debug

## Known Constraints

### OpenRouteService API
**Rate Limit:** 40 requests/minute (free tier)
**Mitigation:** Mandatory caching for 24 hours per location pair
**Fallback:** Straight-line distance calculation if API unavailable

### SignalR
**Endpoint:** /hubs/scheduling
**Connection:** WebSocket (wss:// in production)
**Messages:** JobAssigned, ScheduleUpdated events

### No Authentication (MVP)
Current scope: Internal tool, trusted users
Design consideration: Plan for future auth hooks

## File Locations

**Backend Solution:** `src/backend/SmartScheduler.sln`
**Frontend Package:** `src/frontend/package.json`
**Docker Compose:** `docker-compose.yml` (root)
**Environment Template:** `.env.example` (root)

## Next Steps for PR-002

PR-002 will need to:
1. Create JobType entity and seed data
2. Implement FormattedId generation (database function or app-level)
3. Create WeeklySchedule entity with FK to Contractor
4. Set up EF Core DbContext with proper configurations
5. Generate initial migration

FormattedId implementation options:
- Option A: Database sequence with formatting function
- Option B: App-level service that queries max ID and increments
**Recommendation:** Option B for simplicity, can optimize later
