import { useCallback } from 'react';
import { AxiosError } from 'axios';

/**
 * Custom hook for handling API errors
 *
 * Provides utilities to extract user-friendly error messages
 * and handle common error scenarios.
 *
 * Usage:
 * ```tsx
 * const { getErrorMessage, handleError } = useApiError();
 *
 * try {
 *   await apiCall();
 * } catch (error) {
 *   const message = getErrorMessage(error);
 *   // Show message to user
 * }
 * ```
 */
export function useApiError() {
  /**
   * Extracts user-friendly error message from error object
   */
  const getErrorMessage = useCallback((error: unknown): string => {
    // Check if it's an Axios error with our custom userMessage
    if (isAxiosError(error)) {
      const axiosError = error as AxiosError & { userMessage?: string };
      if (axiosError.userMessage) {
        return axiosError.userMessage;
      }

      // Check response data for error message
      if (axiosError.response?.data) {
        const data = axiosError.response.data as Record<string, unknown>;
        if (data.message && typeof data.message === 'string') {
          return data.message;
        }
        if (data.title && typeof data.title === 'string') {
          return data.title;
        }
      }

      return axiosError.message || 'An unexpected error occurred';
    }

    // Handle standard JavaScript errors
    if (error instanceof Error) {
      return error.message;
    }

    // Handle string errors
    if (typeof error === 'string') {
      return error;
    }

    // Fallback for unknown error types
    return 'An unexpected error occurred';
  }, []);

  /**
   * Gets validation errors from API response
   *
   * Returns a map of field names to error messages for form validation.
   */
  const getValidationErrors = useCallback((error: unknown): Record<string, string[]> | null => {
    if (!isAxiosError(error)) {
      return null;
    }

    const axiosError = error as AxiosError;
    if (axiosError.response?.status !== 400 && axiosError.response?.status !== 422) {
      return null;
    }

    const data = axiosError.response.data as Record<string, unknown>;
    if (data.errors && typeof data.errors === 'object') {
      return data.errors as Record<string, string[]>;
    }

    return null;
  }, []);

  /**
   * Handles error with logging and returns user message
   *
   * Logs error details to console in development and returns
   * user-friendly message.
   */
  const handleError = useCallback(
    (error: unknown, context?: string): string => {
      const message = getErrorMessage(error);

      // Log error details in development
      if (import.meta.env.DEV) {
        console.error(`[API Error${context ? ` - ${context}` : ''}]`, {
          message,
          error,
          validationErrors: getValidationErrors(error),
        });
      }

      return message;
    },
    [getErrorMessage, getValidationErrors]
  );

  /**
   * Checks if error is a specific HTTP status code
   */
  const isErrorStatus = useCallback((error: unknown, status: number): boolean => {
    if (!isAxiosError(error)) {
      return false;
    }

    return (error as AxiosError).response?.status === status;
  }, []);

  /**
   * Checks if error is a network error (no response)
   */
  const isNetworkError = useCallback((error: unknown): boolean => {
    if (!isAxiosError(error)) {
      return false;
    }

    const axiosError = error as AxiosError;
    return !axiosError.response && !!axiosError.request;
  }, []);

  return {
    getErrorMessage,
    getValidationErrors,
    handleError,
    isErrorStatus,
    isNetworkError,
  };
}

/**
 * Type guard for Axios errors
 */
function isAxiosError(error: unknown): error is AxiosError {
  return (error as AxiosError).isAxiosError === true;
}
