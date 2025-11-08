import { Box, Typography, Paper, Divider, Stack, Snackbar, Alert } from '@mui/material';
import { useMemo, useState } from 'react';
import { useJobs } from '../hooks/useJobs';
import { useJobAssignment } from '../hooks/useJobAssignment';
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

  // Notification state
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [snackbarSeverity, setSnackbarSeverity] = useState<'success' | 'error'>(
    'success'
  );

  // Assignment mutation
  const assignmentMutation = useJobAssignment();

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

  // Handle contractor assignment
  const handleAssignContractor = async (
    contractor: RankedContractorDto,
    job: JobDto
  ) => {
    try {
      // Build scheduled start time from job's desired date and contractor's best slot
      const scheduledStartTime = buildScheduledStartTime(
        job.desiredDate,
        contractor.bestAvailableSlot?.start || job.desiredTime || '09:00:00'
      );

      await assignmentMutation.mutateAsync({
        jobId: job.id,
        data: {
          contractorId: contractor.contractorId,
          scheduledStartTime,
        },
      });

      // Success - show notification and close modal
      showSuccessNotification(contractor.name, job.formattedId);
      handleCloseModal();
    } catch (error) {
      // Error - show notification but keep modal open
      showErrorNotification(error);
    }
  };

  // Show success notification
  const showSuccessNotification = (contractorName: string, jobId: string) => {
    setSnackbarMessage(
      `Successfully assigned ${contractorName} to ${jobId}`
    );
    setSnackbarSeverity('success');
    setSnackbarOpen(true);
  };

  // Show error notification
  const showErrorNotification = (error: unknown) => {
    const message =
      error instanceof Error
        ? error.message
        : 'Failed to assign contractor. Please try again.';
    setSnackbarMessage(message);
    setSnackbarSeverity('error');
    setSnackbarOpen(true);
  };

  // Handle snackbar close
  const handleSnackbarClose = () => {
    setSnackbarOpen(false);
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
        onAssignmentComplete={handleAssignContractor}
        isAssigning={assignmentMutation.isPending}
      />

      {/* Success/Error Notification */}
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={3000}
        onClose={handleSnackbarClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert
          onClose={handleSnackbarClose}
          severity={snackbarSeverity}
          variant="filled"
          sx={{ width: '100%' }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
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

/**
 * Build ISO datetime string from date and time components
 */
function buildScheduledStartTime(dateString: string, timeString: string): string {
  // Parse date (YYYY-MM-DD)
  const date = new Date(dateString);

  // Parse time (HH:mm:ss)
  const [hours, minutes, seconds = '00'] = timeString.split(':').map(Number);

  // Combine into ISO datetime
  date.setHours(hours, minutes, seconds, 0);

  return date.toISOString();
}
