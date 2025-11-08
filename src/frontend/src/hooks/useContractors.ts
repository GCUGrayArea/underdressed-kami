import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { contractorApi, type ContractorSearchParams } from '../services/contractorApi';
import type { ContractorDto, ContractorListItemDto } from '../types/dto';
import type { PagedResult } from '../types/api';

/**
 * Query key factory for contractor queries
 */
export const contractorKeys = {
  all: ['contractors'] as const,
  lists: () => [...contractorKeys.all, 'list'] as const,
  list: (params: ContractorSearchParams) => [...contractorKeys.lists(), params] as const,
  details: () => [...contractorKeys.all, 'detail'] as const,
  detail: (id: string) => [...contractorKeys.details(), id] as const,
};

/**
 * Hook to fetch paginated and filtered contractors
 */
export function useContractors(params: ContractorSearchParams = {}) {
  return useQuery<PagedResult<ContractorListItemDto>, Error>({
    queryKey: contractorKeys.list(params),
    queryFn: () => contractorApi.getContractors(params),
    staleTime: 30000, // Consider data fresh for 30 seconds
    placeholderData: (previousData) => previousData, // Keep previous data while fetching
  });
}

/**
 * Hook to fetch single contractor by ID
 */
export function useContractor(id: string) {
  return useQuery<ContractorDto, Error>({
    queryKey: contractorKeys.detail(id),
    queryFn: () => contractorApi.getContractorById(id),
    enabled: !!id, // Only run query if ID is provided
  });
}

/**
 * Hook to create a new contractor
 */
export function useCreateContractor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: contractorApi.createContractor.bind(contractorApi),
    onSuccess: () => {
      // Invalidate all contractor list queries to refetch
      queryClient.invalidateQueries({ queryKey: contractorKeys.lists() });
    },
  });
}

/**
 * Hook to update an existing contractor
 */
export function useUpdateContractor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Parameters<typeof contractorApi.updateContractor>[1] }) =>
      contractorApi.updateContractor(id, data),
    onSuccess: (_, variables) => {
      // Invalidate specific contractor detail query
      queryClient.invalidateQueries({ queryKey: contractorKeys.detail(variables.id) });
      // Invalidate all list queries
      queryClient.invalidateQueries({ queryKey: contractorKeys.lists() });
    },
  });
}

/**
 * Hook to delete (deactivate) a contractor
 */
export function useDeleteContractor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: contractorApi.deleteContractor.bind(contractorApi),
    onSuccess: () => {
      // Invalidate all contractor queries
      queryClient.invalidateQueries({ queryKey: contractorKeys.all });
    },
  });
}
