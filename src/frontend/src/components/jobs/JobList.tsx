import { Skeleton, Typography, Box } from '@mui/material';
import type { JobDto } from '../../types/dto';
import { JobCard } from './JobCard';

interface JobListProps {
  jobs: JobDto[];
  loading?: boolean;
  onJobClick?: (job: JobDto) => void;
  onFindContractor?: (job: JobDto) => void;
  emptyMessage?: string;
}

/**
 * Job list component that renders a grid of job cards
 */
export function JobList({
  jobs,
  loading = false,
  onJobClick,
  onFindContractor,
  emptyMessage,
}: JobListProps) {
  if (loading) {
    return <LoadingSkeleton />;
  }

  if (jobs.length === 0) {
    return <EmptyState message={emptyMessage} />;
  }

  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: {
          xs: '1fr',
          sm: 'repeat(2, 1fr)',
          md: 'repeat(3, 1fr)',
        },
        gap: 2,
      }}
    >
      {jobs.map((job) => (
        <JobCard
          key={job.id}
          job={job}
          onClick={onJobClick}
          onFindContractor={onFindContractor}
        />
      ))}
    </Box>
  );
}

/**
 * Loading skeleton while jobs are being fetched
 */
function LoadingSkeleton() {
  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: {
          xs: '1fr',
          sm: 'repeat(2, 1fr)',
          md: 'repeat(3, 1fr)',
        },
        gap: 2,
      }}
    >
      {[1, 2, 3, 4, 5, 6].map((i) => (
        <Skeleton key={i} variant="rectangular" height={200} sx={{ borderRadius: 1 }} />
      ))}
    </Box>
  );
}

interface EmptyStateProps {
  message?: string;
}

/**
 * Empty state when no jobs are available
 */
function EmptyState({ message }: EmptyStateProps) {
  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      minHeight={200}
      sx={{ py: 4 }}
    >
      <Typography variant="body1" color="text.secondary">
        {message || 'No jobs found'}
      </Typography>
    </Box>
  );
}
