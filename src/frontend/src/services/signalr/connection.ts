import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { SignalRLogger } from '../../utils/signalr-logger';

/**
 * Creates and configures a SignalR HubConnection
 *
 * Features:
 * - Automatic reconnection on disconnect
 * - Custom logging for debugging
 * - Configurable retry delays
 *
 * @returns Configured HubConnection instance (not started)
 */
export function createSignalRConnection(): HubConnection {
  const hubUrl = import.meta.env.VITE_SIGNALR_HUB_URL;

  if (!hubUrl) {
    throw new Error(
      'VITE_SIGNALR_HUB_URL environment variable is not defined. ' +
      'Please check your .env file.'
    );
  }

  const connection = new HubConnectionBuilder()
    .withUrl(hubUrl)
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        // Exponential backoff: 0s, 2s, 10s, 30s, then 30s for all subsequent retries
        const delays = [0, 2000, 10000, 30000];
        return delays[retryContext.previousRetryCount] ?? 30000;
      },
    })
    .configureLogging(new SignalRLogger())
    .build();

  return connection;
}

/**
 * Gets a human-readable string for the connection state
 */
export function getConnectionStateLabel(state: HubConnectionState): string {
  switch (state) {
    case HubConnectionState.Connecting:
      return 'Connecting';
    case HubConnectionState.Connected:
      return 'Connected';
    case HubConnectionState.Reconnecting:
      return 'Reconnecting';
    case HubConnectionState.Disconnected:
      return 'Disconnected';
    default:
      return 'Unknown';
  }
}
