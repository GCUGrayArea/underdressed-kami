import {
  Card,
  CardContent,
  Typography,
  Box,
  Stack,
  Rating,
  Chip,
  Button,
} from '@mui/material';
import { LocationOn, EmojiEvents } from '@mui/icons-material';
import type { RankedContractorDto } from '../../types/dto';
import { ScoreBreakdown } from './ScoreBreakdown';
import { TimeSlotPicker } from './TimeSlotPicker';

interface ContractorRecommendationCardProps {
  contractor: RankedContractorDto;
  rank: number;
  onAssign?: (contractor: RankedContractorDto) => void;
}

/**
 * ContractorRecommendationCard component
 * Displays a single ranked contractor with score and availability
 */
export function ContractorRecommendationCard({
  contractor,
  rank,
  onAssign,
}: ContractorRecommendationCardProps) {
  const handleAssign = () => {
    if (onAssign) {
      onAssign(contractor);
    }
  };

  return (
    <Card
      sx={{
        position: 'relative',
        border: rank === 1 ? 2 : 1,
        borderColor: rank === 1 ? 'primary.main' : 'divider',
      }}
    >
      {rank === 1 && (
        <Box
          sx={{
            position: 'absolute',
            top: -12,
            left: 16,
            bgcolor: 'primary.main',
            color: 'white',
            px: 2,
            py: 0.5,
            borderRadius: 1,
            display: 'flex',
            alignItems: 'center',
            gap: 0.5,
          }}
        >
          <EmojiEvents fontSize="small" />
          <Typography variant="caption" fontWeight="bold">
            Best Match
          </Typography>
        </Box>
      )}

      <CardContent sx={{ pt: rank === 1 ? 3 : 2 }}>
        <Stack spacing={2.5}>
          {/* Header with name and ID */}
          <Box>
            <Box
              display="flex"
              justifyContent="space-between"
              alignItems="start"
              mb={1}
            >
              <Box>
                <Typography variant="h6" component="div">
                  {contractor.name}
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  fontFamily="monospace"
                >
                  {contractor.formattedId}
                </Typography>
              </Box>
              <Chip
                label={`Rank #${rank}`}
                color={rank === 1 ? 'primary' : 'default'}
                size="small"
              />
            </Box>

            {/* Job type and rating */}
            <Stack direction="row" spacing={2} alignItems="center">
              <Typography variant="body2" color="text.secondary">
                {contractor.jobType}
              </Typography>
              <Box display="flex" alignItems="center" gap={0.5}>
                <Rating
                  value={contractor.rating}
                  precision={0.1}
                  readOnly
                  size="small"
                />
                <Typography variant="body2" color="text.secondary">
                  {contractor.rating.toFixed(1)}
                </Typography>
              </Box>
            </Stack>
          </Box>

          {/* Distance */}
          <Box display="flex" alignItems="center" gap={1}>
            <LocationOn fontSize="small" color="action" />
            <Typography variant="body2">
              {contractor.distanceMiles.toFixed(1)} miles away
            </Typography>
          </Box>

          {/* Time slot */}
          <TimeSlotPicker slot={contractor.bestAvailableSlot} />

          {/* Score breakdown */}
          <ScoreBreakdown scoreBreakdown={contractor.scoreBreakdown} />

          {/* Assign button */}
          {onAssign && (
            <Button
              variant="contained"
              fullWidth
              onClick={handleAssign}
              disabled={!contractor.bestAvailableSlot}
            >
              Assign Contractor
            </Button>
          )}
        </Stack>
      </CardContent>
    </Card>
  );
}
