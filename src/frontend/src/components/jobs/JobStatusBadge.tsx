import { Chip } from '@mui/material';
import { JobStatus } from '../../types/dto';

interface JobStatusBadgeProps {
  status: JobStatus;
}

/**
 * Status badge component that displays job status with color coding
 */
export function JobStatusBadge({ status }: JobStatusBadgeProps) {
  const config = getStatusConfig(status);

  return (
    <Chip
      label={config.label}
      color={config.color}
      size="small"
      sx={{ fontWeight: 'medium' }}
    />
  );
}

/**
 * Get configuration for each status type
 */
function getStatusConfig(status: JobStatus) {
  switch (status) {
    case JobStatus.Unassigned:
      return { label: 'Unassigned', color: 'warning' as const };
    case JobStatus.Assigned:
      return { label: 'Assigned', color: 'info' as const };
    case JobStatus.InProgress:
      return { label: 'In Progress', color: 'primary' as const };
    case JobStatus.Completed:
      return { label: 'Completed', color: 'success' as const };
    case JobStatus.Cancelled:
      return { label: 'Cancelled', color: 'error' as const };
    default:
      return { label: 'Unknown', color: 'default' as const };
  }
}
