import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  Rating,
  Box,
  Typography,
  TablePagination,
  CircularProgress,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import type { ContractorListItemDto } from '../../types/dto';

interface ContractorTableProps {
  contractors: ContractorListItemDto[];
  loading?: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (newPage: number) => void;
  onPageSizeChange: (newPageSize: number) => void;
}

/**
 * ContractorTable component
 * Displays contractors in a table with pagination and navigation
 */
export function ContractorTable({
  contractors,
  loading = false,
  page,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange,
}: ContractorTableProps) {
  const navigate = useNavigate();

  const handleRowClick = (contractorId: string) => {
    // Navigate to contractor detail/edit page (will be implemented in future PR)
    navigate(`/contractors/${contractorId}`);
  };

  const handleChangePage = (_event: unknown, newPage: number) => {
    onPageChange(newPage + 1); // Material-UI uses 0-indexed pages, backend uses 1-indexed
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    onPageSizeChange(parseInt(event.target.value, 10));
    onPageChange(1); // Reset to first page when changing page size
  };

  if (loading && contractors.length === 0) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (contractors.length === 0) {
    return (
      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="body1" color="text.secondary">
          No contractors found matching your filters.
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          Try adjusting your search criteria or filters.
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Rating</TableCell>
              <TableCell>Contact</TableCell>
              <TableCell>Status</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {contractors.map((contractor) => (
              <TableRow
                key={contractor.id}
                hover
                onClick={() => handleRowClick(contractor.id)}
                sx={{ cursor: 'pointer' }}
              >
                <TableCell>
                  <Typography variant="body2" fontFamily="monospace">
                    {contractor.formattedId}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body1" fontWeight="medium">
                    {contractor.name}
                  </Typography>
                </TableCell>
                <TableCell>{contractor.jobTypeName}</TableCell>
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Rating value={contractor.rating} precision={0.1} readOnly size="small" />
                    <Typography variant="body2" color="text.secondary">
                      {contractor.rating.toFixed(1)}
                    </Typography>
                  </Box>
                </TableCell>
                <TableCell>
                  {contractor.phone && (
                    <Typography variant="body2">{contractor.phone}</Typography>
                  )}
                  {contractor.email && (
                    <Typography variant="body2" color="text.secondary" sx={{ fontSize: '0.875rem' }}>
                      {contractor.email}
                    </Typography>
                  )}
                  {!contractor.phone && !contractor.email && (
                    <Typography variant="body2" color="text.secondary">
                      No contact info
                    </Typography>
                  )}
                </TableCell>
                <TableCell>
                  <Chip
                    label={contractor.isActive ? 'Active' : 'Inactive'}
                    color={contractor.isActive ? 'success' : 'default'}
                    size="small"
                  />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={totalCount}
        page={page - 1} // Material-UI uses 0-indexed pages
        onPageChange={handleChangePage}
        rowsPerPage={pageSize}
        onRowsPerPageChange={handleChangeRowsPerPage}
        rowsPerPageOptions={[10, 20, 50, 100]}
      />
    </Paper>
  );
}
