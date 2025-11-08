import { useQuery } from '@tanstack/react-query';
import {
  recommendationApi,
  type GetRecommendationsRequest,
} from '../services/recommendationApi';

/**
 * Hook for fetching contractor recommendations
 * Uses React Query for caching and state management
 */
export function useRecommendations(
  request: GetRecommendationsRequest | null,
  options?: {
    enabled?: boolean;
  }
) {
  return useQuery({
    queryKey: ['recommendations', request],
    queryFn: () => {
      if (!request) {
        throw new Error('Request is required');
      }
      return recommendationApi.getRecommendations(request);
    },
    enabled: options?.enabled !== false && request !== null,
    // Don't cache recommendations - they depend on real-time availability
    staleTime: 0,
    gcTime: 0,
  });
}
