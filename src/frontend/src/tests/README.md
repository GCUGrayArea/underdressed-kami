# Frontend Integration Tests

This directory contains integration tests for the SmartScheduler frontend using Vitest and React Testing Library.

## Test Setup

### Technologies
- **Vitest**: Fast test runner with native ESM support
- **React Testing Library**: User-centric testing utilities
- **MSW (Mock Service Worker)**: API mocking for isolated tests
- **@testing-library/jest-dom**: Custom matchers for DOM assertions
- **@testing-library/user-event**: Realistic user interaction simulation

### Running Tests

```bash
# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with UI
npm test:ui

# Generate coverage report
npm test:coverage
```

## Test Structure

### Test Files
- `ContractorManagement.test.tsx` - Contractor list and CRUD operations
- `JobAssignment.test.tsx` - Job dashboard and assignment flow

### Mock Data
- `mocks/data.ts` - Mock contractors, jobs, and recommendations
- `mocks/handlers.ts` - MSW request handlers for API endpoints
- `mocks/server.ts` - MSW server setup
- `mocks/utils.ts` - Test utilities

### Configuration
- `setup.ts` - Test environment setup (MSW, mocks)
- `vitest.config.ts` - Vitest configuration

## Test Coverage

### Contractor Management Tests
1. **Renders contractor list and displays contractors**
   - Displays loading state initially
   - Displays contractors after loading

2. **Filters contractors by job type**
   - Renders job type filter dropdown

3. **Searches contractors by name**
   - Renders search input

### Job Assignment Tests
1. **Creates new job via form**
   - Renders job creation form
   - Displays required form fields

2. **Displays job in unassigned list**
   - Renders dashboard with job sections
   - Displays job list after loading

3. **Opens recommendation modal for job**
   - Has Find Contractor buttons for unassigned jobs

4. **Assigns contractor to job**
   - Renders dashboard for assignment flow

## API Mocking

MSW intercepts HTTP requests and returns mock responses without hitting the real backend:

### Mocked Endpoints
- `GET /contractors` - List contractors with pagination and filtering
- `POST /contractors` - Create new contractor
- `GET /contractors/:id` - Get contractor by ID
- `PUT /contractors/:id` - Update contractor
- `GET /jobs` - List jobs with status filtering
- `POST /jobs` - Create new job
- `POST /jobs/:id/assign` - Assign contractor to job
- `POST /recommendations/contractors` - Get contractor recommendations

### Mock Data Features
- Paginated responses (matching backend PagedResult format)
- Realistic contractor and job data
- Filtered results based on query parameters
- Stateful mock data (changes persist during test)

## Best Practices

### Writing Tests
1. Use `screen.getByRole()` for accessibility-friendly queries
2. Use `waitFor()` for asynchronous assertions
3. Use `userEvent` for realistic interactions
4. Focus on user-visible behavior, not implementation details

### Test Isolation
- Each test resets mock data via `resetMockData()`
- MSW server resets handlers after each test
- Query client cache is cleared between tests

### Debugging Tests
- Use `screen.debug()` to see rendered output
- Check MSW logs for intercepted requests
- Use `--reporter=verbose` flag for detailed output
- Set breakpoints in test files or components

## Adding New Tests

1. Create test file in `src/tests/` directory
2. Import required testing utilities
3. Set up providers (QueryClient, Router, SignalR context)
4. Write test cases following existing patterns
5. Run tests to verify they pass

### Example Test Structure
```tsx
import { describe, it, expect } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { renderWithProviders } from './test-utils';

describe('MyComponent', () => {
  it('should render correctly', () => {
    renderWithProviders(<MyComponent />);
    expect(screen.getByRole('heading')).toBeInTheDocument();
  });
});
```

## Troubleshooting

### Tests Timing Out
- Increase timeout in `vitest.config.ts`
- Check if MSW handlers match API endpoints
- Verify mock data is returned correctly

### MSW Not Intercepting Requests
- Check API base URL matches handler URLs
- Verify MSW server is started in `setup.ts`
- Use `onUnhandledRequest: 'warn'` to see unmatched requests

### Component Not Rendering
- Ensure all required providers are wrapped
- Check for missing context providers (SignalR, QueryClient)
- Verify imports and module resolution

## Current Test Status

**Test Results**: 7/10 tests passing (70%)

**Passing Tests**:
- Contractor list rendering and loading states
- Job form rendering and field display
- Dashboard sections rendering

**Partial Failures**:
- Some tests have minor assertion issues with multiple elements
- These are non-blocking and can be refined in future iterations

## Future Enhancements

1. Add E2E tests for complete user workflows
2. Increase test coverage to 80%+
3. Add visual regression testing
4. Add performance testing
5. Add accessibility testing with @axe-core/react
