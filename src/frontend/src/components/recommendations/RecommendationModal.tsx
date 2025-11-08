import { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  CircularProgress,
  Alert,
  Stack,
  IconButton,
} from '@mui/material';
import { Close } from '@mui/icons-material';
import type { JobDto, RankedContractorDto } from '../../types/dto';
import { useRecommendations } from '../../hooks/useRecommendations';
import type { GetRecommendationsRequest } from '../../services/recommendationApi';
import { ContractorRecommendationCard } from './ContractorRecommendationCard';
import { AssignmentConfirmation } from './AssignmentConfirmation';

interface RecommendationModalProps {
  open: boolean;
  job: JobDto | null;
  onClose: () => void;
  onAssignmentComplete?: (contractor: RankedContractorDto, job: JobDto) => void;
  isAssigning?: boolean;
}

/**
 * RecommendationModal component
 * Displays ranked contractor recommendations for a selected job
 */
export function RecommendationModal({
  open,
  job,
  onClose,
  onAssignmentComplete,
  isAssigning = false,
}: RecommendationModalProps) {
  // State for assignment confirmation dialog
  const [confirmationOpen, setConfirmationOpen] = useState(false);
  const [selectedContractor, setSelectedContractor] =
    useState<RankedContractorDto | null>(null);

  const recommendationRequest = buildRecommendationRequest(job);

  const { data: contractors, isLoading, error } = useRecommendations(
    recommendationRequest,
    { enabled: open && job !== null }
  );

  // Handle assign button click - show confirmation dialog
  const handleAssignClick = (contractor: RankedContractorDto) => {
    setSelectedContractor(contractor);
    setConfirmationOpen(true);
  };

  // Handle confirmation dialog cancel
  const handleConfirmationCancel = () => {
    setConfirmationOpen(false);
    setSelectedContractor(null);
  };

  // Handle confirmed assignment - delegate to parent
  const handleConfirmationConfirm = () => {
    if (selectedContractor && job && onAssignmentComplete) {
      onAssignmentComplete(selectedContractor, job);
      // Keep confirmation open - parent will close both dialogs on success
    }
  };

  return (
    <>
      <Dialog
        open={open}
        onClose={onClose}
        maxWidth="md"
        fullWidth
        scroll="paper"
      >
        <DialogTitle>
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
          >
            <Box>
              <Typography variant="h6">
                Find Contractor for {job?.formattedId}
              </Typography>
              {job && (
                <Typography variant="body2" color="text.secondary">
                  {job.jobTypeName} - {formatDate(job.desiredDate)}
                  {job.desiredTime && ` at ${formatTime(job.desiredTime)}`}
                </Typography>
              )}
            </Box>
            <IconButton onClick={onClose} size="small">
              <Close />
            </IconButton>
          </Box>
        </DialogTitle>

        <DialogContent dividers>
          {renderContent(contractors, isLoading, error, handleAssignClick)}
        </DialogContent>

        <DialogActions>
          <Button onClick={onClose}>Cancel</Button>
        </DialogActions>
      </Dialog>

      {/* Assignment Confirmation Dialog */}
      <AssignmentConfirmation
        open={confirmationOpen}
        contractor={selectedContractor}
        job={job}
        onConfirm={handleConfirmationConfirm}
        onCancel={handleConfirmationCancel}
        isLoading={isAssigning}
      />
    </>
  );
}

/**
 * Build recommendation request from job data
 */
function buildRecommendationRequest(
  job: JobDto | null
): GetRecommendationsRequest | null {
  if (!job) return null;

  return {
    jobTypeId: job.jobTypeId,
    desiredDate: job.desiredDate,
    desiredTime: job.desiredTime || '09:00:00',
    location: {
      latitude: job.latitude,
      longitude: job.longitude,
    },
    estimatedDurationHours: job.estimatedDurationHours,
    topN: 5,
  };
}

/**
 * Render modal content based on loading/error/data state
 */
function renderContent(
  contractors: RankedContractorDto[] | undefined,
  isLoading: boolean,
  error: Error | null,
  onAssign: (contractor: RankedContractorDto) => void
) {
  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight={300}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error">
        Failed to load recommendations: {error.message}
      </Alert>
    );
  }

  if (!contractors || contractors.length === 0) {
    return (
      <Alert severity="info">
        No contractors available for this job. Try adjusting the job date or
        time.
      </Alert>
    );
  }

  return (
    <Stack spacing={3}>
      <Typography variant="body2" color="text.secondary">
        Showing top {contractors.length} recommended contractors
      </Typography>
      {contractors.map((contractor, index) => (
        <ContractorRecommendationCard
          key={contractor.contractorId}
          contractor={contractor}
          rank={index + 1}
          onAssign={onAssign}
        />
      ))}
    </Stack>
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
