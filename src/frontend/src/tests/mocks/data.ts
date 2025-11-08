import type {
  ContractorDto,
  ContractorListItemDto,
  JobDto,
  JobTypeDto,
  RankedContractorDto,
  TimeSlotDto,
  ScoreBreakdownDto,
} from '../../types/dto';
import { JobStatus } from '../../types/dto';

/**
 * Mock job types
 */
export const mockJobTypes: JobTypeDto[] = [
  { id: '1', name: 'Tile Installer', description: 'Professional tile installation' },
  { id: '2', name: 'Carpet Installer', description: 'Carpet laying and fitting' },
  { id: '3', name: 'Hardwood Specialist', description: 'Hardwood floor installation' },
];

/**
 * Mock contractors for list display
 */
export const mockContractorList: ContractorListItemDto[] = [
  {
    id: 'ctr-1',
    formattedId: 'CTR-001',
    name: 'John Smith',
    jobTypeName: 'Tile Installer',
    rating: 4.8,
    isActive: true,
    phone: '555-0101',
    email: 'john.smith@example.com',
  },
  {
    id: 'ctr-2',
    formattedId: 'CTR-002',
    name: 'Jane Doe',
    jobTypeName: 'Carpet Installer',
    rating: 4.5,
    isActive: true,
    phone: '555-0102',
    email: 'jane.doe@example.com',
  },
  {
    id: 'ctr-3',
    formattedId: 'CTR-003',
    name: 'Bob Wilson',
    jobTypeName: 'Tile Installer',
    rating: 4.2,
    isActive: true,
    phone: '555-0103',
    email: 'bob.wilson@example.com',
  },
  {
    id: 'ctr-4',
    formattedId: 'CTR-004',
    name: 'Alice Johnson',
    jobTypeName: 'Hardwood Specialist',
    rating: 4.9,
    isActive: true,
    phone: '555-0104',
    email: 'alice.johnson@example.com',
  },
  {
    id: 'ctr-5',
    formattedId: 'CTR-005',
    name: 'Charlie Brown',
    jobTypeName: 'Tile Installer',
    rating: 3.8,
    isActive: false,
    phone: '555-0105',
    email: 'charlie.brown@example.com',
  },
];

/**
 * Mock contractor detail
 */
export const mockContractor: ContractorDto = {
  id: 'ctr-1',
  formattedId: 'CTR-001',
  name: 'John Smith',
  jobTypeId: '1',
  jobTypeName: 'Tile Installer',
  rating: 4.8,
  baseLocation: {
    latitude: 40.7128,
    longitude: -74.006,
    address: '123 Main St, New York, NY 10001',
  },
  phone: '555-0101',
  email: 'john.smith@example.com',
  isActive: true,
  weeklySchedule: [
    {
      id: 'sched-1',
      dayOfWeek: 1,
      startTime: '09:00:00',
      endTime: '17:00:00',
      durationHours: 8,
    },
    {
      id: 'sched-2',
      dayOfWeek: 2,
      startTime: '09:00:00',
      endTime: '17:00:00',
      durationHours: 8,
    },
    {
      id: 'sched-3',
      dayOfWeek: 3,
      startTime: '09:00:00',
      endTime: '17:00:00',
      durationHours: 8,
    },
    {
      id: 'sched-4',
      dayOfWeek: 4,
      startTime: '09:00:00',
      endTime: '17:00:00',
      durationHours: 8,
    },
    {
      id: 'sched-5',
      dayOfWeek: 5,
      startTime: '09:00:00',
      endTime: '17:00:00',
      durationHours: 8,
    },
  ],
};

/**
 * Mock jobs
 */
