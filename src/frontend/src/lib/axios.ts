import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { generateCorrelationId } from '../utils/correlation';

/**
 * Axios instance configured for SmartScheduler API
 *
 * Features:
 * - Base URL from environment variable
 * - Automatic correlation ID header for request tracing
 * - Global error handling with user-friendly messages
 * - JSON content type defaults
 */

// Get base URL from environment or fallback to localhost
const baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000, // 30 second timeout
});

/**
 * Request interceptor: Adds correlation ID to every request
 *
 * Correlation IDs help trace requests across frontend/backend logs
 * for debugging and monitoring.
 */
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Generate unique correlation ID for this request
    const correlationId = generateCorrelationId();

    // Add to headers for backend logging
    config.headers.set('X-Correlation-ID', correlationId);

    // Log request for debugging (dev only)
    if (import.meta.env.DEV) {
      console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, {
        correlationId,
        data: config.data,
      });
    }

    return config;
  },
  (error) => {
    console.error('[API Request Error]', error);
    return Promise.reject(error);
  }
);

/**
 * Response interceptor: Global error handling
 *
 * Transforms backend errors into user-friendly messages
 * and logs for debugging.
 */
apiClient.interceptors.response.use(
  (response) => {
    // Log successful response (dev only)
    if (import.meta.env.DEV) {
      const correlationId = response.config.headers.get('X-Correlation-ID');
      console.log(`[API Response] ${response.config.method?.toUpperCase()} ${response.config.url}`, {
        correlationId,
        status: response.status,
        data: response.data,
      });
    }

    return response;
  },
  (error: AxiosError) => {
    // Extract correlation ID from request for error tracking
    const correlationId = error.config?.headers?.get('X-Correlation-ID');

    // Build user-friendly error message
    let message = 'An unexpected error occurred';

    if (error.response) {
      // Server responded with error status
      const status = error.response.status;

      switch (status) {
        case 400:
          message = 'Invalid request. Please check your input.';
          break;
        case 401:
          message = 'Unauthorized. Please log in.';
          break;
        case 403:
          message = 'You do not have permission to perform this action.';
          break;
        case 404:
          message = 'The requested resource was not found.';
          break;
        case 409:
          message = 'Conflict. The resource already exists or is in use.';
          break;
        case 422:
          message = 'Validation failed. Please check your input.';
          break;
        case 500:
          message = 'Server error. Please try again later.';
          break;
        case 503:
          message = 'Service unavailable. Please try again later.';
          break;
        default:
          message = `Request failed with status ${status}`;
      }

      // Check for backend error message in response
      if (error.response.data && typeof error.response.data === 'object') {
        const data = error.response.data as Record<string, unknown>;
        if (data.message && typeof data.message === 'string') {
          message = data.message;
        } else if (data.title && typeof data.title === 'string') {
          // ASP.NET Core Problem Details format
          message = data.title;
        }
      }
    } else if (error.request) {
      // Request made but no response received
      message = 'Network error. Please check your connection.';
    }

    // Log error for debugging
    console.error('[API Error]', {
      correlationId,
      message,
      status: error.response?.status,
      url: error.config?.url,
      error,
    });

    // Attach user-friendly message to error
    const enrichedError = error as AxiosError & { userMessage?: string };
    enrichedError.userMessage = message;

    return Promise.reject(enrichedError);
  }
);

export default apiClient;
