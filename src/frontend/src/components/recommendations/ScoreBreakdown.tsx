import { Box, Typography, LinearProgress, Stack } from '@mui/material';
import type { ScoreBreakdownDto } from '../../types/dto';

interface ScoreBreakdownProps {
  scoreBreakdown: ScoreBreakdownDto;
}

/**
 * ScoreBreakdown component
 * Displays visual breakdown of contractor ranking scores
 */
export function ScoreBreakdown({ scoreBreakdown }: ScoreBreakdownProps) {
  return (
    <Stack spacing={1.5}>
      <Typography variant="subtitle2" fontWeight="bold">
        Score Breakdown
      </Typography>

      <ScoreItem
        label="Availability"
        score={scoreBreakdown.availabilityScore}
        color="primary"
      />
      <ScoreItem
        label="Rating"
        score={scoreBreakdown.ratingScore}
        color="secondary"
      />
      <ScoreItem
        label="Distance"
        score={scoreBreakdown.distanceScore}
        color="success"
      />

      <Box sx={{ pt: 1, borderTop: 1, borderColor: 'divider' }}>
        <ScoreItem
          label="Overall Score"
          score={scoreBreakdown.overallScore}
          color="primary"
          variant="determinate"
          bold
        />
      </Box>
    </Stack>
  );
}

interface ScoreItemProps {
  label: string;
  score: number;
  color: 'primary' | 'secondary' | 'success';
  variant?: 'determinate' | 'indeterminate';
  bold?: boolean;
}

/**
 * Individual score item with label, progress bar, and percentage
 */
function ScoreItem({
  label,
  score,
  color,
  variant = 'determinate',
  bold = false,
}: ScoreItemProps) {
  const percentage = Math.round(score * 100);

  return (
    <Box>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 0.5,
        }}
      >
        <Typography
          variant="body2"
          fontWeight={bold ? 'bold' : 'normal'}
        >
          {label}
        </Typography>
        <Typography
          variant="body2"
          color="text.secondary"
          fontWeight={bold ? 'bold' : 'normal'}
        >
          {percentage}%
        </Typography>
      </Box>
      <LinearProgress
        variant={variant}
        value={percentage}
        color={color}
        sx={{ height: bold ? 8 : 6, borderRadius: 1 }}
      />
    </Box>
  );
}
