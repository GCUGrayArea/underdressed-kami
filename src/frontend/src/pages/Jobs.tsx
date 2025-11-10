import {
  Box,
  Typography,
  Paper,
  Stack,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useJobs } from '../hooks/useJobs';
import { JobList } from '../components/jobs/JobList';
import { JobStatus } from '../types/dto';
import type { JobDto } from '../types/dto';

/**
 * Jobs page - manage jobs
 * View, create, and manage job requests with filtering
 */
export function Jobs() {
  const navigate = useNavigate();
  const [statusFilter, setStatusFilter] = useState<JobStatus | 'all'>('all');

  // Fetch jobs with auto-refresh
  const { data, isLoading } = useJobs({
    params: statusFilter === 'all' ? undefined : { status: statusFilter },
    refetchInterval: 30000, // Auto-refresh every 30 seconds
  });

  const jobs = data?.items || [];

  // Navigate to create job page
  const handleCreateJob = () => {
    navigate('/jobs/new');
  };

  // Handle job card click (navigate to detail page in future)
  const handleJobClick = (job: JobDto) => {
    console.log('Job clicked:', job);
    // TODO: Navigate to job detail page when implemented
  };

  // Get status badge info
  const getStatusStats = () => {
    if (!jobs.length) return null;

    const unassigned = jobs.filter(j => j.status === JobStatus.Unassigned).length;
    const assigned = jobs.filter(j => j.status === JobStatus.Assigned).length;
    const inProgress = jobs.filter(j => j.status === JobStatus.InProgress).length;
    const completed = jobs.filter(j => j.status === JobStatus.Completed).length;

    return { unassigned, assigned, inProgress, completed, total: jobs.length };
  };

  const stats = getStatusStats();

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Jobs</Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<Add />}
          onClick={handleCreateJob}
        >
          Create Job
        </Button>
      </Box>

      {/* Stats Summary */}
      {stats && (
        <Paper sx={{ p: 2, mb: 3 }}>
          <Stack direction="row" spacing={2} alignItems="center" flexWrap="wrap">
            <Typography variant="body2" color="text.secondary">
              Total: {stats.total}
            </Typography>
            {stats.unassigned > 0 && (
              <Chip
                label={`${stats.unassigned} Unassigned`}
                size="small"
                color="warning"
              />
            )}
            {stats.assigned > 0 && (
              <Chip
                label={`${stats.assigned} Assigned`}
                size="small"
                color="info"
              />
            )}
            {stats.inProgress > 0 && (
              <Chip
                label={`${stats.inProgress} In Progress`}
                size="small"
                color="primary"
              />
            )}
            {stats.completed > 0 && (
              <Chip
                label={`${stats.completed} Completed`}
                size="small"
                color="success"
              />
            )}
          </Stack>
        </Paper>
      )}

      {/* Filters */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Stack direction="row" spacing={2} alignItems="center">
          <Typography variant="subtitle2" color="text.secondary">
            Filters:
          </Typography>
          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel>Status</InputLabel>
            <Select
              value={statusFilter}
              label="Status"
              onChange={(e) => setStatusFilter(e.target.value as JobStatus | 'all')}
            >
              <MenuItem value="all">All Statuses</MenuItem>
              <MenuItem value={JobStatus.Unassigned}>Unassigned</MenuItem>
              <MenuItem value={JobStatus.Assigned}>Assigned</MenuItem>
              <MenuItem value={JobStatus.InProgress}>In Progress</MenuItem>
              <MenuItem value={JobStatus.Completed}>Completed</MenuItem>
              <MenuItem value={JobStatus.Cancelled}>Cancelled</MenuItem>
            </Select>
          </FormControl>
        </Stack>
      </Paper>

      {/* Job List */}
      <Paper sx={{ p: 3 }}>
        <JobList
          jobs={jobs}
          loading={isLoading}
          onJobClick={handleJobClick}
          emptyMessage={
            statusFilter === 'all'
              ? 'No jobs found. Create your first job to get started!'
              : `No ${statusFilter.toLowerCase()} jobs found.`
          }
        />
      </Paper>
    </Box>
  );
}
