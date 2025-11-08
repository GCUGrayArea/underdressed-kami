import { useState, useMemo } from 'react';
import { Box, Typography, Button, Alert } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { useNavigate } from 'react-router-dom';
import { ContractorSearch } from '../components/contractors/ContractorSearch';
import { ContractorFilters, type ContractorFilterValues } from '../components/contractors/ContractorFilters';
import { ContractorTable } from '../components/contractors/ContractorTable';
import { useContractors } from '../hooks/useContractors';

/**
 * Contractors page - manage contractor list
 * Browse, search, filter, create, and edit contractors
 */
export function Contractors() {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [filters, setFilters] = useState<ContractorFilterValues>({});
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);

  // Fetch contractors with current filters and pagination
  const { data, isLoading, error } = useContractors({
    ...filters,
    page,
    pageSize,
  });

  // Client-side search filtering by name
  // Backend doesn't support name search yet, so we filter on the client
  const filteredContractors = useMemo(() => {
    if (!data?.items) return [];
    if (!searchTerm.trim()) return data.items;

    const lowerSearch = searchTerm.toLowerCase();
    return data.items.filter((contractor) =>
      contractor.name.toLowerCase().includes(lowerSearch)
    );
  }, [data?.items, searchTerm]);

  const handleAddContractor = () => {
    // Navigate to create contractor form (will be implemented in future PR)
    navigate('/contractors/new');
  };

  const handleSearchChange = (newSearchTerm: string) => {
    setSearchTerm(newSearchTerm);
  };

  const handleFiltersChange = (newFilters: ContractorFilterValues) => {
    setFilters(newFilters);
    setPage(1); // Reset to first page when filters change
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1); // Reset to first page when page size changes
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">
          Contractors
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleAddContractor}
        >
          Add Contractor
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          Failed to load contractors: {error.message}
        </Alert>
      )}

      <ContractorSearch onSearchChange={handleSearchChange} />

      <ContractorFilters
        filters={filters}
        onFiltersChange={handleFiltersChange}
        jobTypes={[
          // Hardcoded job types for now - will be fetched from API in future
          { id: '1', name: 'Tile Installer' },
          { id: '2', name: 'Carpet Installer' },
          { id: '3', name: 'Hardwood Specialist' },
        ]}
      />

      <ContractorTable
        contractors={filteredContractors}
        loading={isLoading}
        page={page}
        pageSize={pageSize}
        totalCount={data?.totalCount || 0}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
    </Box>
  );
}
