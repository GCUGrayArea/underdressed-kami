import { Box, Typography, Paper } from '@mui/material';

/**
 * Dashboard page - main dispatcher view
 * Shows unassigned and assigned jobs
 */
export function Dashboard() {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Paper sx={{ p: 3, mt: 2 }}>
        <Typography variant="body1" color="text.secondary">
          Job dashboard will be implemented in future PRs.
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          This page will display unassigned jobs and assigned jobs grouped by date.
        </Typography>
      </Paper>
    </Box>
  );
}
