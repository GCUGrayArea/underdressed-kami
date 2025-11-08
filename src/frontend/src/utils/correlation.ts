/**
 * Generates a unique correlation ID for request tracing
 *
 * Format: timestamp-random
 * Example: 1704902400000-a3f2d8c9
 *
 * Correlation IDs help trace requests across frontend and backend logs
 * for debugging and monitoring purposes.
 */
export function generateCorrelationId(): string {
  const timestamp = Date.now();
  const random = Math.random().toString(36).substring(2, 10);
  return `${timestamp}-${random}`;
}

/**
 * Gets or creates a correlation ID for the current request context
 *
 * Returns existing ID from storage if available, otherwise generates new one.
 * Useful for grouping related requests (e.g., batch operations).
 */
export function getOrCreateCorrelationId(key = 'current'): string {
  const storageKey = `correlation-${key}`;
  const existing = sessionStorage.getItem(storageKey);

  if (existing) {
    return existing;
  }

  const newId = generateCorrelationId();
  sessionStorage.setItem(storageKey, newId);
  return newId;
}

/**
 * Clears stored correlation ID
 */
export function clearCorrelationId(key = 'current'): void {
  const storageKey = `correlation-${key}`;
  sessionStorage.removeItem(storageKey);
}
