import { ApiService } from './api';
import type { ContractorDto, ContractorListItemDto } from '../types/dto';
import type { PagedResult } from '../types/api';

/**
 * Contractor search/filter parameters matching backend SearchContractorsQuery
 */
export interface ContractorSearchParams {
  jobTypeId?: string;
  minRating?: number;
  maxRating?: number;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
  searchTerm?: string; // For name search (client-side filtering initially)
}

/**
 * Contractor API Service
 * Provides methods for contractor CRUD operations
 */
class ContractorApiService extends ApiService {
  private readonly BASE_PATH = '/contractors';

  /**
   * Get all contractors with pagination and filtering
   */
  async getContractors(params?: ContractorSearchParams): Promise<PagedResult<ContractorListItemDto>> {
    const queryParams: Record<string, unknown> = {
      page: params?.page || 1,
      pageSize: params?.pageSize || 20,
    };

    // Add optional filters
    if (params?.jobTypeId) {
      queryParams.jobTypeId = params.jobTypeId;
    }
    if (params?.minRating !== undefined) {
      queryParams.minRating = params.minRating;
    }
    if (params?.maxRating !== undefined) {
      queryParams.maxRating = params.maxRating;
    }
    if (params?.isActive !== undefined) {
      queryParams.isActive = params.isActive;
    }

    return this.get<PagedResult<ContractorListItemDto>>(this.BASE_PATH, queryParams);
  }

  /**
   * Get contractor by ID
   */
  async getContractorById(id: string): Promise<ContractorDto> {
    return this.get<ContractorDto>(`${this.BASE_PATH}/${id}`);
  }

  /**
   * Create new contractor
   */
  async createContractor(data: CreateContractorData): Promise<ContractorDto> {
    return this.post<ContractorDto, CreateContractorData>(this.BASE_PATH, data);
  }

  /**
   * Update contractor
   */
  async updateContractor(id: string, data: UpdateContractorData): Promise<void> {
    return this.put<void, UpdateContractorData>(`${this.BASE_PATH}/${id}`, data);
  }

  /**
   * Delete contractor (soft delete - marks as inactive)
   */
  async deleteContractor(id: string): Promise<void> {
    return this.delete<void>(`${this.BASE_PATH}/${id}`);
  }
}

/**
 * Create contractor request data
 */
export interface CreateContractorData {
  name: string;
  jobTypeId: string;
  baseLocation: {
    latitude: number;
    longitude: number;
    address?: string;
  };
  phone?: string;
  email?: string;
  rating?: number;
}

/**
 * Update contractor request data
 */
export interface UpdateContractorData {
  name: string;
  jobTypeId: string;
  baseLocation: {
    latitude: number;
    longitude: number;
    address?: string;
  };
  phone?: string;
  email?: string;
}

/**
 * Singleton instance of contractor API service
 */
export const contractorApi = new ContractorApiService();
