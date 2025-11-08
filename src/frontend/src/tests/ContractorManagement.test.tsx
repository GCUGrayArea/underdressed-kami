import { describe, it, expect, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import { Contractors } from '../pages/Contractors';
import { resetMockData } from './mocks/utils';

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
    },
  });

  return {
    user: userEvent.setup(),
    ...render(
      <QueryClientProvider client={queryClient}>
        <MemoryRouter>
          {ui}
        </MemoryRouter>
      </QueryClientProvider>
    ),
  };
}

describe('ContractorManagement', () => {
  beforeEach(() => {
    resetMockData();
  });

  describe('renders contractor list and displays contractors', () => {
    it('should display loading state initially', () => {
      renderWithProviders(<Contractors />);

      // Component should render without errors
      expect(screen.getByText(/contractors/i)).toBeInTheDocument();
    });

    it('should display contractors after loading', async () => {
      renderWithProviders(<Contractors />);

      // Wait for the contractor list to load
      await waitFor(() => {
        // Look for any contractor data or the table structure
        const table = document.querySelector('table');
        expect(table).toBeInTheDocument();
      }, { timeout: 5000 });
    });
  });

  describe('filters contractors by job type', () => {
    it('should render job type filter dropdown', async () => {
      renderWithProviders(<Contractors />);

      await waitFor(() => {
        expect(screen.getByLabelText(/job type/i)).toBeInTheDocument();
      });
    });
  });

  describe('searches contractors by name', () => {
    it('should render search input', async () => {
      renderWithProviders(<Contractors />);

      await waitFor(() => {
        expect(screen.getByPlaceholderText(/search/i)).toBeInTheDocument();
      });
    });
  });
});
