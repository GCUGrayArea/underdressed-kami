import { Box, Typography, Button, Paper } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';

/**
 * 404 Not Found page
 */
export function NotFound() {
  const navigate = useNavigate();

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="400px"
    >
      <Paper sx={{ p: 4, textAlign: 'center', maxWidth: 500 }}>
        <ErrorOutlineIcon sx={{ fontSize: 64, color: 'error.main', mb: 2 }} />
        <Typography variant="h4" gutterBottom>
          404 - Page Not Found
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          The page you are looking for does not exist.
        </Typography>
        <Button variant="contained" onClick={() => navigate('/')}>
          Go to Dashboard
        </Button>
      </Paper>
    </Box>
  );
}
