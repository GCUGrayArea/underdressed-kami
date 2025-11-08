/**
 * Backend DTO Types
 *
 * TypeScript interfaces matching backend C# DTOs for type-safe API communication.
 * These types mirror the SmartScheduler.Application.DTOs namespace.
 */

/**
 * Job status enum matching backend JobStatus
 */
export const JobStatus = {
  Unassigned: 0,
  Assigned: 1,
  InProgress: 2,
  Completed: 3,
  Cancelled: 4,
} as const;

export type JobStatus = typeof JobStatus[keyof typeof JobStatus];

/**
 * Geographic location data
 */
export interface LocationDto {
  latitude: number;
  longitude: number;
  address?: string;
}

/**
 * Working hours schedule entry for a specific day
 */
export interface WeeklyScheduleDto {
  id: string;
  dayOfWeek: number; // 0=Sunday, 1=Monday, ..., 6=Saturday
  startTime: string; // TimeOnly format: "HH:mm:ss"
  endTime: string; // TimeOnly format: "HH:mm:ss"
  durationHours: number;
}

/**
 * Complete contractor details with location and schedule
 */
export interface ContractorDto {
  id: string;
  formattedId: string; // e.g., "CTR-001"
  name: string;
  jobTypeId: string;
  jobTypeName: string;
  rating: number; // 0.0-5.0
  baseLocation: LocationDto;
  phone?: string;
  email?: string;
  isActive: boolean;
  weeklySchedule: WeeklyScheduleDto[];
}

/**
 * Lightweight contractor list item for list displays
 */
export interface ContractorListItemDto {
  id: string;
  formattedId: string; // e.g., "CTR-001"
  name: string;
  jobTypeName: string;
  rating: number; // 0.0-5.0
  isActive: boolean;
  phone?: string;
  email?: string;
}

/**
 * Job data transfer object
 */
export interface JobDto {
  id: string;
  formattedId: string; // e.g., "JOB-001"
  jobTypeId: string;
  jobTypeName: string;
  customerId: string;
  customerName: string;
  latitude: number;
  longitude: number;
  desiredDate: string; // ISO 8601 date string
  desiredTime?: string; // TimeOnly format: "HH:mm:ss"
  estimatedDurationHours: number;
  status: JobStatus;
  assignedContractorId?: string;
  assignedContractorName?: string;
  assignedContractorFormattedId?: string;
  scheduledStartTime?: string; // ISO 8601 datetime string
  createdAt: string; // ISO 8601 datetime string
  completedAt?: string; // ISO 8601 datetime string
}

/**
 * Available time slot
 */
export interface TimeSlotDto {
  start: string; // TimeOnly format: "HH:mm:ss"
  end: string; // TimeOnly format: "HH:mm:ss"
  durationHours: number;
}

/**
 * Contractor availability for a specific date
 */
export interface AvailabilityDto {
  contractorId: string;
  contractorFormattedId: string;
  contractorName: string;
  targetDate: string; // ISO 8601 date string
  availableSlots: TimeSlotDto[];
}

/**
 * Job type definition
 */
export interface JobTypeDto {
  id: string;
  name: string;
  description?: string;
}

/**
 * Score breakdown for contractor ranking
 */
export interface ScoreBreakdownDto {
  availabilityScore: number; // 0.0-1.0
  ratingScore: number; // 0.0-1.0
  distanceScore: number; // 0.0-1.0
  overallScore: number; // 0.0-1.0
}

/**
 * Ranked contractor recommendation with score and availability
 */
export interface RankedContractorDto {
  contractorId: string;
  formattedId: string; // e.g., "CTR-001"
  name: string;
  jobType: string;
  rating: number; // 0.0-5.0
  baseLocation: LocationDto;
  distanceMiles: number;
  bestAvailableSlot?: TimeSlotDto;
  scoreBreakdown: ScoreBreakdownDto;
}
