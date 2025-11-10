/**
 * Create Job Page
 *
 * Form for creating new job requests with type, desired date/time, location, and duration.
 * Follows the pattern established in ContractorForm.tsx (PR-021).
 */

import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Paper,
  Alert,
  Stack,
} from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import CancelIcon from '@mui/icons-material/Cancel';
import { JobFormFields, type JobFormData, type JobTypeOption } from '../components/jobs/JobFormFields';
import { useCreateJob } from '../hooks/useJobs';
import {
  validateCustomerId,
  validateCustomerName,
  validateDuration,
  validateFutureDate,
  validateLatitude,
  validateLongitude,
  validateAddress,
} from '../utils/validation';

/**
 * Job types (hardcoded for now - matches database seed GUIDs)
 */
const JOB_TYPES: JobTypeOption[] = [
  { id: '10000000-0000-0000-0000-000000000001', name: 'Tile Installer' },
  { id: '10000000-0000-0000-0000-000000000002', name: 'Carpet Installer' },
  { id: '10000000-0000-0000-0000-000000000003', name: 'Hardwood Specialist' },
];

/**
 * Create Job Page Component
 */
export function CreateJob() {
  const navigate = useNavigate();
  const createJob = useCreateJob();

  // Form state
  const [formData, setFormData] = useState<JobFormData>({
    jobTypeId: '',
    customerId: '',
    customerName: '',
    location: {
      latitude: 0,
      longitude: 0,
      address: '',
    },
    desiredDate: null,
    desiredTime: null,
    estimatedDurationHours: 4.0, // Default to 4 hours
  });

  const [errors, setErrors] = useState<Record<string, string | undefined>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const handleFieldChange = (field: string, value: unknown) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));

    // Clear error for this field
    setErrors((prev) => ({
      ...prev,
      [field]: undefined,
    }));
  };

  const validateFormData = (): Record<string, string | undefined> => {
    return {
      jobTypeId: !formData.jobTypeId ? 'Job type is required' : undefined,
      customerId: validateCustomerId(formData.customerId),
      customerName: validateCustomerName(formData.customerName),
      latitude: validateLatitude(formData.location.latitude),
      longitude: validateLongitude(formData.location.longitude),
      address: validateAddress(formData.location.address),
      desiredDate: validateFutureDate(formData.desiredDate),
      estimatedDurationHours: validateDuration(formData.estimatedDurationHours),
    };
  };

  const handleSubmit = async () => {
    // Validate form data
    const fieldErrors = validateFormData();

    // Check if there are any errors
    const hasErrors = Object.values(fieldErrors).some((error) => error !== undefined);
    if (hasErrors) {
      setErrors(fieldErrors);
      setSubmitError('Please fix validation errors before submitting');
      return;
    }

    setErrors({});
    setSubmitError(null);

    try {
      // Prepare data for API
      const jobData = {
        jobTypeId: formData.jobTypeId,
        customerId: formData.customerId,
        customerName: formData.customerName,
        latitude: formData.location.latitude,
        longitude: formData.location.longitude,
        address: formData.location.address || undefined,
        desiredDate: formData.desiredDate!.toISOString().split('T')[0], // Format as YYYY-MM-DD
        desiredTime: formData.desiredTime
          ? formatTimeForApi(formData.desiredTime)
          : undefined,
        estimatedDurationHours: formData.estimatedDurationHours,
      };

      await createJob.mutateAsync(jobData);
      setSuccessMessage('Job created successfully');

      // Redirect to dashboard after short delay
      setTimeout(() => {
        navigate('/');
      }, 1500);
    } catch (error) {
      setSubmitError(
        error instanceof Error ? error.message : 'Failed to create job'
      );
    }
  };

  const handleCancel = () => {
    navigate('/');
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Create Job
      </Typography>

      {submitError && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setSubmitError(null)}>
          {submitError}
        </Alert>
      )}

      {successMessage && (
        <Alert severity="success" sx={{ mb: 3 }}>
          {successMessage}
        </Alert>
      )}

      <Paper sx={{ p: 3, mb: 3 }}>
        <JobFormFields
          formData={formData}
          onChange={handleFieldChange}
          errors={errors}
          jobTypes={JOB_TYPES}
        />
      </Paper>

      <Stack direction="row" spacing={2}>
        <Button
          variant="contained"
          startIcon={<SaveIcon />}
          onClick={handleSubmit}
          disabled={createJob.isPending}
        >
          {createJob.isPending ? 'Creating...' : 'Create Job'}
        </Button>
        <Button
          variant="outlined"
          startIcon={<CancelIcon />}
          onClick={handleCancel}
          disabled={createJob.isPending}
        >
          Cancel
        </Button>
      </Stack>
    </Box>
  );
}

/**
 * Format time for API (HH:mm format from Date object)
 */
function formatTimeForApi(date: Date): string {
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${hours}:${minutes}`;
}
