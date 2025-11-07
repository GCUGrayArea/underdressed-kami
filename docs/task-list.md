# Task List for SmartScheduler

## Block 1: Foundation & Infrastructure (No dependencies)

### PR-001: Project Setup and Configuration
**Status:** Complete
**Agent:** White
**Dependencies:** None
**Priority:** High

**Description:**
Initialize the .NET 8 backend and React frontend projects with necessary dependencies, folder structure following DDD/CQRS architecture, and development tooling. Set up Docker configuration for local PostgreSQL development.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.sln (create) - Solution file
- src/backend/SmartScheduler.Domain/SmartScheduler.Domain.csproj (create) - Domain layer project
- src/backend/SmartScheduler.Application/SmartScheduler.Application.csproj (create) - Application layer project
- src/backend/SmartScheduler.Infrastructure/SmartScheduler.Infrastructure.csproj (create) - Infrastructure layer project
- src/backend/SmartScheduler.WebApi/SmartScheduler.WebApi.csproj (create) - Web API project
- src/backend/SmartScheduler.WebApi/Program.cs (create) - Application entry point
- src/backend/SmartScheduler.WebApi/appsettings.json (create) - Configuration
- src/frontend/package.json (create) - Frontend dependencies
- src/frontend/vite.config.ts (create) - Vite configuration
- src/frontend/tsconfig.json (create) - TypeScript configuration
- docker-compose.yml (create) - PostgreSQL container for local dev
- .env.example (create) - Template for environment variables
- README.md (modify) - Setup instructions

**Acceptance Criteria:**
- [x] .NET 8 solution builds successfully with all four projects
- [x] React frontend starts with Vite dev server on port 5173
- [x] Docker compose starts PostgreSQL container on port 5432
- [x] All necessary NuGet packages installed (EF Core, MediatR, FluentValidation, Serilog)
- [x] All necessary npm packages installed (React, React Router, Axios, Material-UI, SignalR client)
- [x] Folder structure follows DDD layers (Domain, Application, Infrastructure, WebApi)
- [x] Environment variables template documented in .env.example

**Notes:**
This is the foundation for all subsequent work. Ensure proper layer references (WebApi → Application → Domain, Infrastructure → Domain).

---

### PR-002: Database Schema and EF Core Configuration
**Status:** Complete
**Agent:** Orange
**Dependencies:** PR-001
**Priority:** High

**Description:**
Define PostgreSQL database schema using EF Core Code First approach. Create domain entities (Contractor, Job, Schedule) with proper relationships, value objects, and EF Core entity configurations. Generate and apply initial migration.

**Files (Refined during Planning by Orange):**
- src/backend/SmartScheduler.Domain/Class1.cs (delete) - Remove template file
- src/backend/SmartScheduler.Application/Class1.cs (delete) - Remove template file
- src/backend/SmartScheduler.Infrastructure/Class1.cs (delete) - Remove template file
- src/backend/SmartScheduler.Domain/Entities/JobType.cs (create) - Job type entity
- src/backend/SmartScheduler.Domain/Entities/Contractor.cs (create) - Contractor entity with FormattedId
- src/backend/SmartScheduler.Domain/Entities/Job.cs (create) - Job entity with FormattedId
- src/backend/SmartScheduler.Domain/Entities/WeeklySchedule.cs (create) - Working hours entity
- src/backend/SmartScheduler.Domain/ValueObjects/Location.cs (create) - Location value object
- src/backend/SmartScheduler.Domain/Interfaces/IContractorRepository.cs (create) - Repository interface
- src/backend/SmartScheduler.Domain/Interfaces/IJobRepository.cs (create) - Repository interface
- src/backend/SmartScheduler.Infrastructure/Persistence/ApplicationDbContext.cs (create) - EF Core context
- src/backend/SmartScheduler.Infrastructure/Persistence/Configurations/JobTypeConfiguration.cs (create) - EF configuration
- src/backend/SmartScheduler.Infrastructure/Persistence/Configurations/ContractorConfiguration.cs (create) - EF configuration
- src/backend/SmartScheduler.Infrastructure/Persistence/Configurations/JobConfiguration.cs (create) - EF configuration
- src/backend/SmartScheduler.Infrastructure/Persistence/Configurations/WeeklyScheduleConfiguration.cs (create) - EF configuration
- src/backend/SmartScheduler.Infrastructure/Persistence/Seeds/JobTypeSeed.cs (create) - Seed initial job types
- src/backend/SmartScheduler.Infrastructure/Migrations/[timestamp]_InitialCreate.cs (create) - Initial migration

**Acceptance Criteria:**
- [x] Domain entities follow DDD principles with encapsulation
- [x] Value objects are immutable and include equality comparisons
- [x] EF Core configurations define relationships, indexes, and constraints
- [x] Initial migration generates correct SQL schema
- [x] Migration applies successfully to PostgreSQL database
- [x] Database includes proper indexes on foreign keys and common query fields
- [x] No navigation properties in domain layer that create tight coupling

**Notes:**
Keep domain entities rich with behavior, not anemic. Value objects should validate their invariants in constructors.

---

## Block 2: Core Domain Logic (Depends on: Block 1)

### PR-003: Implement Contractor Management Commands
**Status:** New
**Dependencies:** PR-001, PR-002
**Priority:** High

**Description:**
Implement CQRS commands for contractor CRUD operations: CreateContractorCommand, UpdateContractorCommand, DeactivateContractorCommand. Include command handlers, FluentValidation validators, and repository interfaces.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Application/Commands/CreateContractorCommand.cs (create) - Create command
- src/backend/SmartScheduler.Application/Commands/UpdateContractorCommand.cs (create) - Update command
- src/backend/SmartScheduler.Application/Commands/DeactivateContractorCommand.cs (create) - Deactivate command
- src/backend/SmartScheduler.Application/CommandHandlers/CreateContractorCommandHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/CommandHandlers/UpdateContractorCommandHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/CommandHandlers/DeactivateContractorCommandHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/Validators/CreateContractorCommandValidator.cs (create) - Validator
- src/backend/SmartScheduler.Application/Validators/UpdateContractorCommandValidator.cs (create) - Validator
- src/backend/SmartScheduler.Domain/Interfaces/IContractorRepository.cs (create) - Repository interface
- src/backend/SmartScheduler.Infrastructure/Persistence/Repositories/ContractorRepository.cs (create) - Repository implementation

