/**
 * Contractor Form Page
 *
 * Handles both creating new contractors and editing existing ones.
 * Supports validation, working hours editing, and success/error handling.
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Paper,
  Alert,
  CircularProgress,
  Stack,
} from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import CancelIcon from '@mui/icons-material/Cancel';
import {
  ContractorFormFields,
  validateContractorFormData,
  type ContractorFormData,
  type JobTypeOption,
} from '../components/contractors/ContractorFormFields';
import {
  WorkingHoursEditor,
  type WorkingHoursSchedule,
} from '../components/contractors/WorkingHoursEditor';
import { useContractor, useCreateContractor, useUpdateContractor } from '../hooks/useContractors';
import { validateWorkingHours } from '../utils/validation';
import type { WeeklyScheduleDto } from '../types/dto';

/**
 * Job types (hardcoded for now - will be fetched from API in future)
 */
const JOB_TYPES: JobTypeOption[] = [
  { id: '1', name: 'Tile Installer' },
  { id: '2', name: 'Carpet Installer' },
  { id: '3', name: 'Hardwood Specialist' },
];

/**
 * Initialize empty working hours schedule (all days disabled)
 */
function initializeWorkingHours(): WorkingHoursSchedule[] {
  return Array.from({ length: 7 }, (_, i) => ({
    dayOfWeek: i,
    startTime: '09:00',
    endTime: '17:00',
    enabled: false,
  }));
}

/**
 * Convert backend WeeklyScheduleDto[] to WorkingHoursSchedule[]
 */
function convertToWorkingHours(backendSchedules: WeeklyScheduleDto[]): WorkingHoursSchedule[] {
  const schedules = initializeWorkingHours();

  backendSchedules.forEach((backendSchedule) => {
    const schedule = schedules.find((s) => s.dayOfWeek === backendSchedule.dayOfWeek);
    if (schedule) {
      // Convert backend TimeOnly format "HH:mm:ss" to "HH:mm"
      schedule.startTime = backendSchedule.startTime.substring(0, 5);
      schedule.endTime = backendSchedule.endTime.substring(0, 5);
      schedule.enabled = true;
    }
  });

  return schedules;
}

/**
 * Contractor Form Page Component
 */
export function ContractorForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditMode = !!id;

  // Fetch contractor data if editing
  const { data: contractor, isLoading: isLoadingContractor } = useContractor(id || '');

  // Mutations
  const createContractor = useCreateContractor();
  const updateContractor = useUpdateContractor();

  // Form state
  const [formData, setFormData] = useState<ContractorFormData>({
    name: '',
    jobTypeId: '',
    rating: 3.0,
    baseLocation: {
      latitude: 0,
      longitude: 0,
      address: '',
    },
    phone: '',
    email: '',
  });

  const [workingHours, setWorkingHours] = useState<WorkingHoursSchedule[]>(initializeWorkingHours());
  const [errors, setErrors] = useState<Record<string, string | undefined>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  // Populate form when contractor data loads (edit mode)
  useEffect(() => {
    if (contractor && isEditMode) {
      setFormData({
        name: contractor.name,
        jobTypeId: contractor.jobTypeId,
        rating: contractor.rating,
        baseLocation: {
          latitude: contractor.baseLocation.latitude,
          longitude: contractor.baseLocation.longitude,
          address: contractor.baseLocation.address || '',
        },
        phone: contractor.phone || '',
        email: contractor.email || '',
      });

      setWorkingHours(convertToWorkingHours(contractor.weeklySchedule));
    }
  }, [contractor, isEditMode]);

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

  const handleWorkingHoursChange = (newSchedules: WorkingHoursSchedule[]) => {
    setWorkingHours(newSchedules);
    // Clear working hours error
    setErrors((prev) => ({
      ...prev,
      workingHours: undefined,
    }));
  };

  const handleSubmit = async () => {
    // Validate form data
    const fieldErrors = validateContractorFormData(formData);
    const workingHoursError = validateWorkingHours(workingHours);

    if (workingHoursError) {
      fieldErrors.workingHours = workingHoursError;
    }

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
      // Prepare working hours for API
      const weeklySchedule = workingHours
        .filter(wh => wh.isEnabled)
        .map(wh => ({
          dayOfWeek: wh.dayOfWeek,
          startTime: wh.startTime,
          endTime: wh.endTime,
        }));

      // Prepare data for API
      const contractorData = {
        name: formData.name,
        jobTypeId: formData.jobTypeId,
        baseLocation: {
          latitude: formData.baseLocation.latitude,
          longitude: formData.baseLocation.longitude,
          address: formData.baseLocation.address || undefined,
        },
        phone: formData.phone || undefined,
        email: formData.email || undefined,
        weeklySchedule, // Include working hours in both create and update
        ...(isEditMode ? {} : { rating: formData.rating }), // Rating only for create
      };

      if (isEditMode && id) {
        await updateContractor.mutateAsync({ id, data: contractorData });
        setSuccessMessage('Contractor updated successfully');
      } else {
        await createContractor.mutateAsync(contractorData);
        setSuccessMessage('Contractor created successfully');
      }

      // Redirect to contractors list after short delay
      setTimeout(() => {
        navigate('/contractors');
      }, 1500);
    } catch (error) {
      setSubmitError(
        error instanceof Error ? error.message : 'Failed to save contractor'
      );
    }
  };

  const handleCancel = () => {
    navigate('/contractors');
  };

  // Loading state for edit mode
  if (isEditMode && isLoadingContractor) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
        <CircularProgress />
      </Box>
    );
  }

  // Error state for edit mode
  if (isEditMode && !isLoadingContractor && !contractor) {
    return (
      <Box>
        <Alert severity="error">Contractor not found</Alert>
        <Button onClick={handleCancel} sx={{ mt: 2 }}>
          Back to Contractors
        </Button>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {isEditMode ? 'Edit Contractor' : 'Create Contractor'}
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
        <ContractorFormFields
          formData={formData}
          onChange={handleFieldChange}
          errors={errors}
          jobTypes={JOB_TYPES}
        />
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <WorkingHoursEditor
          schedules={workingHours}
          onChange={handleWorkingHoursChange}
          error={errors.workingHours}
        />
      </Paper>

      <Stack direction="row" spacing={2}>
        <Button
          variant="contained"
          startIcon={<SaveIcon />}
          onClick={handleSubmit}
          disabled={createContractor.isPending || updateContractor.isPending}
        >
          {createContractor.isPending || updateContractor.isPending ? 'Saving...' : 'Save'}
        </Button>
        <Button
          variant="outlined"
          startIcon={<CancelIcon />}
          onClick={handleCancel}
          disabled={createContractor.isPending || updateContractor.isPending}
        >
          Cancel
        </Button>
      </Stack>
    </Box>
  );
}
