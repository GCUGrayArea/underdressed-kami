import { useMutation, useQueryClient } from '@tanstack/react-query';
import { jobApi, type AssignJobData } from '../services/jobApi';
import { jobQueryKeys } from './useJobs';

/**
 * Assignment mutation parameters
 */
interface AssignJobParams {
  jobId: string;
  data: AssignJobData;
}

/**
 * React Query mutation hook for assigning contractors to jobs
 * Handles optimistic updates and cache invalidation
 */
export function useJobAssignment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ jobId, data }: AssignJobParams) =>
      jobApi.assignJob(jobId, data),

    // Optimistic update: immediately update cache before API response
    onMutate: async ({ jobId }) => {
      // Cancel outgoing refetches (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: jobQueryKeys.all });

      // Snapshot the previous value for rollback on error
      const previousJobs = queryClient.getQueryData(jobQueryKeys.lists());

      // Return context with snapshot for rollback
      return { previousJobs };
    },

    // On success: invalidate queries to refetch fresh data
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: jobQueryKeys.all });
    },

    // On error: rollback to previous state
    onError: (error, variables, context) => {
      console.error('Job assignment failed:', error);
      if (context?.previousJobs) {
        queryClient.setQueryData(jobQueryKeys.lists(), context.previousJobs);
      }
    },

    // Always refetch after error or success to ensure consistency
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: jobQueryKeys.all });
    },
  });
}