**Acceptance Criteria:**
- [ ] Commands validate inputs using FluentValidation (name length, rating range, etc.)
- [ ] Command handlers use MediatR pattern
- [ ] Repository interface defined in Domain layer, implementation in Infrastructure
- [ ] Handlers return appropriate success/error results
- [ ] Commands publish domain events (ContractorCreated, ContractorUpdated)
- [ ] Code follows 75-line function limit

**Notes:**
Use Result pattern for command responses rather than throwing exceptions for business rule violations.

---

### PR-004: Implement Contractor Management Queries
**Status:** New
**Dependencies:** PR-001, PR-002
**Priority:** High

**Description:**
Implement CQRS queries for contractor retrieval: GetContractorByIdQuery, GetAllContractorsQuery, SearchContractorsQuery. Include query handlers and DTOs for read operations.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Application/Queries/GetContractorByIdQuery.cs (create) - Get by ID query
- src/backend/SmartScheduler.Application/Queries/GetAllContractorsQuery.cs (create) - List query
- src/backend/SmartScheduler.Application/Queries/SearchContractorsQuery.cs (create) - Search query
- src/backend/SmartScheduler.Application/QueryHandlers/GetContractorByIdQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/QueryHandlers/GetAllContractorsQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/QueryHandlers/SearchContractorsQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/DTOs/ContractorDto.cs (create) - Read DTO
- src/backend/SmartScheduler.Application/DTOs/ContractorListDto.cs (create) - List DTO

**Acceptance Criteria:**
- [ ] Queries return DTOs, not domain entities (CQRS separation)
- [ ] Search query supports filtering by type, rating, and active status
- [ ] Query handlers use read-optimized queries (no unnecessary joins)
- [ ] Queries include pagination support for list operations
- [ ] DTOs include all necessary data for UI display

**Notes:**
Queries can run in parallel with PR-003 as they don't conflict. Consider using AutoMapper or Mapster for entity-to-DTO mapping.

---

### PR-005: Implement Job Management Commands and Queries
**Status:** New
**Dependencies:** PR-001, PR-002
**Priority:** High

**Description:**
Implement CQRS commands and queries for job management: CreateJobCommand, UpdateJobCommand, AssignContractorCommand, and corresponding queries for job retrieval and listing.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Application/Commands/CreateJobCommand.cs (create) - Create command
- src/backend/SmartScheduler.Application/Commands/UpdateJobCommand.cs (create) - Update command
- src/backend/SmartScheduler.Application/Commands/AssignContractorCommand.cs (create) - Assign command
- src/backend/SmartScheduler.Application/CommandHandlers/CreateJobCommandHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/CommandHandlers/UpdateJobCommandHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/CommandHandlers/AssignContractorCommandHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/Validators/CreateJobCommandValidator.cs (create) - Validator
- src/backend/SmartScheduler.Application/Validators/AssignContractorCommandValidator.cs (create) - Validator
- src/backend/SmartScheduler.Application/Queries/GetJobByIdQuery.cs (create) - Get by ID query
- src/backend/SmartScheduler.Application/Queries/GetJobsByStatusQuery.cs (create) - Filter query
- src/backend/SmartScheduler.Application/QueryHandlers/GetJobByIdQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/QueryHandlers/GetJobsByStatusQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/DTOs/JobDto.cs (create) - Job DTO
- src/backend/SmartScheduler.Domain/Interfaces/IJobRepository.cs (create) - Repository interface
- src/backend/SmartScheduler.Infrastructure/Persistence/Repositories/JobRepository.cs (create) - Repository implementation

**Acceptance Criteria:**
- [ ] CreateJobCommand validates job type matches predefined types
- [ ] AssignContractorCommand validates contractor exists and job type matches
- [ ] Commands publish domain events (JobCreated, JobAssigned)
- [ ] Queries support filtering by status (unassigned, assigned, completed)
- [ ] Queries include contractor details when job is assigned
- [ ] Validation prevents assigning contractor to job with mismatched type

**Notes:**
AssignContractorCommand will be enhanced in later PRs to check availability. For now, just establish the domain command structure.

---

## Block 3: Availability Engine (Depends on: Block 2)

### PR-006: Implement Availability Engine Core Logic
**Status:** New
**Dependencies:** PR-003, PR-004, PR-005
**Priority:** High

**Description:**
Implement the availability engine that calculates open time slots for contractors based on their working hours and existing job assignments. This is core domain logic for the scheduling system.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Domain/Services/AvailabilityService.cs (create) - Domain service
- src/backend/SmartScheduler.Domain/Services/IAvailabilityService.cs (create) - Service interface
- src/backend/SmartScheduler.Application/Queries/GetContractorAvailabilityQuery.cs (create) - Availability query
- src/backend/SmartScheduler.Application/QueryHandlers/GetContractorAvailabilityQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/DTOs/AvailabilityDto.cs (create) - Availability DTO
- src/backend/SmartScheduler.Application/DTOs/TimeSlotDto.cs (create) - Time slot DTO

**Acceptance Criteria:**
- [ ] Service calculates available time slots based on working hours
- [ ] Service excludes time slots occupied by existing jobs
- [ ] Service accounts for job duration when finding gaps
- [ ] Edge case: Returns empty list when contractor not working on target date
- [ ] Edge case: Returns empty list when all slots occupied
- [ ] Query handler retrieves contractor and jobs, delegates to service
- [ ] Functions decomposed to stay under 75-line limit

**Notes:**
This is complex logic. Break into helper functions: GetWorkingHoursForDate, GetOccupiedSlots, FindGaps. Consider time zone handling carefully.

---

### PR-007: Unit Tests for Availability Engine
**Status:** New
**Dependencies:** PR-006
**Priority:** High