export const mockJobs: JobDto[] = [
  {
    id: 'job-1',
    formattedId: 'JOB-001',
    jobTypeId: '1',
    jobTypeName: 'Tile Installer',
    customerId: 'cust-1',
    customerName: 'ABC Company',
    latitude: 40.7589,
    longitude: -73.9851,
    desiredDate: '2025-12-15',
    desiredTime: '10:00:00',
    estimatedDurationHours: 4,
    status: JobStatus.Unassigned,
    createdAt: '2025-11-08T10:00:00Z',
  },
  {
    id: 'job-2',
    formattedId: 'JOB-002',
    jobTypeId: '2',
    jobTypeName: 'Carpet Installer',
    customerId: 'cust-2',
    customerName: 'XYZ Corp',
    latitude: 40.7484,
    longitude: -73.9857,
    desiredDate: '2025-12-16',
    desiredTime: '14:00:00',
    estimatedDurationHours: 6,
    status: JobStatus.Unassigned,
    createdAt: '2025-11-08T11:00:00Z',
  },
  {
    id: 'job-3',
    formattedId: 'JOB-003',
    jobTypeId: '1',
    jobTypeName: 'Tile Installer',
    customerId: 'cust-3',
    customerName: 'Test Client',
    latitude: 40.7306,
    longitude: -73.9352,
    desiredDate: '2025-12-17',
    desiredTime: '09:00:00',
    estimatedDurationHours: 5,
    status: JobStatus.Assigned,
    assignedContractorId: 'ctr-1',
    assignedContractorName: 'John Smith',
    assignedContractorFormattedId: 'CTR-001',
    scheduledStartTime: '2025-12-17T09:00:00Z',
    createdAt: '2025-11-08T09:00:00Z',
  },
];

/**
 * Mock time slots
 */
export const mockTimeSlots: TimeSlotDto[] = [
  {
    start: '09:00:00',
    end: '13:00:00',
    durationHours: 4,
  },
  {
    start: '14:00:00',
    end: '17:00:00',
    durationHours: 3,
  },
];

/**
 * Mock score breakdown
 */
export const mockScoreBreakdown: ScoreBreakdownDto = {
  availabilityScore: 1.0,
  ratingScore: 0.96,
  distanceScore: 0.85,
  overallScore: 0.92,
};

/**
 * Mock ranked contractors for recommendations
 */
export const mockRankedContractors: RankedContractorDto[] = [
  {
    contractorId: 'ctr-1',
    formattedId: 'CTR-001',
    name: 'John Smith',
    jobType: 'Tile Installer',
    rating: 4.8,
    baseLocation: {
      latitude: 40.7128,
      longitude: -74.006,
      address: '123 Main St, New York, NY 10001',
    },
    distanceMiles: 8.5,
    bestAvailableSlot: {
      start: '09:00:00',
      end: '13:00:00',
      durationHours: 4,
    },
    scoreBreakdown: {
      availabilityScore: 1.0,
      ratingScore: 0.96,
      distanceScore: 0.85,
      overallScore: 0.92,
    },
  },
  {
    contractorId: 'ctr-3',
    formattedId: 'CTR-003',
    name: 'Bob Wilson',
    jobType: 'Tile Installer',
    rating: 4.2,
    baseLocation: {
      latitude: 40.7306,
      longitude: -73.9352,
      address: '456 Oak Ave, Brooklyn, NY 11201',
    },
    distanceMiles: 12.3,
    bestAvailableSlot: {
      start: '10:00:00',
      end: '14:00:00',
      durationHours: 4,
    },
    scoreBreakdown: {
      availabilityScore: 0.7,
      ratingScore: 0.84,
      distanceScore: 0.75,
      overallScore: 0.75,
    },
  },
];

/**
 * Create a new contractor (used in create tests)
 */
export const newContractor: ContractorDto = {
  id: 'ctr-new',
  formattedId: 'CTR-999',
  name: 'New Contractor',
  jobTypeId: '1',
  jobTypeName: 'Tile Installer',
  rating: 3.0,
  baseLocation: {
    latitude: 40.7489,
    longitude: -73.9680,
    address: '789 New St, New York, NY 10002',
  },
  phone: '555-9999',
  email: 'new.contractor@example.com',
  isActive: true,
  weeklySchedule: [
    {
      id: 'sched-new',
      dayOfWeek: 1,
      startTime: '08:00:00',
      endTime: '16:00:00',
      durationHours: 8,
    },
  ],
};

/**
 * Create a new job (used in create tests)
 */
export const newJob: JobDto = {
  id: 'job-new',
  formattedId: 'JOB-999',
  jobTypeId: '1',
  jobTypeName: 'Tile Installer',
  customerId: 'cust-new',
  customerName: 'New Customer',
  latitude: 40.7580,
  longitude: -73.9855,
  desiredDate: '2025-12-20',
  desiredTime: '11:00:00',
  estimatedDurationHours: 3,
  status: JobStatus.Unassigned,
  createdAt: new Date().toISOString(),
};
