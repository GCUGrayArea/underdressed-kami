import { QueryClient } from '@tanstack/react-query';

/**
 * React Query client configuration with default options
 *
 * Configuration optimized for real-time updates:
 * - staleTime: 30 seconds - data considered fresh for 30s before refetch
 * - cacheTime: 5 minutes - cached data retained in memory for 5 minutes
 * - retry: 1 attempt - single retry on failure to avoid long delays
 * - refetchOnWindowFocus: true - refresh data when user returns to tab
 */
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // Data is considered fresh for 30 seconds
      staleTime: 30 * 1000,

      // Unused cached data is garbage collected after 5 minutes
      gcTime: 5 * 60 * 1000,

      // Retry failed requests once before showing error
      retry: 1,

      // Refetch on window focus to ensure data freshness
      refetchOnWindowFocus: true,

      // Don't refetch on reconnect (SignalR handles real-time updates)
      refetchOnReconnect: false,

      // Don't refetch on mount if data is still fresh
      refetchOnMount: false,
    },
    mutations: {
      // Retry mutations once on network errors
      retry: 1,
    },
  },
});