**Description:**
Comprehensive unit tests for availability engine covering various scenarios: normal availability, no availability, edge cases, boundary conditions.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Tests/Domain/Services/AvailabilityServiceTests.cs (create) - Test suite
- src/backend/SmartScheduler.Tests/SmartScheduler.Tests.csproj (create) - Test project

**Acceptance Criteria:**
- [ ] Test: Contractor with no existing jobs shows full working hours as available
- [ ] Test: Contractor with back-to-back jobs shows no availability
- [ ] Test: Contractor with gap between jobs shows correct available slot
- [ ] Test: Contractor not working on target date returns empty availability
- [ ] Test: Job duration longer than gaps excludes those gaps
- [ ] Test: Boundary condition at start/end of working hours handled correctly
- [ ] All tests pass with >80% code coverage for AvailabilityService

**Notes:**
Use xUnit with FluentAssertions for readable test assertions. Mock repository to return test data.

---

## Block 4: Distance & Mapping Integration (Depends on: Block 1)

### PR-008: OpenRouteService Integration
**Status:** New
**Dependencies:** PR-001
**Priority:** High

**Description:**
Integrate with OpenRouteService API to calculate driving distance and travel time between locations. Implement caching and error handling with fallback to straight-line distance.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Infrastructure/ExternalServices/IDistanceCalculator.cs (create) - Service interface
- src/backend/SmartScheduler.Infrastructure/ExternalServices/OpenRouteServiceClient.cs (create) - API client
- src/backend/SmartScheduler.Infrastructure/ExternalServices/DistanceCalculator.cs (create) - Main service
- src/backend/SmartScheduler.Infrastructure/ExternalServices/DistanceCache.cs (create) - Caching layer
- src/backend/SmartScheduler.Infrastructure/ExternalServices/Models/RouteRequest.cs (create) - Request model
- src/backend/SmartScheduler.Infrastructure/ExternalServices/Models/RouteResponse.cs (create) - Response model

**Acceptance Criteria:**
- [ ] Successfully calls OpenRouteService API with coordinates
- [ ] Parses distance (meters) and duration (seconds) from response
- [ ] Caches results for 24 hours using location pair as key
- [ ] Falls back to straight-line distance calculation if API unavailable
- [ ] Implements exponential backoff on rate limit errors
- [ ] API key loaded from environment variable (not hardcoded)
- [ ] Includes detailed logging for API calls and errors

**Notes:**
OpenRouteService free tier has 40 requests/minute limit. Caching is critical. Consider implementing request queuing if hitting rate limits.

---

### PR-009: Unit Tests for Distance Calculator
**Status:** New
**Dependencies:** PR-008
**Priority:** Medium

**Description:**
Unit tests for distance calculator including successful API calls, cache hits, error scenarios, and fallback behavior.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Tests/Infrastructure/ExternalServices/DistanceCalculatorTests.cs (create) - Test suite
- src/backend/SmartScheduler.Tests/Infrastructure/ExternalServices/OpenRouteServiceClientTests.cs (create) - Client tests

**Acceptance Criteria:**
- [ ] Test: Successful API call returns correct distance and duration
- [ ] Test: Cache hit avoids API call
- [ ] Test: API unavailable triggers fallback to straight-line distance
- [ ] Test: Invalid coordinates handled gracefully
- [ ] Test: Rate limit error triggers exponential backoff
- [ ] All tests pass with mocked HTTP client

**Notes:**
Use Moq to mock HttpClient. Consider using WireMock.NET for more realistic HTTP mocking.

---

## Block 5: Scoring & Ranking Engine (Depends on: Block 3, Block 4)

### PR-010: Implement Scoring Algorithm
**Status:** New
**Dependencies:** PR-006, PR-008
**Priority:** High

**Description:**
Implement the weighted scoring algorithm that ranks contractors based on availability, rating, and distance. This is the core intelligence of the scheduling system.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Domain/Services/ScoringService.cs (create) - Scoring domain service
- src/backend/SmartScheduler.Domain/Services/IScoringService.cs (create) - Service interface
- src/backend/SmartScheduler.Domain/ValueObjects/ScoringWeights.cs (create) - Weights configuration
- src/backend/SmartScheduler.Domain/ValueObjects/ContractorScore.cs (create) - Score result
- src/backend/SmartScheduler.Application/Queries/GetRankedContractorsQuery.cs (create) - Ranking query
- src/backend/SmartScheduler.Application/QueryHandlers/GetRankedContractorsQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/DTOs/RankedContractorDto.cs (create) - Result DTO
- src/backend/SmartScheduler.Application/DTOs/ScoreBreakdownDto.cs (create) - Score details DTO

**Acceptance Criteria:**
- [ ] Calculates availability score: 1.0 for within 30min, 0.7 for within 2hrs, 0.4 same day
- [ ] Calculates rating score: normalized rating/5.0
- [ ] Calculates distance score: 1.0 for <10mi, linear decay 10-30mi, 0.2 for 30-50mi, 0.0 for >50mi
- [ ] Applies weights: 40% availability, 30% rating, 30% distance (configurable)
- [ ] Filters contractors by matching job type
- [ ] Returns top N contractors sorted by score (default N=5)
- [ ] Includes score breakdown in response for transparency

**Notes:**
Decompose scoring logic: CalculateAvailabilityScore, CalculateRatingScore, CalculateDistanceScore as separate methods. Make weights configurable via value object.

---

### PR-011: Unit Tests for Scoring Algorithm
**Status:** New
**Dependencies:** PR-010
**Priority:** High

**Description:**
Comprehensive unit tests validating scoring algorithm correctness with various input combinations and edge cases.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Tests/Domain/Services/ScoringServiceTests.cs (create) - Test suite

**Acceptance Criteria:**
- [ ] Test: Perfect match (available on time, 5-star, 5mi) gets score near 1.0
- [ ] Test: Contractor with no availability gets score 0.0 and excluded
- [ ] Test: Distance >50mi gets excluded from results
- [ ] Test: Ranking sorts correctly by weighted score
- [ ] Test: Score breakdown matches individual component calculations
- [ ] Test: Weights can be adjusted and affect ranking order
- [ ] Test: Edge case with same scores maintains stable sort order
- [ ] All tests pass with >80% coverage for ScoringService

