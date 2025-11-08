import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useContext, useEffect } from 'react';
import { jobApi, type JobSearchParams } from '../services/jobApi';
import { SignalRContext } from '../contexts/SignalRContext';
import type { JobDto } from '../types/dto';

/**
 * Query key factory for job queries
 */
export const jobQueryKeys = {
  all: ['jobs'] as const,
  lists: () => [...jobQueryKeys.all, 'list'] as const,
  list: (params?: JobSearchParams) => [...jobQueryKeys.lists(), params] as const,
  details: () => [...jobQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...jobQueryKeys.details(), id] as const,
};

/**
 * Hook options for useJobs
 */
interface UseJobsOptions {
  params?: JobSearchParams;
  refetchInterval?: number; // milliseconds
  enabled?: boolean;
}

/**
 * React Query hook for fetching jobs with auto-refresh and SignalR integration
 */
export function useJobs(options: UseJobsOptions = {}) {
  const { params, refetchInterval = 30000, enabled = true } = options;
  const queryClient = useQueryClient();
  const { connection, connectionStatus } = useContext(SignalRContext);

  const query = useQuery({
    queryKey: jobQueryKeys.list(params),
    queryFn: () => jobApi.getJobs(params),
    refetchInterval,
    enabled,
  });

  // Set up SignalR listeners for real-time updates
  useEffect(() => {
    if (!connection || connectionStatus !== 'connected') {
      return;
    }

    const handleJobAssigned = (data: JobAssignedEvent) => {
      console.log('Job assigned event received:', data);
      invalidateJobQueries(queryClient);
    };

    const handleJobUpdated = (data: JobUpdatedEvent) => {
      console.log('Job updated event received:', data);
      invalidateJobQueries(queryClient);
    };

    const handleJobCreated = (data: JobCreatedEvent) => {
      console.log('Job created event received:', data);
      invalidateJobQueries(queryClient);
    };

    // Register SignalR event handlers
    connection.on('JobAssigned', handleJobAssigned);
    connection.on('JobUpdated', handleJobUpdated);
    connection.on('JobCreated', handleJobCreated);

    // Cleanup on unmount
    return () => {
      connection.off('JobAssigned', handleJobAssigned);
      connection.off('JobUpdated', handleJobUpdated);
      connection.off('JobCreated', handleJobCreated);
    };
  }, [connection, connectionStatus, queryClient]);

  return query;
}

/**
 * Hook for fetching a single job by ID
 */
export function useJob(id: string, enabled = true) {
  return useQuery({
    queryKey: jobQueryKeys.detail(id),
    queryFn: () => jobApi.getJobById(id),
    enabled: enabled && !!id,
  });
}

/**
 * Invalidate all job queries to trigger refetch
 */
function invalidateJobQueries(queryClient: ReturnType<typeof useQueryClient>) {
  queryClient.invalidateQueries({ queryKey: jobQueryKeys.all });
}

/**
 * SignalR event types
 */
interface JobAssignedEvent {
  jobId: string;
  contractorId: string;
  scheduledStartTime: string;
}

interface JobUpdatedEvent {
  jobId: string;
  job: JobDto;
}

interface JobCreatedEvent {
  jobId: string;
  job: JobDto;
}
