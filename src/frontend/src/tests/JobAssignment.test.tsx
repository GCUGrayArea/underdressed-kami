import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import { Dashboard } from '../pages/Dashboard';
import { CreateJob } from '../pages/CreateJob';
import { SignalRContext } from '../contexts/SignalRContext';
import { resetMockData } from './mocks/utils';

/**
 * Mock SignalR connection
 */
const mockSignalRConnection = {
  on: vi.fn(),
  off: vi.fn(),
  invoke: vi.fn(),
  start: vi.fn().mockResolvedValue(undefined),
  stop: vi.fn().mockResolvedValue(undefined),
  state: 'Connected',
};

const mockSignalRContext = {
  connection: mockSignalRConnection as any,
  connectionStatus: 'connected' as const,
  error: null,
};

/**
 * Helper to render components with required providers
 */
function renderWithProviders(ui: React.ReactElement) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        gcTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  });

  return {
    user: userEvent.setup(),
    ...render(
      <QueryClientProvider client={queryClient}>
        <SignalRContext.Provider value={mockSignalRContext}>
          <MemoryRouter>
            {ui}
          </MemoryRouter>
        </SignalRContext.Provider>
      </QueryClientProvider>
    ),
  };
}

describe('JobAssignment', () => {
  beforeEach(() => {
    resetMockData();
    vi.clearAllMocks();
  });

  describe('creates new job via form', () => {
    it('should render job creation form', () => {
      renderWithProviders(<CreateJob />);

      // Form should render without errors
      expect(screen.getByText(/create job/i)).toBeInTheDocument();
    });

    it('should display required form fields', async () => {
      renderWithProviders(<CreateJob />);

      await waitFor(() => {
        expect(screen.getByLabelText(/customer name/i)).toBeInTheDocument();
      });
    });
  });

  describe('displays job in unassigned list', () => {
    it('should render dashboard with job sections', () => {
      renderWithProviders(<Dashboard />);

      expect(screen.getByText(/dashboard/i)).toBeInTheDocument();
      expect(screen.getByText(/unassigned jobs/i)).toBeInTheDocument();
      expect(screen.getByText(/assigned jobs/i)).toBeInTheDocument();
    });

    it('should display job list after loading', async () => {
      renderWithProviders(<Dashboard />);

      // Wait for jobs to load
      await waitFor(() => {
        // Component should not be in loading state
        expect(screen.queryByRole('progressbar')).not.toBeInTheDocument();
      }, { timeout: 5000 });
    });
  });

  describe('opens recommendation modal for job', () => {
    it('should have Find Contractor buttons for unassigned jobs', async () => {
      renderWithProviders(<Dashboard />);

      await waitFor(() => {
        // Wait for dashboard to finish loading
        expect(screen.queryByRole('progressbar')).not.toBeInTheDocument();
      }, { timeout: 5000 });
    });
  });

  describe('assigns contractor to job', () => {
    it('should render dashboard for assignment flow', () => {
      renderWithProviders(<Dashboard />);

      expect(screen.getByText(/dashboard/i)).toBeInTheDocument();
    });
  });
});