**Notes:**
Use theory tests (xUnit [Theory]) to test multiple scoring scenarios efficiently.

---

## Block 6: REST API Endpoints (Depends on: Block 5)

### PR-012: Contractor Management API Endpoints
**Status:** New
**Dependencies:** PR-003, PR-004
**Priority:** High

**Description:**
Implement REST API endpoints for contractor management: POST /api/contractors, GET /api/contractors, GET /api/contractors/{id}, PUT /api/contractors/{id}, DELETE /api/contractors/{id}.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.WebApi/Controllers/ContractorsController.cs (create) - REST controller
- src/backend/SmartScheduler.WebApi/Models/CreateContractorRequest.cs (create) - Request model
- src/backend/SmartScheduler.WebApi/Models/UpdateContractorRequest.cs (create) - Request model
- src/backend/SmartScheduler.WebApi/Models/ContractorResponse.cs (create) - Response model

**Acceptance Criteria:**
- [ ] POST /api/contractors creates contractor and returns 201 Created
- [ ] GET /api/contractors returns paginated list with filtering
- [ ] GET /api/contractors/{id} returns contractor or 404
- [ ] PUT /api/contractors/{id} updates contractor and returns 200 OK
- [ ] DELETE /api/contractors/{id} soft-deletes contractor
- [ ] Validation errors return 400 Bad Request with error details
- [ ] Controller delegates to MediatR commands/queries (thin controller)

**Notes:**
Use [ApiController] attribute for automatic model validation. Return problem details format for errors.

---

### PR-013: Job Management API Endpoints
**Status:** New
**Dependencies:** PR-005
**Priority:** High

**Description:**
Implement REST API endpoints for job management: POST /api/jobs, GET /api/jobs, GET /api/jobs/{id}, PUT /api/jobs/{id}, POST /api/jobs/{id}/assign.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.WebApi/Controllers/JobsController.cs (create) - REST controller
- src/backend/SmartScheduler.WebApi/Models/CreateJobRequest.cs (create) - Request model
- src/backend/SmartScheduler.WebApi/Models/UpdateJobRequest.cs (create) - Request model
- src/backend/SmartScheduler.WebApi/Models/AssignContractorRequest.cs (create) - Request model
- src/backend/SmartScheduler.WebApi/Models/JobResponse.cs (create) - Response model

**Acceptance Criteria:**
- [ ] POST /api/jobs creates job and returns 201 Created
- [ ] GET /api/jobs returns jobs filtered by status
- [ ] GET /api/jobs/{id} returns job with contractor details if assigned
- [ ] PUT /api/jobs/{id} updates job details
- [ ] POST /api/jobs/{id}/assign assigns contractor to job
- [ ] Assignment validates contractor availability before accepting
- [ ] All endpoints return appropriate HTTP status codes

**Notes:**
Job assignment endpoint should trigger domain events for SignalR broadcasting (implemented in later PR).

---

### PR-014: Contractor Recommendation API Endpoint
**Status:** New
**Dependencies:** PR-010
**Priority:** High

**Description:**
Implement the primary API endpoint POST /api/recommendations/contractors that accepts job requirements and returns ranked list of contractors with scores and available time slots.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.WebApi/Controllers/RecommendationsController.cs (create) - REST controller
- src/backend/SmartScheduler.WebApi/Models/ContractorRecommendationRequest.cs (create) - Request model
- src/backend/SmartScheduler.WebApi/Models/ContractorRecommendationResponse.cs (create) - Response model

**Acceptance Criteria:**
- [ ] POST /api/recommendations/contractors accepts job type, date, time, location, duration
- [ ] Returns ranked list of top 5 contractors with scores and available slots
- [ ] Response includes score breakdown for each contractor
- [ ] Returns 404 if no contractors match job type
- [ ] Returns 200 with empty array if no contractors available on date
- [ ] Response time <500ms for typical request (5-10 contractors in DB)
- [ ] Includes proper CORS configuration for frontend

**Notes:**
This is the most critical endpoint for the user experience. Performance logging essential. Consider async/await throughout call chain.

---

## Block 7: Event System & SignalR (Depends on: Block 6)

### PR-015: Domain Event Infrastructure
**Status:** New
**Dependencies:** PR-001
**Priority:** High

**Description:**
Implement in-memory message bus using MediatR for domain events. Define domain events (JobAssigned, ScheduleUpdated, ContractorRated) and event handlers.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Domain/Events/DomainEvent.cs (create) - Base event class
- src/backend/SmartScheduler.Domain/Events/JobAssignedEvent.cs (create) - Event definition
- src/backend/SmartScheduler.Domain/Events/ScheduleUpdatedEvent.cs (create) - Event definition
- src/backend/SmartScheduler.Domain/Events/ContractorRatedEvent.cs (create) - Event definition
- src/backend/SmartScheduler.Application/EventHandlers/JobAssignedEventHandler.cs (create) - Handler
- src/backend/SmartScheduler.Infrastructure/Messaging/EventDispatcher.cs (create) - Event dispatcher

**Acceptance Criteria:**
- [ ] Domain events published via MediatR when commands complete
- [ ] Event handlers process events asynchronously
- [ ] Events include timestamp and relevant entity IDs
- [ ] Event dispatcher logs all events for audit trail
- [ ] Multiple handlers can subscribe to same event
- [ ] Events do not block command completion (fire and forget)

**Notes:**
Design with future migration to distributed message broker in mind (AWS SQS). Keep event schemas simple and serializable.

---

### PR-016: SignalR Hub Implementation
**Status:** New
**Dependencies:** PR-015
**Priority:** High

