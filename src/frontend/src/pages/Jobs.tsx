import { Box, Typography, Paper } from '@mui/material';

/**
 * Jobs page - manage jobs
 * View, create, and manage job requests
 */
export function Jobs() {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Jobs
      </Typography>
      <Paper sx={{ p: 3, mt: 2 }}>
        <Typography variant="body1" color="text.secondary">
          Job management will be implemented in future PRs.
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          This page will display job list with filtering by status and CRUD operations.
        </Typography>
      </Paper>
    </Box>
  );
}
