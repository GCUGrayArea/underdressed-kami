import { Box, CircularProgress, Typography } from '@mui/material';

/**
 * Loading fallback component displayed during lazy loading of routes
 */
export function LoadingFallback() {
  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="400px"
      gap={2}
    >
      <CircularProgress size={48} />
      <Typography variant="body1" color="text.secondary">
        Loading...
      </Typography>
    </Box>
  );
}