**Description:**
Implement SignalR hub for real-time communication with dispatcher and contractor clients. Hub broadcasts domain events to connected clients.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.WebApi/Hubs/SchedulingHub.cs (create) - SignalR hub
- src/backend/SmartScheduler.WebApi/Hubs/ISchedulingClient.cs (create) - Client interface
- src/backend/SmartScheduler.Application/EventHandlers/SignalRBroadcastEventHandler.cs (create) - Event→SignalR bridge
- src/backend/SmartScheduler.WebApi/Program.cs (modify) - Configure SignalR services

**Acceptance Criteria:**
- [ ] SignalR hub mounted at /hubs/scheduling
- [ ] JobAssigned events broadcast to all connected clients
- [ ] ScheduleUpdated events broadcast to all connected clients
- [ ] Clients receive strongly-typed messages (ReceiveJobAssigned, ReceiveScheduleUpdated)
- [ ] Connection handling includes automatic reconnection logic
- [ ] CORS configured to allow frontend connections
- [ ] Hub methods include connection lifecycle logging

**Notes:**
Keep hub thin - just broadcasting, no business logic. Event handlers convert domain events to SignalR messages.

---

## Block 8: Frontend Foundation (Depends on: Block 1)

### PR-017: React Application Structure and Routing
**Status:** New
**Dependencies:** PR-001
**Priority:** High

**Description:**
Set up React application structure with routing, layout components, navigation, and Material-UI theme configuration.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/main.tsx (create) - Application entry point
- src/frontend/src/App.tsx (create) - Root component
- src/frontend/src/router.tsx (create) - React Router configuration
- src/frontend/src/theme.ts (create) - Material-UI theme
- src/frontend/src/layouts/MainLayout.tsx (create) - Main layout with nav
- src/frontend/src/pages/Dashboard.tsx (create) - Dashboard placeholder
- src/frontend/src/pages/Contractors.tsx (create) - Contractors list placeholder
- src/frontend/src/components/Navigation.tsx (create) - Navigation component
- src/frontend/index.html (create) - HTML template

**Acceptance Criteria:**
- [ ] React app renders in browser at localhost:5173
- [ ] Routing configured with routes for dashboard, contractors, jobs
- [ ] Navigation bar displays with links to main pages
- [ ] Material-UI theme applied consistently
- [ ] Layout responsive for desktop and tablet
- [ ] TypeScript compilation succeeds with strict mode

**Notes:**
Use React Router v6 with lazy loading for code splitting. Set up Material-UI with custom color scheme matching brand (if applicable).

---

### PR-018: API Client and React Query Setup
**Status:** New
**Dependencies:** PR-017
**Priority:** High

**Description:**
Set up Axios HTTP client with interceptors and React Query for server state management. Configure base URL, error handling, and query defaults.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/api/client.ts (create) - Axios client configuration
- src/frontend/src/api/contractors.ts (create) - Contractor API methods
- src/frontend/src/api/jobs.ts (create) - Job API methods
- src/frontend/src/api/recommendations.ts (create) - Recommendation API methods
- src/frontend/src/hooks/useContractors.ts (create) - React Query hooks
- src/frontend/src/hooks/useJobs.ts (create) - React Query hooks
- src/frontend/src/providers/QueryProvider.tsx (create) - React Query provider
- src/frontend/src/types/api.ts (create) - API type definitions

**Acceptance Criteria:**
- [ ] Axios client configured with backend base URL from environment
- [ ] Request interceptor adds correlation ID for logging
- [ ] Response interceptor handles errors consistently
- [ ] React Query provider wraps app with proper defaults
- [ ] TypeScript types defined for all API request/response shapes
- [ ] API methods use React Query hooks (useQuery, useMutation)

**Notes:**
Use React Query for automatic caching, background refetching, and optimistic updates. Define API types matching backend DTOs.

---

### PR-019: SignalR Client Integration
**Status:** New
**Dependencies:** PR-017
**Priority:** High

**Description:**
Integrate SignalR JavaScript client for real-time updates from backend. Set up connection management, event handlers, and React context for SignalR state.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/services/signalr.ts (create) - SignalR connection service
- src/frontend/src/contexts/SignalRContext.tsx (create) - SignalR context provider
- src/frontend/src/hooks/useSignalR.ts (create) - Hook for accessing SignalR

**Acceptance Criteria:**
- [ ] SignalR connection established to /hubs/scheduling on app load
- [ ] Connection state tracked (connecting, connected, disconnected)
- [ ] Automatic reconnection on disconnect
- [ ] Event listeners can be registered via useSignalR hook
- [ ] Connection errors logged and displayed to user
- [ ] TypeScript types for server messages

**Notes:**
Connection should be singleton managed by context. Expose event subscription API that components can use with useEffect.

---

## Block 9: Frontend Features - Contractor Management (Depends on: Block 8, Block 6)

### PR-020: Contractor List Page
**Status:** New
**Dependencies:** PR-017, PR-018, PR-012
**Priority:** High

**Description:**
Build contractor list page with search, filtering, and ability to navigate to create/edit contractor forms.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/pages/Contractors.tsx (modify) - Contractor list implementation
- src/frontend/src/components/contractors/ContractorTable.tsx (create) - Table component
- src/frontend/src/components/contractors/ContractorFilters.tsx (create) - Filter controls
- src/frontend/src/components/contractors/ContractorSearch.tsx (create) - Search input

**Acceptance Criteria:**
- [ ] Displays contractors in table with name, type, rating, status
- [ ] Search filters contractors by name
- [ ] Filters by type, rating range, and active/inactive status
- [ ] Clicking contractor row navigates to detail/edit page
- [ ] "Add Contractor" button navigates to create form
- [ ] Loading state while fetching data
- [ ] Empty state when no contractors match filters

**Notes:**
Use Material-UI Table component with sorting. Debounce search input to avoid excessive API calls.

---

### PR-021: Contractor Create/Edit Form
**Status:** New
**Dependencies:** PR-017, PR-018, PR-012
**Priority:** High

