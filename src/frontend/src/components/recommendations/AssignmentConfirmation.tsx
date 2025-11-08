import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Stack,
  Rating,
  Divider,
  CircularProgress,
} from '@mui/material';
import { AccessTime, Person, LocationOn } from '@mui/icons-material';
import type { RankedContractorDto, JobDto } from '../../types/dto';

interface AssignmentConfirmationProps {
  open: boolean;
  contractor: RankedContractorDto | null;
  job: JobDto | null;
  onConfirm: () => void;
  onCancel: () => void;
  isLoading?: boolean;
}

/**
 * AssignmentConfirmation component
 * Shows confirmation dialog before assigning contractor to job
 */
export function AssignmentConfirmation({
  open,
  contractor,
  job,
  onConfirm,
  onCancel,
  isLoading = false,
}: AssignmentConfirmationProps) {
  if (!contractor || !job) {
    return null;
  }

  return (
    <Dialog open={open} onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>Confirm Assignment</DialogTitle>

      <DialogContent>
        <Stack spacing={3}>
          <Typography variant="body2" color="text.secondary">
            You are about to assign this contractor to the job. Please review
            the details below.
          </Typography>

          <Divider />

          {/* Job Details */}
          <Box>
            <Typography variant="subtitle2" gutterBottom color="text.secondary">
              Job Details
            </Typography>
            <Stack spacing={1}>
              <Typography variant="body1" fontWeight="medium">
                {job.formattedId} - {job.jobTypeName}
              </Typography>
              <Box display="flex" alignItems="center" gap={1}>
                <LocationOn fontSize="small" color="action" />
                <Typography variant="body2">
                  {job.customerName} - {formatDate(job.desiredDate)}
                  {job.desiredTime && ` at ${formatTime(job.desiredTime)}`}
                </Typography>
              </Box>
            </Stack>
          </Box>

          <Divider />

          {/* Contractor Details */}
          <Box>
            <Typography variant="subtitle2" gutterBottom color="text.secondary">
              Contractor Details
            </Typography>
            <Stack spacing={1.5}>
              <Box>
                <Box display="flex" alignItems="center" gap={1} mb={0.5}>
                  <Person fontSize="small" color="action" />
                  <Typography variant="body1" fontWeight="medium">
                    {contractor.name}
                  </Typography>
                </Box>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  fontFamily="monospace"
                  ml={3.5}
                >
                  {contractor.formattedId}
                </Typography>
              </Box>

              <Box display="flex" alignItems="center" gap={1} ml={3.5}>
                <Rating
                  value={contractor.rating}
                  precision={0.1}
                  readOnly
                  size="small"
                />
                <Typography variant="body2" color="text.secondary">
                  {contractor.rating.toFixed(1)} stars
                </Typography>
              </Box>

              <Box ml={3.5}>
                <Typography variant="body2" color="text.secondary">
                  {contractor.distanceMiles.toFixed(1)} miles away
                </Typography>
              </Box>
            </Stack>
          </Box>

          {/* Time Slot */}
          {contractor.bestAvailableSlot && (
            <>
              <Divider />
              <Box>
                <Typography
                  variant="subtitle2"
                  gutterBottom
                  color="text.secondary"
                >
                  Scheduled Time
                </Typography>
                <Box display="flex" alignItems="center" gap={1}>
                  <AccessTime fontSize="small" color="action" />
                  <Typography variant="body1">
                    {formatTimeSlot(contractor.bestAvailableSlot)}
                  </Typography>
                </Box>
              </Box>
            </>
          )}
        </Stack>
      </DialogContent>

      <DialogActions>
        <Button onClick={onCancel} disabled={isLoading}>
          Cancel
        </Button>
        <Button
          onClick={onConfirm}
          variant="contained"
          disabled={isLoading}
          startIcon={isLoading ? <CircularProgress size={16} /> : null}
        >
          {isLoading ? 'Assigning...' : 'Confirm Assignment'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}

/**
 * Format date for display
 */
function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    weekday: 'short',
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

/**
 * Format time for display (HH:mm:ss to 12-hour format)
 */
function formatTime(timeString: string): string {
  const [hours, minutes] = timeString.split(':').map(Number);
  const period = hours >= 12 ? 'PM' : 'AM';
  const displayHours = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours;
  return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
}

/**
 * Format time slot for display
 */
function formatTimeSlot(slot: { start: string; end: string }): string {
  return `${formatTime(slot.start)} - ${formatTime(slot.end)}`;
}
