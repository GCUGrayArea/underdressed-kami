import { ApiService } from './api';
import type { JobDto, JobStatus } from '../types/dto';
import type { PagedResult } from '../types/api';

/**
 * Job search/filter parameters
 */
export interface JobSearchParams {
  status?: JobStatus;
  contractorId?: string;
  jobTypeId?: string;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
}

/**
 * Job API Service
 * Provides methods for job operations
 */
class JobApiService extends ApiService {
  private readonly BASE_PATH = '/jobs';

  /**
   * Get all jobs with optional filtering
   */
  async getJobs(params?: JobSearchParams): Promise<PagedResult<JobDto>> {
    const queryParams: Record<string, unknown> = {
      page: params?.page || 1,
      pageSize: params?.pageSize || 50,
    };

    // Add optional filters
    if (params?.status !== undefined) {
      queryParams.status = params.status;
    }
    if (params?.contractorId) {
      queryParams.contractorId = params.contractorId;
    }
    if (params?.jobTypeId) {
      queryParams.jobTypeId = params.jobTypeId;
    }
    if (params?.fromDate) {
      queryParams.fromDate = params.fromDate;
    }
    if (params?.toDate) {
      queryParams.toDate = params.toDate;
    }

    return this.get<PagedResult<JobDto>>(this.BASE_PATH, queryParams);
  }

  /**
   * Get job by ID
   */
  async getJobById(id: string): Promise<JobDto> {
    return this.get<JobDto>(`${this.BASE_PATH}/${id}`);
  }

  /**
   * Create new job
   */
  async createJob(data: CreateJobData): Promise<JobDto> {
    return this.post<JobDto, CreateJobData>(this.BASE_PATH, data);
  }

  /**
   * Update job
   */
  async updateJob(id: string, data: UpdateJobData): Promise<void> {
    return this.put<void, UpdateJobData>(`${this.BASE_PATH}/${id}`, data);
  }

  /**
   * Assign contractor to job
   */
  async assignJob(id: string, data: AssignJobData): Promise<void> {
    return this.post<void, AssignJobData>(`${this.BASE_PATH}/${id}/assign`, data);
  }

  /**
   * Cancel job
   */
  async cancelJob(id: string): Promise<void> {
    return this.post<void>(`${this.BASE_PATH}/${id}/cancel`);
  }
}

/**
 * Create job request data
 */
export interface CreateJobData {
  jobTypeId: string;
  customerId: string;
  customerName: string;
  latitude: number;
  longitude: number;
  desiredDate: string;
  desiredTime?: string;
  estimatedDurationHours: number;
}

/**
 * Update job request data
 */
export interface UpdateJobData {
  jobTypeId: string;
  customerId: string;
  customerName: string;
  latitude: number;
  longitude: number;
  desiredDate: string;
  desiredTime?: string;
  estimatedDurationHours: number;
}

/**
 * Assign job request data
 */
export interface AssignJobData {
  contractorId: string;
  scheduledStartTime: string;
}

/**
 * Singleton instance of job API service
 */
export const jobApi = new JobApiService();
