import { Box, Typography, Paper, Divider, Stack } from '@mui/material';
import { useMemo, useState } from 'react';
import { useJobs } from '../hooks/useJobs';
import { JobList } from '../components/jobs/JobList';
import { RecommendationModal } from '../components/recommendations/RecommendationModal';
import type { JobDto, RankedContractorDto } from '../types/dto';
import { JobStatus } from '../types/dto';

/**
 * Dashboard page - main dispatcher view
 * Shows unassigned jobs and assigned jobs grouped by date
 */
export function Dashboard() {
  const { data, isLoading } = useJobs({
    refetchInterval: 30000, // Auto-refresh every 30 seconds
  });

  const jobs = data?.items || [];

  // Modal state
  const [selectedJob, setSelectedJob] = useState<JobDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  // Separate and sort jobs by status
  const { unassignedJobs, assignedJobsByDate } = useMemo(() => {
    return categorizeJobs(jobs);
  }, [jobs]);

  // Handle Find Contractor button click
  const handleFindContractor = (job: JobDto) => {
    setSelectedJob(job);
    setModalOpen(true);
  };

  // Handle modal close
  const handleCloseModal = () => {
    setModalOpen(false);
    setSelectedJob(null);
  };

  // Handle contractor assignment (placeholder for PR-025)
  const handleAssignContractor = (
    contractor: RankedContractorDto,
    job: JobDto
  ) => {
    // TODO: Implement in PR-025 (Job Assignment Flow)
    console.log('Assign contractor', contractor, 'to job', job);
    handleCloseModal();
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>

      <Stack spacing={4} sx={{ mt: 3 }}>
        {/* Unassigned Jobs - Priority Queue */}
        <Paper sx={{ p: 3 }}>
          <Typography variant="h5" gutterBottom>
            Unassigned Jobs
          </Typography>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            Sorted by desired date (earliest first)
          </Typography>
          <Divider sx={{ my: 2 }} />
          <JobList
            jobs={unassignedJobs}
            loading={isLoading}
            emptyMessage="No unassigned jobs"
            onFindContractor={handleFindContractor}
          />
        </Paper>

        {/* Assigned Jobs - Grouped by Date */}
        <Paper sx={{ p: 3 }}>
          <Typography variant="h5" gutterBottom>
            Assigned Jobs
          </Typography>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            Grouped by scheduled date
          </Typography>
          <Divider sx={{ my: 2 }} />
          {renderAssignedJobsByDate(assignedJobsByDate, isLoading)}
        </Paper>
      </Stack>

      {/* Recommendation Modal */}
      <RecommendationModal
        open={modalOpen}
        job={selectedJob}
        onClose={handleCloseModal}
        onAssign={handleAssignContractor}
      />
    </Box>
  );
}

/**
 * Categorize jobs into unassigned and assigned groups
 */
function categorizeJobs(jobs: JobDto[]) {
  const unassigned = jobs.filter((job) => job.status === JobStatus.Unassigned);
  const assigned = jobs.filter(
    (job) => job.status !== JobStatus.Unassigned && job.status !== JobStatus.Cancelled
  );

  // Sort unassigned by desired date (earliest first)
  const unassignedJobs = sortJobsByDesiredDate(unassigned);

  // Group assigned jobs by scheduled date
  const assignedJobsByDate = groupJobsByScheduledDate(assigned);

  return { unassignedJobs, assignedJobsByDate };
}

/**
 * Sort jobs by desired date ascending
 */
function sortJobsByDesiredDate(jobs: JobDto[]): JobDto[] {
  return [...jobs].sort((a, b) => {
    const dateA = new Date(a.desiredDate).getTime();
    const dateB = new Date(b.desiredDate).getTime();
    return dateA - dateB;
  });
}

/**
 * Group jobs by scheduled date
 */
function groupJobsByScheduledDate(jobs: JobDto[]): Map<string, JobDto[]> {
  const grouped = new Map<string, JobDto[]>();

  for (const job of jobs) {
    const dateKey = job.scheduledStartTime
      ? new Date(job.scheduledStartTime).toLocaleDateString()
      : 'No Date';

    if (!grouped.has(dateKey)) {
      grouped.set(dateKey, []);
    }
    grouped.get(dateKey)!.push(job);
  }

  return grouped;
}

/**
 * Render assigned jobs grouped by date
 */
function renderAssignedJobsByDate(
  jobsByDate: Map<string, JobDto[]>,
  isLoading: boolean
) {
  if (isLoading) {
    return <JobList jobs={[]} loading />;
  }

  if (jobsByDate.size === 0) {
    return <JobList jobs={[]} emptyMessage="No assigned jobs" />;
  }

  const sortedDates = Array.from(jobsByDate.keys()).sort((a, b) => {
    if (a === 'No Date') return 1;
    if (b === 'No Date') return -1;
    return new Date(a).getTime() - new Date(b).getTime();
  });

  return (
    <Stack spacing={3}>
      {sortedDates.map((date) => (
        <Box key={date}>
          <Typography variant="h6" gutterBottom sx={{ mb: 2 }}>
            {date}
          </Typography>
          <JobList jobs={jobsByDate.get(date) || []} />
        </Box>
      ))}
    </Stack>
  );
}
