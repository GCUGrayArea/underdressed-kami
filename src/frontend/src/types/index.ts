/**
 * Common TypeScript types for SmartScheduler frontend
 */

export type JobStatus = 'unassigned' | 'assigned' | 'in-progress' | 'completed' | 'cancelled';

export interface Location {
  address: string;
  latitude: number;
  longitude: number;
}

export interface TimeSlot {
  start: string; // ISO 8601 datetime
  end: string; // ISO 8601 datetime
}

export interface Contractor {
  id: string;
  formattedId: string; // e.g., "CTR-001"
  name: string;
  jobTypeId: string;
  rating: number;
  baseLocation: Location;
  isActive: boolean;
}

export interface Job {
  id: string;
  formattedId: string; // e.g., "JOB-001"
  jobTypeId: string;
  desiredDateTime: string; // ISO 8601
  location: Location;
  status: JobStatus;
  estimatedDurationHours: number;
  customerId?: string;
  customerName?: string;
  assignedContractorId?: string;
}

export interface JobType {
  id: string;
  name: string;
  description?: string;
}
