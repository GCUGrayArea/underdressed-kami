import {
  createContext,
  useEffect,
  useState,
  useCallback,
  ReactNode,
} from 'react';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import { createSignalRConnection } from '../services/signalr/connection';

/**
 * Connection state for the SignalR context
 */
export type ConnectionStatus = 'connecting' | 'connected' | 'disconnected' | 'reconnecting';

/**
 * SignalR context value providing connection access and state
 */
export interface SignalRContextValue {
  connection: HubConnection | null;
  connectionStatus: ConnectionStatus;
  error: Error | null;
}

/**
 * Context for sharing SignalR connection across the application
 */
export const SignalRContext = createContext<SignalRContextValue>({
  connection: null,
  connectionStatus: 'disconnected',
  error: null,
});

interface SignalRProviderProps {
  children: ReactNode;
}

/**
 * Provider component that manages the SignalR connection lifecycle
 *
 * Features:
 * - Singleton connection shared across all components
 * - Automatic connection on mount
 * - Automatic cleanup on unmount
 * - Connection state tracking
 * - Error handling
 */
export function SignalRProvider({ children }: SignalRProviderProps) {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>('disconnected');
  const [error, setError] = useState<Error | null>(null);

  const mapHubStateToStatus = useCallback((state: HubConnectionState): ConnectionStatus => {
    switch (state) {
      case HubConnectionState.Connecting:
        return 'connecting';
      case HubConnectionState.Connected:
        return 'connected';
      case HubConnectionState.Reconnecting:
        return 'reconnecting';
      case HubConnectionState.Disconnected:
        return 'disconnected';
      default:
        return 'disconnected';
    }
  }, []);

  useEffect(() => {
    let hubConnection: HubConnection | null = null;

    async function startConnection() {
      try {
        hubConnection = createSignalRConnection();
        setConnection(hubConnection);

        // Set up event handlers for connection state changes
        hubConnection.onclose((err) => {
          setConnectionStatus('disconnected');
          if (err) {
            setError(err);
            console.error('SignalR connection closed with error:', err);
          }
        });

        hubConnection.onreconnecting((err) => {
          setConnectionStatus('reconnecting');
          if (err) {
            console.warn('SignalR reconnecting due to error:', err);
          }
        });

        hubConnection.onreconnected(() => {
          setConnectionStatus('connected');
          setError(null);
          console.info('SignalR reconnected successfully');
        });

        // Start the connection
        setConnectionStatus('connecting');
        await hubConnection.start();
        setConnectionStatus(mapHubStateToStatus(hubConnection.state));
        setError(null);

        console.info('SignalR connection established');
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Unknown SignalR error');
        setError(error);
        setConnectionStatus('disconnected');
        console.error('Failed to start SignalR connection:', error);
      }
    }

    startConnection();

    // Cleanup on unmount
    return () => {
      if (hubConnection) {
        hubConnection
          .stop()
          .then(() => console.info('SignalR connection stopped'))
          .catch((err) => console.error('Error stopping SignalR connection:', err));
      }
    };
  }, [mapHubStateToStatus]);

  const contextValue: SignalRContextValue = {
    connection,
    connectionStatus,
    error,
  };

  return (
    <SignalRContext.Provider value={contextValue}>
      {children}
    </SignalRContext.Provider>
  );
}
