import { Box, Typography, Paper } from '@mui/material';

/**
 * Contractors page - manage contractor list
 * Browse, search, filter, create, and edit contractors
 */
export function Contractors() {
  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Contractors
      </Typography>
      <Paper sx={{ p: 3, mt: 2 }}>
        <Typography variant="body1" color="text.secondary">
          Contractor management will be implemented in future PRs.
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          This page will display contractor list with search, filtering, and CRUD operations.
        </Typography>
      </Paper>
    </Box>
  );
}