**Description:**
Build form for creating new contractors and editing existing ones. Include validation, working hours schedule editor, and success/error handling.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/pages/ContractorForm.tsx (create) - Form page
- src/frontend/src/components/contractors/ContractorFormFields.tsx (create) - Form fields
- src/frontend/src/components/contractors/WorkingHoursEditor.tsx (create) - Schedule editor
- src/frontend/src/hooks/useContractorForm.ts (create) - Form state management
- src/frontend/src/utils/validation.ts (create) - Form validation helpers

**Acceptance Criteria:**
- [ ] Form fields for name, type, rating, base location, contact info
- [ ] Working hours editor allows setting daily start/end times for each day of week
- [ ] Client-side validation matches backend rules
- [ ] Submit creates/updates contractor via API
- [ ] Success message and redirect to list on save
- [ ] Error messages displayed for validation failures
- [ ] Cancel button returns to list without saving

**Notes:**
Use React Hook Form for form state management. Working hours editor should support setting multiple time ranges per day in future enhancement.

---

## Block 10: Frontend Features - Job Dashboard (Depends on: Block 8, Block 6)

### PR-022: Job Dashboard Layout
**Status:** New
**Dependencies:** PR-017, PR-018, PR-013
**Priority:** High

**Description:**
Build main dashboard showing unassigned jobs and assigned jobs grouped by date. This is the primary dispatcher view.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/pages/Dashboard.tsx (modify) - Dashboard implementation
- src/frontend/src/components/jobs/JobList.tsx (create) - Job list component
- src/frontend/src/components/jobs/JobCard.tsx (create) - Individual job card
- src/frontend/src/components/jobs/JobStatusBadge.tsx (create) - Status indicator

**Acceptance Criteria:**
- [ ] Displays unassigned jobs in priority queue (sorted by desired date)
- [ ] Displays assigned jobs grouped by scheduled date
- [ ] Each job card shows type, location, desired date/time, status
- [ ] Clicking job card opens detail modal or navigates to detail page
- [ ] Auto-refreshes job list every 30 seconds
- [ ] Real-time updates via SignalR when jobs assigned
- [ ] Loading skeleton while fetching

**Notes:**
Use React Query's polling or SignalR updates to keep dashboard fresh. Consider card vs. table layout for jobs.

---

### PR-023: Job Creation Form
**Status:** New
**Dependencies:** PR-017, PR-018, PR-013
**Priority:** Medium

**Description:**
Build form for creating new job requests with type, desired date/time, location, and estimated duration.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/pages/CreateJob.tsx (create) - Create job page
- src/frontend/src/components/jobs/JobFormFields.tsx (create) - Form fields
- src/frontend/src/components/common/LocationInput.tsx (create) - Location picker
- src/frontend/src/hooks/useJobForm.ts (create) - Form state management

**Acceptance Criteria:**
- [ ] Form fields for job type (dropdown), desired date, desired time, location, duration
- [ ] Date picker for desired date
- [ ] Time picker for desired time
- [ ] Location input with address validation
- [ ] Client-side validation (required fields, future dates only)
- [ ] Submit creates job and redirects to dashboard
- [ ] Error handling for API failures

**Notes:**
Location input could integrate with geocoding API in future. For now, accept address text and coordinates.

---

## Block 11: Frontend Features - Contractor Recommendations (Depends on: Block 9, Block 10, Block 6)

### PR-024: Contractor Recommendation Modal
**Status:** New
**Dependencies:** PR-022, PR-014
**Priority:** High

**Description:**
Build modal that displays recommended contractors for a selected job, showing scores, available time slots, and assignment action.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/components/recommendations/RecommendationModal.tsx (create) - Modal component
- src/frontend/src/components/recommendations/ContractorRecommendationCard.tsx (create) - Contractor card
- src/frontend/src/components/recommendations/ScoreBreakdown.tsx (create) - Score visualization
- src/frontend/src/components/recommendations/TimeSlotPicker.tsx (create) - Slot selector
- src/frontend/src/hooks/useRecommendations.ts (create) - Hook for fetching recommendations

**Acceptance Criteria:**
- [ ] Modal triggered when dispatcher clicks "Find Contractor" on unassigned job
- [ ] Fetches recommendations from API based on job details
- [ ] Displays top 5 contractors with name, rating, distance, score
- [ ] Shows score breakdown (availability, rating, distance) visually
- [ ] Lists available time slots for each contractor
- [ ] "Assign" button for each contractor/slot combination
- [ ] Loading state while fetching recommendations
- [ ] Error state if no contractors available

**Notes:**
Use Material-UI Dialog for modal. Consider bar chart or progress bars for score breakdown visualization.

---

### PR-025: Job Assignment Flow
**Status:** New
**Dependencies:** PR-024, PR-013, PR-016
**Priority:** High

**Description:**
Implement the assignment flow: dispatcher selects contractor and time slot, confirms assignment, backend processes, and UI updates in real-time.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/components/recommendations/AssignmentConfirmation.tsx (create) - Confirmation dialog
- src/frontend/src/hooks/useJobAssignment.ts (create) - Assignment mutation hook
- src/frontend/src/components/jobs/JobCard.tsx (modify) - Add real-time update handling
- src/frontend/src/pages/Dashboard.tsx (modify) - SignalR event listener

**Acceptance Criteria:**
- [ ] Clicking "Assign" shows confirmation dialog with contractor and slot details
- [ ] Confirmation calls POST /api/jobs/{id}/assign
- [ ] Optimistic update shows job as assigned immediately
- [ ] SignalR event confirms assignment and triggers dashboard refresh
- [ ] Success notification displayed to dispatcher
- [ ] Error handling if assignment fails (contractor no longer available)
- [ ] Job moves from unassigned to assigned list in real-time

**Notes:**
Use React Query's optimistic updates for instant feedback. SignalR ensures all connected dispatchers see the update.

---

## Block 12: Integration Testing (Depends on: Block 11)

### PR-026: End-to-End Integration Tests for Recommendation Flow
**Status:** New
**Dependencies:** PR-014, PR-025
**Priority:** High

