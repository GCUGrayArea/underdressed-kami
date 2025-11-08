/**
 * Location Input Component
 *
 * Reusable component for entering location data (address and coordinates).
 * For now, this accepts manual text input for address and lat/lon coordinates.
 * Future enhancement: Add geocoding API integration for automatic coordinate lookup.
 */

import { Grid, TextField, Box, Typography } from '@mui/material';

/**
 * Location data structure
 */
export interface LocationData {
  latitude: number;
  longitude: number;
  address: string;
}

interface LocationInputProps {
  value: LocationData;
  onChange: (location: LocationData) => void;
  errors?: {
    latitude?: string;
    longitude?: string;
    address?: string;
  };
  required?: boolean;
  addressRequired?: boolean;
}

/**
 * LocationInput Component
 *
 * Simple location input with text fields for address, latitude, and longitude.
 * Geocoding integration can be added in future iterations.
 */
export function LocationInput({
  value,
  onChange,
  errors = {},
  required = true,
  addressRequired = false,
}: LocationInputProps) {
  const handleChange = (field: keyof LocationData, newValue: string | number) => {
    onChange({
      ...value,
      [field]: newValue,
    });
  };

  return (
    <Box>
      <Grid container spacing={2}>
        {/* Address */}
        <Grid item xs={12}>
          <TextField
            label="Address"
            value={value.address}
            onChange={(e) => handleChange('address', e.target.value)}
            error={!!errors.address}
            helperText={errors.address || 'Street address, city, state, zip'}
            required={addressRequired}
            fullWidth
          />
        </Grid>

        {/* Latitude */}
        <Grid item xs={12} sm={6}>
          <TextField
            label="Latitude"
            type="number"
            value={value.latitude}
            onChange={(e) => handleChange('latitude', parseFloat(e.target.value) || 0)}
            error={!!errors.latitude}
            helperText={errors.latitude || 'Latitude (-90 to 90)'}
            required={required}
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
            value={value.longitude}
            onChange={(e) => handleChange('longitude', parseFloat(e.target.value) || 0)}
            error={!!errors.longitude}
            helperText={errors.longitude || 'Longitude (-180 to 180)'}
            required={required}
            inputProps={{
              min: -180,
              max: 180,
              step: 0.000001,
            }}
            fullWidth
          />
        </Grid>

        {/* Helper text for future geocoding */}
        <Grid item xs={12}>
          <Typography variant="caption" color="text.secondary">
            Note: Future versions will include automatic coordinate lookup from address
          </Typography>
        </Grid>
      </Grid>
    </Box>
  );
}
