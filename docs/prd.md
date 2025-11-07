# Product Requirements Document: SmartScheduler

## Product Overview

### Brief Description
SmartScheduler is an intelligent contractor discovery and scheduling system for the flooring industry. It automates the process of matching contractors to jobs based on availability, proximity, and performance metrics, transforming manual scheduling into an optimized, data-driven operation.

### Problem Statement
Current manual scheduling processes in the flooring industry result in:
- Significant time wasted on phone calls and manual coordination
- Scheduling errors and double-bookings
- Underutilized contractor capacity
- Slow response times to customer job requests
- Suboptimal contractor-job matching based on gut feeling rather than data

### Target Users
- **Primary:** Dispatchers who assign contractors to flooring jobs
- **Secondary:** Contractors who receive job assignments
- **Tertiary:** Operations managers who monitor system performance

### Success Criteria

**Quantitative Metrics:**
- 40% reduction in manual scheduling time
- 25% improvement in contractor utilization rate
- 20% faster average job assignment time
- Sub-500ms response time for contractor recommendations

**Qualitative Metrics:**
- Improved customer satisfaction through faster, more accurate matching
- Reduced dispatcher cognitive load
- Better contractor-job fit based on objective criteria

## Functional Requirements

### 1. Contractor Management (CRUD)

**Core Attributes:**
- Contractor ID (unique identifier)
- Name (full legal name)
- Type (e.g., "tile installer", "carpet installer", "hardwood specialist")
- Rating (0.0-5.0 scale, based on historical job performance)
- Base Location (address or coordinates)
- Working Hours Schedule (recurring weekly schedule)
- Contact Information (phone, email)
- Active/Inactive Status

**Operations:**
- Create new contractor profiles with validation
- Read/list contractors with filtering and search
- Update contractor information, including schedule and rating
- Soft-delete contractors (mark inactive, maintain history)

**Validation Rules:**
- Name: Required, 2-100 characters
- Type: Required, must match predefined job types
- Rating: 0.0-5.0, defaults to 3.0 for new contractors
- Base Location: Required, must be valid address or coordinates
- Working Hours: Must define at least one available time slot

### 2. Job Management

**Core Attributes:**
- Job ID (unique identifier)
- Job Type (must match contractor types)
- Desired Date/Time (when customer wants service)
- Location (job site address or coordinates)
- Status (unassigned, assigned, in-progress, completed, cancelled)
- Estimated Duration (hours)
- Customer Reference (ID or name)
- Assigned Contractor (nullable, references contractor)

**Operations:**
- Create job requests
- Update job details
- Assign/reassign contractors to jobs
- Track job status changes
- Query jobs by status, contractor, date range

### 3. Availability Engine

**Core Logic:**
The availability engine determines when a contractor has open time slots for new jobs.

**Inputs:**
- Contractor's working hours schedule (recurring weekly)
- Contractor's existing assigned jobs (start time, duration, location)
- Target date/time for new job
- Estimated job duration

**Processing:**
1. Retrieve contractor's working hours for target date
2. Retrieve all existing jobs for contractor on target date
3. Calculate travel time between jobs using mapping API
4. Identify gaps between jobs that fit new job duration + travel time
5. Return list of available time slots

**Edge Cases:**
- Contractor not working on target date → no availability
- All slots occupied by existing jobs → no availability
- Job spans multiple days → check availability across days
- Travel time exceeds gap between jobs → slot not available

**Output:**
- List of available time slots (start time, end time, requires travel time buffer)

### 4. Distance & Proximity Calculation

**Integration:**
Integrate with OpenRouteService API for travel calculations.

**Operations:**
- Calculate driving distance between two locations (miles/km)
- Calculate estimated travel time accounting for traffic patterns
- Batch calculation for multiple contractor-to-job distances

