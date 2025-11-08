/**
 * API Request and Response Types
 *
 * Generic types for API communication patterns used across the application.
 * Specific endpoint types (ContractorDto, JobDto, etc.) are defined in dto.ts
 */

/**
 * Generic paginated response wrapper
 */
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

/**
 * Generic API error response
 */
export interface ApiError {
  message: string;
  status: number;
  correlationId?: string;
  errors?: Record<string, string[]>; // Validation errors
}

/**
 * Generic API success response wrapper
 */
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

/**
 * Query parameters for list endpoints with pagination
 */
export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

/**
 * Query parameters for search/filter endpoints
 */
export interface SearchParams extends PaginationParams {
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}
