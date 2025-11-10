import { ApiService } from './api';
import type { RankedContractorDto } from '../types/dto';

/**
 * Request parameters for contractor recommendations
 */
export interface GetRecommendationsRequest {
  jobTypeId: string;
  desiredDate: string; // ISO date string
  desiredTime: string; // "HH:mm:ss" format
  location: {
    latitude: number;
    longitude: number;
    address?: string;
  };
  estimatedDurationHours: number;
  topN?: number; // Default 5
}

/**
 * Recommendation API service
 * Handles contractor recommendation requests
 */
class RecommendationApiService extends ApiService {
  /**
   * Get ranked contractor recommendations for a job
   */
  async getRecommendations(
    request: GetRecommendationsRequest
  ): Promise<RankedContractorDto[]> {
    return this.post<RankedContractorDto[]>(
      '/api/recommendations/contractors',
      request
    );
  }
}

export const recommendationApi = new RecommendationApiService();