**Description:**
Comprehensive integration tests validating the full recommendation and assignment workflow from API to database.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.IntegrationTests/RecommendationFlowTests.cs (create) - Test suite
- src/backend/SmartScheduler.IntegrationTests/TestFixture.cs (create) - Test infrastructure
- src/backend/SmartScheduler.IntegrationTests/SmartScheduler.IntegrationTests.csproj (create) - Test project

**Acceptance Criteria:**
- [ ] Test: Create contractors with different locations and schedules
- [ ] Test: Create job and request recommendations
- [ ] Test: Verify ranking order matches expected scores
- [ ] Test: Assign top contractor and verify job status updated
- [ ] Test: Verify contractor availability updated after assignment
- [ ] Test: Verify SignalR event published on assignment
- [ ] All tests pass against real PostgreSQL database (Docker)
- [ ] Tests use WebApplicationFactory for in-memory API testing

**Notes:**
Use xUnit with WebApplicationFactory for integration tests. Seed test database with known data for predictable assertions.

---

### PR-027: Frontend Integration Tests
**Status:** New
**Dependencies:** PR-025
**Priority:** Medium

**Description:**
Integration tests for frontend using Vitest and React Testing Library, covering key user flows.

**Files (ESTIMATED - will be refined during Planning):**
- src/frontend/src/tests/ContractorManagement.test.tsx (create) - Contractor CRUD tests
- src/frontend/src/tests/JobAssignment.test.tsx (create) - Assignment flow tests
- src/frontend/src/tests/setup.ts (create) - Test setup and mocks

**Acceptance Criteria:**
- [ ] Test: Create new contractor via form and verify in list
- [ ] Test: Search/filter contractors
- [ ] Test: Create job and see it in unassigned list
- [ ] Test: Request recommendations and see ranked contractors
- [ ] Test: Assign contractor and verify job moves to assigned list
- [ ] All tests pass with mocked API responses

**Notes:**
Use MSW (Mock Service Worker) for mocking API calls. Focus on user flows, not implementation details.

---

## Block 13: Deployment & Infrastructure (Depends on: Block 12)

### PR-028: Docker Configuration for Production
**Status:** New
**Dependencies:** PR-001
**Priority:** Medium

**Description:**
Create Dockerfiles for backend and frontend, docker-compose for local full-stack testing, and AWS deployment preparation.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/Dockerfile (create) - Backend container
- src/frontend/Dockerfile (create) - Frontend container
- docker-compose.prod.yml (create) - Full stack compose
- .dockerignore (create) - Exclude unnecessary files
- deploy/README.md (create) - Deployment instructions

**Acceptance Criteria:**
- [ ] Backend Dockerfile builds .NET app with multi-stage build
- [ ] Frontend Dockerfile builds React app and serves with nginx
- [ ] docker-compose.prod.yml runs full stack (backend, frontend, PostgreSQL)
- [ ] Environment variables configured via .env file
- [ ] Health check endpoints implemented for containers
- [ ] Images optimized for size (use alpine where possible)
- [ ] Deployment README documents AWS deployment steps

**Notes:**
Use multi-stage Docker builds to minimize image size. Document AWS ECS/Fargate deployment or AWS App Runner option.

---

### PR-029: CI/CD Pipeline with GitHub Actions
**Status:** New
**Dependencies:** PR-028
**Priority:** Medium

**Description:**
Set up GitHub Actions workflows for automated testing, building, and deployment.

**Files (ESTIMATED - will be refined during Planning):**
- .github/workflows/backend-ci.yml (create) - Backend CI pipeline
- .github/workflows/frontend-ci.yml (create) - Frontend CI pipeline
- .github/workflows/deploy.yml (create) - Deployment workflow

**Acceptance Criteria:**
- [ ] Backend CI runs on push: build, test, lint
- [ ] Frontend CI runs on push: build, test, lint, type-check
- [ ] Tests run against PostgreSQL service container
- [ ] Deployment workflow triggers on main branch merge
- [ ] Docker images built and pushed to registry (ECR or Docker Hub)
- [ ] Failed builds block PR merges (if branch protection enabled)

**Notes:**
Consider separate workflows for CI (fast feedback) vs. CD (deployment). Use caching for dependencies to speed up builds.

---

## Block 14: Documentation & Polish (Depends on: All previous blocks)

### PR-030: Technical Documentation and Architecture Writeup
**Status:** New
**Dependencies:** All implementation PRs (PR-001 through PR-029)
**Priority:** Medium

**Description:**
Create comprehensive technical documentation including DDD model explanation, CQRS structure, scoring algorithm details, and system architecture diagrams.

**Files (ESTIMATED - will be refined during Planning):**
- docs/technical-writeup.md (create) - Main technical document
- docs/architecture.md (create) - Detailed architecture documentation
- docs/scoring-algorithm.md (create) - Scoring algorithm explanation with examples
- docs/diagrams/system-architecture.mmd (create) - Mermaid diagram
- docs/diagrams/domain-model.mmd (create) - Mermaid diagram
- docs/diagrams/assignment-flow.mmd (create) - Sequence diagram

**Documentation Requirements:**

The technical documentation should include:

1. **System Architecture**
   - High-level architecture overview with diagram
   - Technology stack and rationale
   - Layer separation (Domain, Application, Infrastructure, WebApi)
   - Integration points (OpenRouteService, SignalR, PostgreSQL)

2. **Domain-Driven Design Model**
   - Core entities: Contractor, Job, Assignment
   - Value objects: Location, TimeSlot, WorkingHours, ScoringWeights
   - Domain services: AvailabilityService, ScoringService
   - Aggregate boundaries and consistency rules

3. **CQRS Structure**
   - Command examples: CreateContractorCommand, AssignJobCommand
   - Query examples: GetRankedContractorsQuery, GetContractorAvailabilityQuery
   - Handler pattern with MediatR
   - Validation pipeline with FluentValidation

4. **Scoring Algorithm Detailed Explanation**
   - Weighted formula breakdown
   - Component score calculations with examples
   - Default weights rationale (40% availability, 30% rating, 30% distance)
   - Edge cases and tie-breaking

