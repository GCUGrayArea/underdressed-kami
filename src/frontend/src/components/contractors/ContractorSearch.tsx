import { useState, useEffect } from 'react';
import { TextField, InputAdornment } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';

interface ContractorSearchProps {
  onSearchChange: (searchTerm: string) => void;
  placeholder?: string;
  debounceMs?: number;
}

/**
 * ContractorSearch component
 * Provides debounced search input for filtering contractors by name
 */
export function ContractorSearch({
  onSearchChange,
  placeholder = 'Search contractors by name...',
  debounceMs = 300,
}: ContractorSearchProps) {
  const [searchTerm, setSearchTerm] = useState('');

  // Debounce search term changes
  useEffect(() => {
    const timer = setTimeout(() => {
      onSearchChange(searchTerm);
    }, debounceMs);

    return () => clearTimeout(timer);
  }, [searchTerm, debounceMs, onSearchChange]);

  return (
    <TextField
      fullWidth
      variant="outlined"
      placeholder={placeholder}
      value={searchTerm}
      onChange={(e) => setSearchTerm(e.target.value)}
      InputProps={{
        startAdornment: (
          <InputAdornment position="start">
            <SearchIcon />
          </InputAdornment>
        ),
      }}
      sx={{ mb: 2 }}
    />
  );
}
