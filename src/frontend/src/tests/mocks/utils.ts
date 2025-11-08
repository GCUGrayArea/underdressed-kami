/**
 * Reset mock data to initial state for tests
 *
 * Note: Since MSW handlers maintain internal state,
 * this function is a placeholder. The actual reset happens
 * via server.resetHandlers() in the test setup.
 */
export function resetMockData() {
  // MSW handles state reset through server.resetHandlers()
  // This is called in afterEach() in setup.ts
}
