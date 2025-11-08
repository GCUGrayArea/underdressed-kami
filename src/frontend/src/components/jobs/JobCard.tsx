import { Card, CardContent, Typography, Box, Stack, Button, CardActions } from '@mui/material';
import { LocationOn, CalendarToday, AccessTime, Search } from '@mui/icons-material';
import type { JobDto } from '../../types/dto';
import { JobStatusBadge } from './JobStatusBadge';
import { JobStatus } from '../../types/dto';

interface JobCardProps {
  job: JobDto;
  onClick?: (job: JobDto) => void;
  onFindContractor?: (job: JobDto) => void;
}

/**
 * Individual job card component displaying job details
 */
export function JobCard({ job, onClick, onFindContractor }: JobCardProps) {
  const handleClick = () => {
    if (onClick) {
      onClick(job);
    }
  };

  const handleFindContractor = (e: React.MouseEvent) => {
    e.stopPropagation(); // Prevent card click
    if (onFindContractor) {
      onFindContractor(job);
    }
  };

  const isUnassigned = job.status === JobStatus.Unassigned;

  return (
    <Card
      onClick={handleClick}
      sx={{
        cursor: onClick ? 'pointer' : 'default',
        transition: 'all 0.2s',
        '&:hover': onClick ? {
          boxShadow: 3,
          transform: 'translateY(-2px)',
        } : {},
      }}
    >
      <CardContent>
        <Stack spacing={2}>
          <Box display="flex" justifyContent="space-between" alignItems="start">
            <Box>
              <Typography variant="h6" component="div">
                {job.formattedId}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {job.jobTypeName}
              </Typography>
            </Box>
            <JobStatusBadge status={job.status} />
          </Box>

          <Stack spacing={1}>
            <InfoRow
              icon={<LocationOn fontSize="small" />}
              text={`${job.latitude.toFixed(6)}, ${job.longitude.toFixed(6)}`}
            />
            <InfoRow
              icon={<CalendarToday fontSize="small" />}
              text={formatDate(job.desiredDate)}
            />
            {job.desiredTime && (
              <InfoRow
                icon={<AccessTime fontSize="small" />}
                text={job.desiredTime}
              />
            )}
          </Stack>

          <Typography variant="body2" color="text.secondary">
            Customer: {job.customerName}
          </Typography>

          {job.assignedContractorName && (
            <Typography variant="body2" color="primary">
              Assigned to: {job.assignedContractorName}
            </Typography>
          )}
        </Stack>
      </CardContent>

      {/* Find Contractor button for unassigned jobs */}
      {isUnassigned && onFindContractor && (
        <CardActions sx={{ pt: 0, pb: 2, px: 2 }}>
          <Button
            variant="contained"
            color="primary"
            size="small"
            fullWidth
            startIcon={<Search />}
            onClick={handleFindContractor}
          >
            Find Contractor
          </Button>
        </CardActions>
      )}
    </Card>
  );
}

interface InfoRowProps {
  icon: React.ReactNode;
  text: string;
}

function InfoRow({ icon, text }: InfoRowProps) {
  return (
    <Box display="flex" alignItems="center" gap={1}>
      {icon}
      <Typography variant="body2">{text}</Typography>
    </Box>
  );
}

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    weekday: 'short',
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}
