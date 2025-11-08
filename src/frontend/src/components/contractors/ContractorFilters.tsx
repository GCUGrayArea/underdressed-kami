import { Box, FormControl, InputLabel, Select, MenuItem, TextField, Grid, Button, SelectChangeEvent } from '@mui/material';
import FilterListIcon from '@mui/icons-material/FilterList';
import ClearIcon from '@mui/icons-material/Clear';

export interface ContractorFilterValues {
  jobTypeId?: string;
  minRating?: number;
  maxRating?: number;
  isActive?: boolean;
}

interface ContractorFiltersProps {
  filters: ContractorFilterValues;
  onFiltersChange: (filters: ContractorFilterValues) => void;
  jobTypes?: Array<{ id: string; name: string }>;
}

/**
 * ContractorFilters component
 * Provides filter controls for job type, rating range, and active status
 */
export function ContractorFilters({
  filters,
  onFiltersChange,
  jobTypes = [],
}: ContractorFiltersProps) {
  const handleJobTypeChange = (event: SelectChangeEvent<string>) => {
    onFiltersChange({
      ...filters,
      jobTypeId: event.target.value || undefined,
    });
  };

  const handleMinRatingChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    onFiltersChange({
      ...filters,
      minRating: value ? parseFloat(value) : undefined,
    });
  };

  const handleMaxRatingChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    onFiltersChange({
      ...filters,
      maxRating: value ? parseFloat(value) : undefined,
    });
  };

  const handleStatusChange = (event: SelectChangeEvent<string>) => {
    const value = event.target.value;
    onFiltersChange({
      ...filters,
      isActive: value === '' ? undefined : value === 'true',
    });
  };

  const handleClearFilters = () => {
    onFiltersChange({
      jobTypeId: undefined,
      minRating: undefined,
      maxRating: undefined,
      isActive: undefined,
    });
  };

  const hasActiveFilters =
    filters.jobTypeId !== undefined ||
    filters.minRating !== undefined ||
    filters.maxRating !== undefined ||
    filters.isActive !== undefined;

  return (
    <Box sx={{ mb: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
        <FilterListIcon sx={{ mr: 1 }} />
        <Box sx={{ fontWeight: 'medium' }}>Filters</Box>
        {hasActiveFilters && (
          <Button
            size="small"
            startIcon={<ClearIcon />}
            onClick={handleClearFilters}
            sx={{ ml: 'auto' }}
          >
            Clear Filters
          </Button>
        )}
      </Box>

      <Grid container spacing={2}>
        <Grid item xs={12} sm={6} md={3}>
          <FormControl fullWidth size="small">
            <InputLabel>Job Type</InputLabel>
            <Select
              value={filters.jobTypeId || ''}
              label="Job Type"
              onChange={handleJobTypeChange}
            >
              <MenuItem value="">All Types</MenuItem>
              {jobTypes.map((jobType) => (
                <MenuItem key={jobType.id} value={jobType.id}>
                  {jobType.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <TextField
            fullWidth
            size="small"
            label="Min Rating"
            type="number"
            value={filters.minRating ?? ''}
            onChange={handleMinRatingChange}
            inputProps={{
              min: 0,
              max: 5,
              step: 0.1,
            }}
          />
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <TextField
            fullWidth
            size="small"
            label="Max Rating"
            type="number"
            value={filters.maxRating ?? ''}
            onChange={handleMaxRatingChange}
            inputProps={{
              min: 0,
              max: 5,
              step: 0.1,
            }}
          />
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <FormControl fullWidth size="small">
            <InputLabel>Status</InputLabel>
            <Select
              value={filters.isActive === undefined ? '' : filters.isActive.toString()}
              label="Status"
              onChange={handleStatusChange}
            >
              <MenuItem value="">All</MenuItem>
              <MenuItem value="true">Active</MenuItem>
              <MenuItem value="false">Inactive</MenuItem>
            </Select>
          </FormControl>
        </Grid>
      </Grid>
    </Box>
  );
}
