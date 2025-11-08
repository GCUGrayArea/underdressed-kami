/**
 * Job Form Fields Component
 *
 * Reusable form fields for job create/edit forms.
 * Includes fields for job type, customer info, location, desired date/time, and duration.
 */

import { Grid, TextField, MenuItem, Typography, Box } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { TimePicker } from '@mui/x-date-pickers/TimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

/**
 * Job type option for dropdown
 */
export interface JobTypeOption {
  id: string;
  name: string;
}

/**
 * Form data interface matching CreateJobData
 */
export interface JobFormData {
  jobTypeId: string;
  customerId: string;
  customerName: string;
  location: {
    latitude: number;
    longitude: number;
    address: string;
  };
  desiredDate: Date | null;
  desiredTime: Date | null;
  estimatedDurationHours: number;
}

interface JobFormFieldsProps {
  formData: JobFormData;
  onChange: (field: string, value: unknown) => void;
  errors: Record<string, string | undefined>;
  jobTypes: JobTypeOption[];
}

/**
 * Job Form Fields Component
 */
export function JobFormFields({ formData, onChange, errors, jobTypes }: JobFormFieldsProps) {
  const handleChange = (field: string, value: unknown) => {
    onChange(field, value);
  };

  const handleLocationChange = (locationField: string, value: unknown) => {
    onChange('location', {
      ...formData.location,
      [locationField]: value,
    });
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Box>
        <Typography variant="h6" gutterBottom>
          Job Information
        </Typography>

        <Grid container spacing={3}>
          {/* Job Type */}
          <Grid item xs={12} sm={6}>
            <TextField
              select
              label="Job Type"
              value={formData.jobTypeId}
              onChange={(e) => handleChange('jobTypeId', e.target.value)}
              error={!!errors.jobTypeId}
              helperText={errors.jobTypeId || 'Type of flooring work required'}
              required
              fullWidth
            >
              {jobTypes.map((jobType) => (
                <MenuItem key={jobType.id} value={jobType.id}>
                  {jobType.name}
                </MenuItem>
              ))}
            </TextField>
          </Grid>

          {/* Estimated Duration */}
          <Grid item xs={12} sm={6}>
            <TextField
              label="Estimated Duration (hours)"
              type="number"
              value={formData.estimatedDurationHours}
              onChange={(e) => handleChange('estimatedDurationHours', parseFloat(e.target.value))}
              error={!!errors.estimatedDurationHours}
              helperText={errors.estimatedDurationHours || 'Expected job duration (0.1-24 hours)'}
              required
              inputProps={{
                min: 0.1,
                max: 24,
                step: 0.5,
              }}
              fullWidth
            />
          </Grid>

          {/* Customer Section */}
          <Grid item xs={12}>
            <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
              Customer Information
            </Typography>
          </Grid>

          {/* Customer ID */}
          <Grid item xs={12} sm={6}>
            <TextField
              label="Customer ID"
              value={formData.customerId}
              onChange={(e) => handleChange('customerId', e.target.value)}
              error={!!errors.customerId}
              helperText={errors.customerId || 'Customer reference ID (1-100 characters)'}
              required
              fullWidth
            />
          </Grid>

          {/* Customer Name */}
          <Grid item xs={12} sm={6}>
            <TextField
              label="Customer Name"
              value={formData.customerName}
              onChange={(e) => handleChange('customerName', e.target.value)}
              error={!!errors.customerName}
              helperText={errors.customerName || 'Customer full name (1-200 characters)'}
              required
              fullWidth
            />
          </Grid>

          {/* Date/Time Section */}
          <Grid item xs={12}>
            <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
              Desired Schedule
            </Typography>
          </Grid>

          {/* Desired Date */}
          <Grid item xs={12} sm={6}>
            <DatePicker
              label="Desired Date"
              value={formData.desiredDate}
              onChange={(date) => handleChange('desiredDate', date)}
              slotProps={{
                textField: {
                  error: !!errors.desiredDate,
                  helperText: errors.desiredDate || 'Date when service is needed',
                  required: true,
                  fullWidth: true,
                },
              }}
              minDate={new Date()} // Only allow future dates
            />
          </Grid>

          {/* Desired Time */}
          <Grid item xs={12} sm={6}>
            <TimePicker
              label="Desired Time (optional)"
              value={formData.desiredTime}
              onChange={(time) => handleChange('desiredTime', time)}
              slotProps={{
                textField: {
                  error: !!errors.desiredTime,
                  helperText: errors.desiredTime || 'Preferred start time (optional)',
                  fullWidth: true,
                },
              }}
            />
          </Grid>

          {/* Location Section */}
          <Grid item xs={12}>
            <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
              Job Location
            </Typography>
          </Grid>

          {/* Address */}
          <Grid item xs={12}>
            <TextField
              label="Address"
              value={formData.location.address}
              onChange={(e) => handleLocationChange('address', e.target.value)}
              error={!!errors.address}
              helperText={errors.address || 'Street address, city, state, zip (optional but recommended)'}
              fullWidth
            />
          </Grid>

          {/* Latitude */}
          <Grid item xs={12} sm={6}>
            <TextField
              label="Latitude"
              type="number"
              value={formData.location.latitude}
              onChange={(e) => handleLocationChange('latitude', parseFloat(e.target.value))}
              error={!!errors.latitude}
              helperText={errors.latitude || 'Latitude (-90 to 90)'}
              required
              inputProps={{
                min: -90,
                max: 90,
                step: 0.000001,
              }}
              fullWidth
            />
          </Grid>

          {/* Longitude */}
          <Grid item xs={12} sm={6}>
            <TextField
              label="Longitude"
              type="number"
              value={formData.location.longitude}
              onChange={(e) => handleLocationChange('longitude', parseFloat(e.target.value))}
              error={!!errors.longitude}
              helperText={errors.longitude || 'Longitude (-180 to 180)'}
              required
              inputProps={{
                min: -180,
                max: 180,
                step: 0.000001,
              }}
              fullWidth
            />
          </Grid>
        </Grid>
      </Box>
    </LocalizationProvider>
  );
}
