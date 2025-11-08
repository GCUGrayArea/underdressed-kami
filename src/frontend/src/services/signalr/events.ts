/**
 * TypeScript interfaces for SignalR server events
 * These match the event payloads sent from the backend
 */

/**
 * Event data sent when a job is assigned to a contractor
 */
export interface JobAssignedEventData {
  jobId: string;
  contractorId: string;
  scheduledStartTime: string;
}

/**
 * Event sent when a job is assigned to a contractor
 */
export interface JobAssignedEvent {
  eventType: 'JobAssigned';
  timestamp: string;
  data: JobAssignedEventData;
}

/**
 * Event data sent when a contractor's schedule is updated
 */
export interface ScheduleUpdatedEventData {
  contractorId: string;
  updatedBy?: string;
}

/**
 * Event sent when a contractor's schedule changes
 */
export interface ScheduleUpdatedEvent {
  eventType: 'ScheduleUpdated';
  timestamp: string;
  data: ScheduleUpdatedEventData;
}

/**
 * Union type of all possible server events
 */
export type ServerEvent = JobAssignedEvent | ScheduleUpdatedEvent;

/**
 * Type-safe event handler signatures
 */
export type JobAssignedHandler = (event: JobAssignedEvent) => void;
export type ScheduleUpdatedHandler = (event: ScheduleUpdatedEvent) => void;
