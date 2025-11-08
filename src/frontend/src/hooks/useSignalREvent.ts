import { useEffect } from 'react';
import { useSignalR } from './useSignalR';
import {
  JobAssignedEvent,
  ScheduleUpdatedEvent,
  JobAssignedHandler,
  ScheduleUpdatedHandler,
} from '../services/signalr/events';

/**
 * Event names matching SignalR hub server method names
 */
export type SignalREventName = 'ReceiveJobAssigned' | 'ReceiveScheduleUpdated';

/**
 * Type-safe event handler map
 */
type EventHandlerMap = {
  ReceiveJobAssigned: JobAssignedHandler;
  ReceiveScheduleUpdated: ScheduleUpdatedHandler;
};

/**
 * Hook for subscribing to SignalR events with automatic cleanup
 *
 * Features:
 * - Type-safe event subscription
 * - Automatic subscription on mount
 * - Automatic cleanup on unmount
 * - Handles connection state (waits for connection to be ready)
 *
 * @param eventName - Name of the SignalR event to subscribe to
 * @param handler - Callback function to handle the event
 *
 * @example
 * ```tsx
 * function JobDashboard() {
 *   useSignalREvent('ReceiveJobAssigned', (event) => {
 *     console.log('Job assigned:', event.data.jobId);
 *     // Refetch jobs list or update state
 *   });
 *
 *   return <div>Dashboard content...</div>;
 * }
 * ```
 */
export function useSignalREvent<T extends SignalREventName>(
  eventName: T,
  handler: EventHandlerMap[T]
): void {
  const { connection, connectionStatus } = useSignalR();

  useEffect(() => {
    if (!connection || connectionStatus !== 'connected') {
      return;
    }

    // Register the event handler
    connection.on(eventName, handler);

    console.info(`Subscribed to SignalR event: ${eventName}`);

    // Cleanup: unregister the handler when component unmounts or dependencies change
    return () => {
      connection.off(eventName, handler);
      console.info(`Unsubscribed from SignalR event: ${eventName}`);
    };
  }, [connection, connectionStatus, eventName, handler]);
}

/**
 * Hook for subscribing to JobAssigned events
 *
 * @param handler - Callback to handle JobAssigned events
 *
 * @example
 * ```tsx
 * useJobAssignedEvent((event) => {
 *   queryClient.invalidateQueries(['jobs']);
 * });
 * ```
 */
export function useJobAssignedEvent(handler: JobAssignedHandler): void {
  useSignalREvent('ReceiveJobAssigned', handler);
}

/**
 * Hook for subscribing to ScheduleUpdated events
 *
 * @param handler - Callback to handle ScheduleUpdated events
 *
 * @example
 * ```tsx
 * useScheduleUpdatedEvent((event) => {
 *   queryClient.invalidateQueries(['contractors', event.data.contractorId]);
 * });
 * ```
 */
export function useScheduleUpdatedEvent(handler: ScheduleUpdatedHandler): void {
  useSignalREvent('ReceiveScheduleUpdated', handler);
}
