import { useContext } from 'react';
import { SignalRContext, SignalRContextValue } from '../contexts/SignalRContext';

/**
 * Hook for accessing the SignalR connection and state
 *
 * @returns SignalR context value with connection, status, and error
 * @throws Error if used outside SignalRProvider
 *
 * @example
 * ```tsx
 * function MyComponent() {
 *   const { connection, connectionStatus, error } = useSignalR();
 *
 *   if (connectionStatus === 'connected') {
 *     // Connection is ready to use
 *   }
 * }
 * ```
 */
export function useSignalR(): SignalRContextValue {
  const context = useContext(SignalRContext);

  if (!context) {
    throw new Error('useSignalR must be used within a SignalRProvider');
  }

  return context;
}
