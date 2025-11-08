/**
 * Working Hours Editor Component
 *
 * Allows editing weekly schedule with daily start/end times.
 * Supports enabling/disabling specific days of the week.
 */

import { Box, Checkbox, FormControlLabel, Grid, TextField, Typography } from '@mui/material';
import { validateTimeFormat, validateTimeRange } from '../../utils/validation';

/**
 * Working hours schedule entry
 */
export interface WorkingHoursSchedule {
  dayOfWeek: number; // 0=Sunday, 1=Monday, ..., 6=Saturday
  startTime: string; // HH:mm format
  endTime: string; // HH:mm format
  enabled: boolean; // Whether this day is a working day
}

interface WorkingHoursEditorProps {
  schedules: WorkingHoursSchedule[];
  onChange: (schedules: WorkingHoursSchedule[]) => void;
  error?: string;
}

const DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

/**
 * Working Hours Editor Component
 */
export function WorkingHoursEditor({ schedules, onChange, error }: WorkingHoursEditorProps) {
  const handleToggleDay = (dayOfWeek: number) => {
    const newSchedules = schedules.map((schedule) =>
      schedule.dayOfWeek === dayOfWeek ? { ...schedule, enabled: !schedule.enabled } : schedule
    );
    onChange(newSchedules);
  };

  const handleTimeChange = (dayOfWeek: number, field: 'startTime' | 'endTime', value: string) => {
    const newSchedules = schedules.map((schedule) =>
      schedule.dayOfWeek === dayOfWeek ? { ...schedule, [field]: value } : schedule
    );
    onChange(newSchedules);
  };

  const getTimeError = (schedule: WorkingHoursSchedule, field: 'startTime' | 'endTime'): string | undefined => {
    if (!schedule.enabled) {
      return undefined;
    }

    const formatError = validateTimeFormat(schedule[field]);
    if (formatError) {
      return formatError;
    }

    if (field === 'endTime' && schedule.startTime && schedule.endTime) {
      return validateTimeRange(schedule.startTime, schedule.endTime);
    }

    return undefined;
  };

  return (
    <Box>
      <Typography variant="subtitle1" gutterBottom>
        Working Hours
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Set the working hours for each day of the week
      </Typography>

      {error && (
        <Typography variant="body2" color="error" sx={{ mb: 2 }}>
          {error}
        </Typography>
      )}

      {schedules.map((schedule) => (
        <Box
          key={schedule.dayOfWeek}
          sx={{
            mb: 2,
            p: 2,
            border: 1,
            borderColor: 'divider',
            borderRadius: 1,
            opacity: schedule.enabled ? 1 : 0.6,
          }}
        >
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} sm={3}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={schedule.enabled}
                    onChange={() => handleToggleDay(schedule.dayOfWeek)}
                  />
                }
                label={DAY_NAMES[schedule.dayOfWeek]}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="Start Time"
                type="time"
                value={schedule.startTime}
                onChange={(e) => handleTimeChange(schedule.dayOfWeek, 'startTime', e.target.value)}
                disabled={!schedule.enabled}
                fullWidth
                size="small"
                error={!!getTimeError(schedule, 'startTime')}
                helperText={getTimeError(schedule, 'startTime')}
                InputLabelProps={{
                  shrink: true,
                }}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="End Time"
                type="time"
                value={schedule.endTime}
                onChange={(e) => handleTimeChange(schedule.dayOfWeek, 'endTime', e.target.value)}
                disabled={!schedule.enabled}
                fullWidth
                size="small"
                error={!!getTimeError(schedule, 'endTime')}
                helperText={getTimeError(schedule, 'endTime')}
                InputLabelProps={{
                  shrink: true,
                }}
              />
            </Grid>
          </Grid>
        </Box>
      ))}
    </Box>
  );
}