**Caching Strategy:**
- Cache distance calculations for 24 hours (locations don't move)
- Use location coordinates as cache keys
- Invalidate cache if API returns error

**Error Handling:**
- API unavailable → fall back to straight-line distance estimation
- Invalid coordinates → log error, exclude contractor from ranking
- Rate limiting → implement exponential backoff and queuing

### 5. Scoring & Ranking Engine

**Weighted Scoring Formula:**

```
score = (availabilityWeight × availabilityScore) +
        (ratingWeight × ratingScore) +
        (distanceWeight × distanceScore)
```

**Default Weights:**
- availabilityWeight: 0.4 (40%)
- ratingWeight: 0.3 (30%)
- distanceWeight: 0.3 (30%)

**Score Components:**

**Availability Score (0.0-1.0):**
- 1.0: Has availability within ±30 minutes of desired time
- 0.7: Has availability within ±2 hours of desired time
- 0.4: Has availability same day but >2 hours difference
- 0.0: No availability on desired date

**Rating Score (0.0-1.0):**
- Normalize contractor rating: `ratingScore = rating / 5.0`
- Example: 4.5 star rating → 0.9 score

**Distance Score (0.0-1.0):**
- Use inverse distance with threshold:
- 0-10 miles: 1.0
- 10-30 miles: Linear decay from 1.0 to 0.3
- 30-50 miles: 0.2
- >50 miles: 0.0 (effectively exclude)

**Ranking Process:**
1. Filter contractors by job type match
2. Check availability for each contractor
3. Calculate distance to job site for available contractors
4. Compute weighted score for each candidate
5. Sort descending by score
6. Return top N contractors (default N=5)

**Output Format:**
```json
{
  "jobType": "tile installer",
  "desiredDateTime": "2025-01-15T09:00:00Z",
  "location": {"lat": 40.7128, "lon": -74.0060},
  "recommendations": [
    {
      "contractorId": "CTR-001",
      "name": "John Smith",
      "rating": 4.8,
      "distance": 8.5,
      "availableSlots": [
        {"start": "2025-01-15T09:00:00Z", "end": "2025-01-15T13:00:00Z"},
        {"start": "2025-01-15T14:00:00Z", "end": "2025-01-15T17:00:00Z"}
      ],
      "score": 0.92,
      "scoreBreakdown": {
        "availability": 1.0,
        "rating": 0.96,
        "distance": 0.85
      }
    }
  ]
}
```

### 6. Event-Driven Updates

**Message Bus Implementation:**
Use in-memory message bus (initially) with capability to migrate to distributed message broker (AWS SQS/SNS).

**Events to Publish:**
- `JobAssigned`: When contractor is assigned to job
- `JobUnassigned`: When job assignment is removed
- `ScheduleUpdated`: When contractor schedule changes
- `ContractorRated`: When contractor rating is updated
- `JobCompleted`: When job status moves to completed

**Event Schema:**
```json
{
  "eventType": "JobAssigned",
  "timestamp": "2025-01-15T10:30:00Z",
  "data": {
    "jobId": "JOB-001",
    "contractorId": "CTR-001",
    "scheduledStartTime": "2025-01-16T09:00:00Z"
  }
}
```

**Event Consumers:**
- SignalR hub (broadcasts to connected dispatcher/contractor clients)
- Audit log (records all events for history)
- Analytics pipeline (aggregates metrics for reporting)

### 7. Contractor Recommendation API

**Primary Endpoint:**
`POST /api/recommendations/contractors`

**Request:**
```json
{
  "jobType": "tile installer",
  "desiredDate": "2025-01-16",
  "desiredTime": "09:00:00",
  "location": {
    "address": "123 Main St, New York, NY 10001"
  },
  "estimatedDuration": 4.0
}
```

**Response:**
Ranked list of contractors (see Scoring & Ranking Output Format above)

**Performance Requirements:**
- p50 latency: <200ms
- p95 latency: <500ms
- p99 latency: <1000ms

**Error Scenarios:**
- No contractors match job type → 404 with message
- No contractors available on date → 200 with empty recommendations array
- Invalid input → 400 with validation errors
- Mapping API unavailable → 503 with retry-after header

### 8. Dispatcher UI

**Core Pages:**

**Job Dashboard:**
- List of unassigned jobs (priority queue)
- List of assigned jobs grouped by date
- Ability to click job to request recommendations

**Contractor Recommendation View:**
- Triggered when dispatcher selects a job
- Shows top 5 ranked contractors
- For each contractor displays:
  - Name, rating (star display)
  - Distance from job site
  - Available time slots (clickable)
  - Overall score and breakdown visualization
- "Assign" button for each contractor/slot combination

**Assignment Confirmation:**
- Modal confirming assignment details
- Sends assignment command to backend
- Real-time update via SignalR when assignment completes

**Contractor List View:**
- Browse/search all contractors
- Filter by type, rating, availability status
- Create/edit/deactivate contractors

## Technical Requirements

### Technology Stack

**Backend:**
- **Language/Framework:** C# with .NET 8
- **API Style:** RESTful API with ASP.NET Core Web API
- **Architecture:** Domain-Driven Design (DDD) with CQRS pattern
- **Real-Time:** SignalR for push notifications
- **Database:** PostgreSQL 15+
- **ORM:** Entity Framework Core 8
- **Mapping API:** OpenRouteService REST API
- **Message Bus:** In-memory initially (MediatR), designed for future migration to AWS SQS

**Frontend:**
- **Language/Framework:** TypeScript with React 18
- **State Management:** React Query for server state, Context API for UI state
- **Routing:** React Router v6
- **UI Components:** Material-UI (MUI) or Ant Design
- **Real-Time:** SignalR JavaScript client
- **Build Tool:** Vite
- **HTTP Client:** Axios with interceptors

**Infrastructure:**
- **Cloud Provider:** AWS
- **Container Runtime:** Docker
- **Database Hosting:** AWS RDS for PostgreSQL
- **API Hosting:** AWS ECS Fargate or App Runner
- **Frontend Hosting:** AWS S3 + CloudFront or Amplify
- **Secrets Management:** AWS Secrets Manager
- **CI/CD:** GitHub Actions

**Development Tools:**
- **Version Control:** Git with GitHub
- **C# Testing:** xUnit with FluentAssertions
- **TypeScript Testing:** Vitest with React Testing Library
- **API Testing:** REST Client or Postman
- **Database Migrations:** Entity Framework Core Migrations

### Architecture Principles (Mandatory)

**Domain-Driven Design (DDD):**
- **Domain Layer:** Core entities (Contractor, Job, Schedule, Assignment) as rich domain objects with behavior
- **Application Layer:** Use cases implemented as CQRS commands and queries
- **Infrastructure Layer:** Database access, external APIs, message bus implementations
- **Clear boundaries:** No infrastructure concerns in domain layer

**CQRS (Command Query Responsibility Segregation):**
- **Commands:** AssignJobCommand, CreateContractorCommand, UpdateScheduleCommand
- **Queries:** GetRankedContractorsQuery, GetContractorAvailabilityQuery
- **Handlers:** Separate handlers for each command/query using MediatR pattern
- **Validation:** FluentValidation for input validation on commands/queries

**Vertical Slice Architecture (VSA) Influence:**
- Feature folders where appropriate (e.g., `/Features/ContractorRecommendation`)
- Each feature encapsulates its commands, queries, handlers, and DTOs
- Shared domain models in `/Domain` folder

**Layer Separation:**
```
src/
├── Domain/              # Core business entities and interfaces
│   ├── Entities/
│   ├── ValueObjects/
│   └── Interfaces/
├── Application/         # Use cases (commands, queries, handlers)
│   ├── Commands/
│   ├── Queries/
│   └── DTOs/
├── Infrastructure/      # External concerns (DB, APIs, messaging)
│   ├── Persistence/
│   ├── ExternalServices/
│   └── Messaging/
└── WebApi/             # HTTP API endpoints and SignalR hubs
    ├── Controllers/
    ├── Hubs/
    └── Middleware/
```

### Coding Standards

All code must adhere to standards defined in `.claude/rules/coding-standards.md`:

- **Maximum function length:** 75 lines
- **Maximum file length:** 750 lines
- **Decomposition:** Proactively refactor when approaching limits
- **Naming:** Clear, descriptive names that reveal intent
- **Single Responsibility:** Each function and class has one clear purpose

### Integration Points

**OpenRouteService API:**
- **Endpoint:** `https://api.openrouteservice.org/v2/directions/driving-car`
- **Authentication:** API key in header
- **Rate Limits:** 40 requests/minute on free tier
- **Response Format:** GeoJSON with distance (meters) and duration (seconds)
- **Error Handling:** Retry with exponential backoff, fallback to straight-line distance

**SignalR Real-Time Communication:**
- **Hub:** `/hubs/scheduling` WebSocket endpoint
- **Client Methods:** `ReceiveJobAssigned`, `ReceiveScheduleUpdated`
- **Server Methods:** Broadcasted automatically when domain events occur
- **Connection Management:** Automatic reconnection on disconnect

### Performance Requirements

**API Response Times:**
- Contractor CRUD operations: <100ms p95
- Recommendation endpoint: <500ms p95
- Availability check: <200ms p95

**Scalability:**
- Support 100 concurrent dispatchers
- Handle 1000 contractors in database
- Process 10,000 jobs per month

**Database Performance:**
- Proper indexes on foreign keys and query filters
- Connection pooling configured for concurrent requests
- Query optimization to avoid N+1 problems

### Security Considerations

**Input Validation:**
- Validate all API inputs against expected schemas
- Sanitize inputs to prevent injection attacks
- Rate limiting on public endpoints (future consideration)

**Data Protection:**
- Store sensitive data (contact info) with encryption at rest
- Use HTTPS for all API communication
- Secure WebSocket connections (wss://)
- No sensitive data in logs or error messages

**Authentication & Authorization (Future):**
- Current scope: No authentication required (internal tool)
- Design with authentication hooks for future implementation
- Plan for role-based access (dispatcher, admin, contractor)

**API Security:**
- OpenRouteService API key stored in AWS Secrets Manager
- Never commit secrets to repository
- Use environment variables for configuration

### Data Persistence Requirements

**Entities to Persist:**
- Contractors (with schedules)
- Jobs (with assignments)
- Domain events (audit log)

**Relationship Model:**
- Contractor 1:N Jobs (one contractor can have many jobs)
- Job N:1 Contractor (each job assigned to at most one contractor)

**Database Migrations:**
- Use EF Core migrations for schema changes
- Migrations tracked in version control
- Applied automatically on application startup (dev) or via CI/CD (prod)

**Data Retention:**
- Keep completed jobs for 1 year for analytics
- Soft-delete contractors to maintain referential integrity
- Archive old events beyond 90 days (future consideration)

## Non-Functional Requirements

### Scalability
- **Current:** Single-region deployment supporting 100 concurrent users
- **Future:** Design allows horizontal scaling with load balancer and read replicas

### Reliability
- **Uptime Target:** 99% availability during business hours (8am-6pm ET)
- **Error Handling:** Graceful degradation when mapping API unavailable
- **Data Integrity:** Database transactions ensure consistency of assignments

### Browser Compatibility
- **Primary:** Chrome 110+, Edge 110+
- **Secondary:** Firefox 115+, Safari 16+
- **Mobile:** Responsive design, functional on tablets (iPad)

### Accessibility
- **WCAG 2.1 Level A compliance** (minimum)
- Keyboard navigation support
- Screen reader friendly labels and ARIA attributes
- Sufficient color contrast ratios

### Logging & Monitoring
- **Application Logs:** Structured logging with correlation IDs
- **Performance Metrics:** Request duration, database query times
- **Error Tracking:** Centralized error logging with stack traces
- **Business Metrics:** Jobs assigned per day, average contractor utilization

## Acceptance Criteria

**The project is complete when:**

1. **Contractor Management:**
   - [ ] Dispatcher can create, view, edit, and deactivate contractors via UI
   - [ ] Contractor schedule can be defined with recurring weekly hours
   - [ ] Validation prevents invalid contractor data from being saved

2. **Job Management:**
   - [ ] Dispatcher can create jobs with type, date, time, location, duration
   - [ ] Jobs display in dashboard grouped by status

3. **Recommendation Engine:**
   - [ ] Dispatcher can request recommendations for an unassigned job
   - [ ] API returns top 5 ranked contractors with scores and available slots
   - [ ] Ranking considers availability, rating, and distance with correct weighting
   - [ ] Response time is under 500ms for typical scenarios

4. **Job Assignment:**
   - [ ] Dispatcher can assign a contractor to a job from recommendation list
   - [ ] Assigned jobs update in real-time via SignalR without page refresh
   - [ ] Assigned contractor's availability updates to exclude assigned time

5. **Distance Calculation:**
   - [ ] Integration with OpenRouteService successfully calculates driving distance
   - [ ] Caching reduces redundant API calls for same location pairs
   - [ ] Graceful fallback when API is unavailable

6. **Events & Real-Time:**
   - [ ] JobAssigned events published to message bus
   - [ ] SignalR broadcasts events to connected dispatcher clients
   - [ ] Multiple dispatchers see updates in real-time

7. **Code Quality:**
   - [ ] Follows DDD/CQRS architecture with clear layer separation
   - [ ] Adheres to coding standards (75-line functions, 750-line files)
   - [ ] No violations of OWASP top 10 security issues
   - [ ] All code passes linting and formatting checks

8. **Testing:**
   - [ ] Unit tests cover domain logic with >80% coverage
   - [ ] Integration tests validate end-to-end recommendation flow
   - [ ] Tests for scoring algorithm verify correct calculation
   - [ ] Tests for availability engine verify time slot detection

9. **Documentation:**
   - [ ] Technical writeup documents DDD model and CQRS structure
   - [ ] Scoring algorithm clearly explained with examples
   - [ ] Architecture diagrams show system components and data flow
   - [ ] README provides setup instructions and environment configuration

10. **Demo:**
    - [ ] Video or live demo shows dispatcher creating job
    - [ ] Demo shows system recommending contractors with scores
    - [ ] Demo shows successful assignment with real-time update

## Out of Scope

**Explicitly excluded from this phase:**

- **Authentication & Authorization:** No login system, assume trusted internal users
- **Contractor Mobile App:** No mobile app for contractors to view assignments
- **Customer Portal:** No customer-facing interface to request jobs
- **Payment Processing:** No invoicing or payment handling
- **Advanced Scheduling:** No recurring job scheduling or multi-day job handling
- **Historical Analytics Dashboard:** No reporting UI for utilization metrics
- **Email/SMS Notifications:** No automated notifications to contractors
- **AI/LLM Ranking Explanations:** Planned as late-stage enhancement, not MVP
- **Multi-Tenancy:** Single organization only
- **Mobile-First UI:** Responsive, but optimized for desktop dispatchers

## AI/LLM Integration (Post-MVP Enhancement)

**Note:** AI integration is NOT part of MVP but is planned for late-project enhancement.

**Planned Capability:**
- Use OpenAI API to generate natural language explanations for why a contractor was ranked #1
- Example: "John Smith was recommended because he has a 4.8-star rating, is only 8 miles from the job site, and has availability within 30 minutes of your desired time."

**Implementation Approach:**
- Add optional `explanation` field to recommendation response
- Create separate endpoint or query parameter to request explanations
- Use GPT-3.5 or GPT-4 with structured prompt containing score breakdown
- Cache explanations to avoid redundant API calls

**Success Criteria for AI Enhancement:**
- Explanations are accurate and reflect actual scoring factors
- Response time remains under 500ms when explanations requested
- Fallback to no explanation if OpenAI API unavailable
- Documentation shows example prompts and LLM configuration

This feature demonstrates advanced AI usage but is not required for core system functionality.

## Project Timeline & Milestones

**Estimated Completion:** 1 week (40 hours of development)

**Milestone 1 (Day 1-2):** Foundation & Infrastructure
- Project setup, database schema, domain entities
- Basic CRUD for contractors and jobs

**Milestone 2 (Day 3-4):** Core Scheduling Engine
- Availability engine implementation
- Distance calculation with OpenRouteService
- Scoring and ranking algorithm

**Milestone 3 (Day 5-6):** API & UI
- Recommendation API endpoint
- Dispatcher React UI
- SignalR real-time updates

**Milestone 4 (Day 7):** Testing & Documentation
- Integration tests
- Technical writeup
- Demo preparation

**Post-Week Enhancement:** AI ranking explanations (optional, 4-8 hours additional)