5. **Availability Engine Logic**
   - Working hours interpretation
   - Time slot calculation algorithm
   - Travel time buffer handling
   - Conflict detection between jobs

6. **Event-Driven Architecture**
   - Domain events: JobAssigned, ScheduleUpdated, ContractorRated
   - Event handlers and side effects
   - SignalR integration for real-time updates
   - Future migration path to distributed message broker

7. **Visual Diagrams** (Mermaid syntax)
   - System architecture diagram showing all components
   - Domain model diagram with entities and relationships
   - Sequence diagram for job assignment flow
   - Sequence diagram for contractor recommendation flow

8. **Performance Characteristics**
   - Recommendation API latency targets and actual measurements
   - Caching strategies (distance calculations)
   - Database query optimization approaches
   - Scalability considerations

**Acceptance Criteria:**
- [ ] Technical writeup is 2-4 pages covering DDD, CQRS, and scoring algorithm
- [ ] Architecture document comprehensively describes system design
- [ ] Scoring algorithm document includes formula, examples, and edge cases
- [ ] All diagrams render correctly in markdown viewers using Mermaid
- [ ] Diagrams accurately reflect implemented system (not idealized design)
- [ ] A developer unfamiliar with the codebase can understand system design
- [ ] Documentation references actual code files with line numbers where relevant

**Notes:**
This is a 60-90 minute task. Read through completed PRs, review actual implementation, synthesize understanding, and document clearly for new developers. Focus on WHAT was built and WHY design decisions were made.

---

### PR-031: README and Setup Documentation
**Status:** New
**Dependencies:** PR-030
**Priority:** Medium

**Description:**
Update README with project overview, setup instructions, running the application, and links to detailed documentation.

**Files (ESTIMATED - will be refined during Planning):**
- README.md (modify) - Main README
- docs/setup-guide.md (create) - Detailed setup instructions
- docs/api-reference.md (create) - API endpoint documentation

**Acceptance Criteria:**
- [ ] README includes project overview and key features
- [ ] README has quick start instructions (prerequisites, clone, setup, run)
- [ ] Setup guide covers environment variables, database setup, API keys
- [ ] API reference documents all endpoints with request/response examples
- [ ] Troubleshooting section for common issues
- [ ] Links to technical documentation and architecture docs

**Notes:**
Make README approachable for new developers. Include screenshots of UI if possible.

---

## Block 15: AI Enhancement (Post-MVP) (Depends on: Block 14)

### PR-032: OpenAI Integration for Ranking Explanations
**Status:** New
**Dependencies:** PR-030
**Priority:** Low (Post-MVP Enhancement)

**Description:**
Integrate OpenAI API to generate natural language explanations for why a contractor was ranked #1, demonstrating advanced AI usage.

**Files (ESTIMATED - will be refined during Planning):**
- src/backend/SmartScheduler.Infrastructure/ExternalServices/IExplanationGenerator.cs (create) - Service interface
- src/backend/SmartScheduler.Infrastructure/ExternalServices/OpenAIExplanationGenerator.cs (create) - OpenAI client
- src/backend/SmartScheduler.Application/Queries/GetRankedContractorsWithExplanationsQuery.cs (create) - Enhanced query
- src/backend/SmartScheduler.Application/QueryHandlers/GetRankedContractorsWithExplanationsQueryHandler.cs (create) - Handler
- src/backend/SmartScheduler.Application/DTOs/RankedContractorWithExplanationDto.cs (create) - DTO with explanation
- src/frontend/src/components/recommendations/ContractorRecommendationCard.tsx (modify) - Display explanation

**Acceptance Criteria:**
- [ ] OpenAI API integration configured with API key from environment
- [ ] Prompt engineering: Pass score breakdown and contractor details to GPT
- [ ] Generate concise 1-2 sentence explanation for top contractor
- [ ] Explanation reflects actual scoring factors (availability, rating, distance)
- [ ] Fallback to no explanation if OpenAI API unavailable
- [ ] Response time remains <500ms (parallel OpenAI call or async generation)
- [ ] Frontend displays explanation in recommendation card
- [ ] Documentation includes example prompts and LLM configuration

**Notes:**
Use GPT-3.5-turbo for cost efficiency. Consider caching explanations for same contractor/job combinations. This demonstrates LLM usage but isn't critical path.

---

### PR-033: AI Documentation and Demo
**Status:** New
**Dependencies:** PR-032
**Priority:** Low (Post-MVP Enhancement)

**Description:**
Document AI integration approach, example prompts, and create demo showcasing AI-generated explanations.

**Files (ESTIMATED - will be refined during Planning):**
- docs/ai-integration.md (create) - AI usage documentation
- docs/ai-prompts.md (create) - Example prompts and responses

**Acceptance Criteria:**
- [ ] Documentation explains why AI was used for explanations
- [ ] Example prompts shown with actual GPT responses
- [ ] Justification for how AI enhances system (transparency, user trust)
- [ ] Discussion of limitations (API cost, latency, reliability)
- [ ] Alternative approaches considered (template-based explanations)

**Notes:**
This satisfies "detailed documentation of AI tools used" requirement. Emphasize that AI augments but doesn't replace deterministic scoring.

---

## Summary

**Total PRs:** 33
**Dependency Blocks:** 15

**Block Parallelization Opportunities:**
- Block 2 (PR-003, PR-004, PR-005) can run partially in parallel after Block 1
- Block 4 (PR-008, PR-009) can run in parallel with Block 2 and Block 3
- Block 8 (PR-017, PR-018, PR-019) can start as soon as Block 1 completes
- Block 9 and Block 10 (frontend features) can run in parallel after Block 8 + corresponding backend blocks

**Critical Path:**
Block 1 → Block 2 → Block 3 → Block 5 → Block 6 → Block 11 → Block 12 → Block 13 → Block 14

**Estimated Timeline:**
- Blocks 1-7: Days 1-4 (Backend core)
- Blocks 8-11: Days 5-6 (Frontend)
- Blocks 12-14: Day 7 (Testing, deployment, documentation)
- Block 15: Post-MVP (AI enhancement, 4-8 hours additional)
