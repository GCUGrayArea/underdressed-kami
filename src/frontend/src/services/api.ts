import type { AxiosError, AxiosResponse } from 'axios';
import { apiClient } from '../lib/axios';
import type { PagedResult, PaginationParams } from '../types/api';

/**
 * Base API service class
 *
 * Provides common methods for interacting with the backend API.
 * All specific API services (contractors, jobs, etc.) should extend this class
 * or use these utilities.
 */
export class ApiService {
  /**
   * Generic GET request
   */
  protected async get<T>(url: string, params?: Record<string, unknown>): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.get(url, { params });
    return response.data;
  }

  /**
   * Generic POST request
   */
  protected async post<T, D = unknown>(url: string, data?: D): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.post(url, data);
    return response.data;
  }

  /**
   * Generic PUT request
   */
  protected async put<T, D = unknown>(url: string, data?: D): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.put(url, data);
    return response.data;
  }

  /**
   * Generic DELETE request
   */
  protected async delete<T>(url: string): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.delete(url);
    return response.data;
  }

  /**
   * Generic PATCH request
   */
  protected async patch<T, D = unknown>(url: string, data?: D): Promise<T> {
    const response: AxiosResponse<T> = await apiClient.patch(url, data);
    return response.data;
  }

  /**
   * Build query string from pagination parameters
   */
  protected buildPaginationParams(params?: PaginationParams): Record<string, unknown> {
    if (!params) return {};

    return {
      ...(params.page !== undefined && { page: params.page }),
      ...(params.pageSize !== undefined && { pageSize: params.pageSize }),
    };
  }

  /**
   * Check if error is an Axios error
   */
  protected isAxiosError(error: unknown): error is AxiosError {
    return (error as AxiosError).isAxiosError === true;
  }

  /**
   * Extract user-friendly error message from error object
   */
  protected getErrorMessage(error: unknown): string {
    if (this.isAxiosError(error)) {
      const axiosError = error as AxiosError & { userMessage?: string };
      return axiosError.userMessage || axiosError.message || 'An unexpected error occurred';
    }

    if (error instanceof Error) {
      return error.message;
    }

    return 'An unexpected error occurred';
  }
}

/**
 * Singleton instance of base API service
 */
export const apiService = new ApiService();

/**
 * Helper function to handle paginated responses
 */
export function createPagedResult<T>(
  items: T[],
  page: number,
  pageSize: number,
  totalCount: number
): PagedResult<T> {
  return {
    items,
    page,
    pageSize,
    totalCount,
    totalPages: Math.ceil(totalCount / pageSize),
  };
}
