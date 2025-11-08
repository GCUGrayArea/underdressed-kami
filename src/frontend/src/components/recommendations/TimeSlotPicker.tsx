import { Box, Typography, Chip, Stack } from '@mui/material';
import { AccessTime } from '@mui/icons-material';
import type { TimeSlotDto } from '../../types/dto';

interface TimeSlotPickerProps {
  slot?: TimeSlotDto;
}

/**
 * TimeSlotPicker component
 * Displays available time slot for a contractor
 */
export function TimeSlotPicker({ slot }: TimeSlotPickerProps) {
  if (!slot) {
    return (
      <Box sx={{ py: 1 }}>
        <Typography variant="body2" color="text.secondary">
          No available slots
        </Typography>
      </Box>
    );
  }

  return (
    <Stack spacing={1}>
      <Typography variant="subtitle2" fontWeight="bold">
        Best Available Slot
      </Typography>
      <Chip
        icon={<AccessTime />}
        label={formatTimeSlot(slot)}
        color="primary"
        variant="outlined"
        sx={{ width: 'fit-content' }}
      />
    </Stack>
  );
}

/**
 * Format time slot for display
 */
function formatTimeSlot(slot: TimeSlotDto): string {
  const start = formatTime(slot.start);
  const end = formatTime(slot.end);
  return `${start} - ${end}`;
}

/**
 * Format TimeOnly string (HH:mm:ss) to 12-hour format
 */
function formatTime(timeString: string): string {
  const [hours, minutes] = timeString.split(':').map(Number);
  const period = hours >= 12 ? 'PM' : 'AM';
  const displayHours = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours;
  return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
}
