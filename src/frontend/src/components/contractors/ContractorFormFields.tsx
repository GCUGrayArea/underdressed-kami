/**
 * Contractor Form Fields Component
 *
 * Reusable form fields for contractor create/edit forms.
 * Includes fields for name, type, rating, location, and contact info.
 */

import { Grid, TextField, MenuItem, Typography, Box } from '@mui/material';
import { validateContractorName, validateRating, validateLatitude, validateLongitude, validateEmail, validatePhone } from '../../utils/validation';

/**
 * Job type option for dropdown
 */
export interface JobTypeOption {
  id: string;
  name: string;
}

/**
 * Form data interface matching CreateContractorData/UpdateContractorData
 */
export interface ContractorFormData {
  name: string;
  jobTypeId: string;
  rating: number;
  baseLocation: {
    latitude: number;
    longitude: number;
    address: string;
  };
  phone: string;
  email: string;
}

interface ContractorFormFieldsProps {
  formData: ContractorFormData;
  onChange: (field: string, value: unknown) => void;
  errors: Record<string, string | undefined>;
  jobTypes: JobTypeOption[];
}

/**
 * Contractor Form Fields Component
 */
export function ContractorFormFields({ formData, onChange, errors, jobTypes }: ContractorFormFieldsProps) {
  const handleChange = (field: string, value: unknown) => {
    onChange(field, value);
  };

  const handleLocationChange = (locationField: string, value: unknown) => {
    onChange('baseLocation', {
      ...formData.baseLocation,
      [locationField]: value,
    });
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Contractor Information
      </Typography>

      <Grid container spacing={3}>
        {/* Name */}
        <Grid item xs={12} sm={6}>
          <TextField
            label="Name"
            value={formData.name}
            onChange={(e) => handleChange('name', e.target.value)}
            error={!!errors.name}
            helperText={errors.name || 'Full legal name (2-100 characters)'}
            required
            fullWidth
          />
        </Grid>

        {/* Job Type */}
        <Grid item xs={12} sm={6}>
          <TextField
            select
            label="Job Type"
            value={formData.jobTypeId}
            onChange={(e) => handleChange('jobTypeId', e.target.value)}
            error={!!errors.jobTypeId}
            helperText={errors.jobTypeId || 'Contractor specialty'}
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

        {/* Rating */}
        <Grid item xs={12} sm={6}>
          <TextField
            label="Rating"
            type="number"
            value={formData.rating}
            onChange={(e) => handleChange('rating', parseFloat(e.target.value))}
            error={!!errors.rating}
            helperText={errors.rating || 'Performance rating (0.0-5.0, defaults to 3.0)'}
            inputProps={{
              min: 0,
              max: 5,
              step: 0.1,
            }}
            fullWidth
          />
        </Grid>

        {/* Phone */}
        <Grid item xs={12} sm={6}>
          <TextField
            label="Phone"
            value={formData.phone}
            onChange={(e) => handleChange('phone', e.target.value)}
            error={!!errors.phone}
            helperText={errors.phone || 'Contact phone number (optional)'}
            fullWidth
          />
        </Grid>

        {/* Email */}
        <Grid item xs={12}>
          <TextField
            label="Email"
            type="email"
            value={formData.email}
            onChange={(e) => handleChange('email', e.target.value)}
            error={!!errors.email}
            helperText={errors.email || 'Contact email address (optional)'}
            fullWidth
          />
        </Grid>

        {/* Location Section */}
        <Grid item xs={12}>
          <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
            Base Location
          </Typography>
        </Grid>

        {/* Address */}
        <Grid item xs={12}>
          <TextField
            label="Address"
            value={formData.baseLocation.address}
            onChange={(e) => handleLocationChange('address', e.target.value)}
            error={!!errors.address}
            helperText={errors.address || 'Street address, city, state, zip'}
            fullWidth
          />
        </Grid>

        {/* Latitude */}
        <Grid item xs={12} sm={6}>
          <TextField
            label="Latitude"
            type="number"
            value={formData.baseLocation.latitude}
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
            value={formData.baseLocation.longitude}
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
  );
}

/**
 * Validates contractor form data
 */
export function validateContractorFormData(formData: ContractorFormData): Record<string, string | undefined> {
  return {
    name: validateContractorName(formData.name),
    jobTypeId: !formData.jobTypeId ? 'Job type is required' : undefined,
    rating: validateRating(formData.rating),
    latitude: validateLatitude(formData.baseLocation.latitude),
    longitude: validateLongitude(formData.baseLocation.longitude),
    email: validateEmail(formData.email),
    phone: validatePhone(formData.phone),
  };
}
