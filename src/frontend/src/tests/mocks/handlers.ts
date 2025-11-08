import { http, HttpResponse } from 'msw';
import {
  mockContractorList,
  mockContractor,
  mockJobs,
  mockRankedContractors,
  mockJobTypes,
  newContractor,
  newJob,
} from './data';
import type { ContractorListItemDto, JobDto } from '../../types/dto';
import { JobStatus } from '../../types/dto';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

/**
 * In-memory state for testing
 */
let contractors: ContractorListItemDto[] = [...mockContractorList];
let jobs: JobDto[] = [...mockJobs];

/**
 * MSW request handlers for all API endpoints
 */
export const handlers = [
  // GET /api/contractors - List all contractors with optional filtering
  http.get(`${API_BASE_URL}/contractors`, ({ request }) => {
    const url = new URL(request.url);
    const jobTypeId = url.searchParams.get('jobTypeId');
    const minRating = url.searchParams.get('minRating');
    const maxRating = url.searchParams.get('maxRating');
    const isActive = url.searchParams.get('isActive');
    const page = parseInt(url.searchParams.get('page') || '1');
    const pageSize = parseInt(url.searchParams.get('pageSize') || '20');

    let filtered = [...contractors];

    if (jobTypeId) {
      const jobTypeName = mockJobTypes.find((jt) => jt.id === jobTypeId)?.name;
      if (jobTypeName) {
        filtered = filtered.filter((c) => c.jobTypeName === jobTypeName);
      }
    }

    if (minRating) {
      filtered = filtered.filter((c) => c.rating >= parseFloat(minRating));
    }

    if (maxRating) {
      filtered = filtered.filter((c) => c.rating <= parseFloat(maxRating));
    }

    if (isActive !== null && isActive !== undefined) {
      const activeValue = isActive === 'true';
      filtered = filtered.filter((c) => c.isActive === activeValue);
    }

    // Calculate pagination
    const totalCount = filtered.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const items = filtered.slice(startIndex, endIndex);

    return HttpResponse.json({
      items,
      page,
      pageSize,
      totalCount,
      totalPages,
    });
  }),

  // POST /api/contractors - Create new contractor
  http.post(`${API_BASE_URL}/contractors`, async ({ request }) => {
    const body = (await request.json()) as any;

    const newContractorItem: ContractorListItemDto = {
      id: newContractor.id,
      formattedId: newContractor.formattedId,
      name: body.name,
      jobTypeName: body.jobTypeName || 'Tile Installer',
      rating: body.rating || 3.0,
      isActive: true,
      phone: body.phone,
      email: body.email,
    };

    contractors.push(newContractorItem);

    return HttpResponse.json(newContractorItem, { status: 201 });
  }),

  // GET /api/contractors/:id - Get contractor by ID
  http.get(`${API_BASE_URL}/contractors/:id`, ({ params }) => {
    const { id } = params;

    if (id === mockContractor.id) {
      return HttpResponse.json(mockContractor);
    }

    return HttpResponse.json({ error: 'Contractor not found' }, { status: 404 });
  }),

  // PUT /api/contractors/:id - Update contractor
  http.put(`${API_BASE_URL}/contractors/:id`, async ({ params, request }) => {
    const { id } = params;
    const body = (await request.json()) as any;

    const index = contractors.findIndex((c) => c.id === id);
    if (index === -1) {
      return HttpResponse.json({ error: 'Contractor not found' }, { status: 404 });
    }

    contractors[index] = {
      ...contractors[index],
      name: body.name || contractors[index].name,
      rating: body.rating !== undefined ? body.rating : contractors[index].rating,
      phone: body.phone || contractors[index].phone,
      email: body.email || contractors[index].email,
    };

    return HttpResponse.json(contractors[index]);
  }),

  // GET /api/jobs - List all jobs with optional status filtering
  http.get(`${API_BASE_URL}/jobs`, ({ request }) => {
    const url = new URL(request.url);
    const status = url.searchParams.get('status');
    const page = parseInt(url.searchParams.get('page') || '1');
    const pageSize = parseInt(url.searchParams.get('pageSize') || '50');

    let filtered = [...jobs];

    if (status !== null && status !== undefined) {
      const statusValue = parseInt(status);
      filtered = jobs.filter((j) => j.status === statusValue);
    }

    // Calculate pagination
    const totalCount = filtered.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const items = filtered.slice(startIndex, endIndex);

    return HttpResponse.json({
      items,
      page,
      pageSize,
      totalCount,
      totalPages,
    });
  }),

  // POST /api/jobs - Create new job
  http.post(`${API_BASE_URL}/jobs`, async ({ request }) => {
    const body = (await request.json()) as any;

    const job: JobDto = {
      id: newJob.id,
      formattedId: newJob.formattedId,
      jobTypeId: body.jobTypeId,
      jobTypeName: body.jobTypeName || 'Tile Installer',
      customerId: body.customerId,
      customerName: body.customerName,
      latitude: body.latitude,
      longitude: body.longitude,
      desiredDate: body.desiredDate,
      desiredTime: body.desiredTime,
      estimatedDurationHours: body.estimatedDurationHours,
      status: JobStatus.Unassigned,
      createdAt: new Date().toISOString(),
    };

    jobs.push(job);

    return HttpResponse.json(job, { status: 201 });
  }),

  // GET /api/jobs/:id - Get job by ID
  http.get(`${API_BASE_URL}/jobs/:id`, ({ params }) => {
    const { id } = params;
    const job = jobs.find((j) => j.id === id);

    if (!job) {
      return HttpResponse.json({ error: 'Job not found' }, { status: 404 });
    }

    return HttpResponse.json(job);
  }),

  // POST /api/jobs/:id/assign - Assign contractor to job
  http.post(`${API_BASE_URL}/jobs/:id/assign`, async ({ params, request }) => {
    const { id } = params;
    const body = (await request.json()) as any;

    const jobIndex = jobs.findIndex((j) => j.id === id);
    if (jobIndex === -1) {
      return HttpResponse.json({ error: 'Job not found' }, { status: 404 });
    }

    const contractor = contractors.find((c) => c.id === body.contractorId);
    if (!contractor) {
      return HttpResponse.json({ error: 'Contractor not found' }, { status: 400 });
    }

    jobs[jobIndex] = {
      ...jobs[jobIndex],
      status: JobStatus.Assigned,
      assignedContractorId: contractor.id,
      assignedContractorName: contractor.name,
      assignedContractorFormattedId: contractor.formattedId,
      scheduledStartTime: body.scheduledStartTime || new Date().toISOString(),
    };

    return HttpResponse.json(jobs[jobIndex]);
  }),

  // POST /api/recommendations/contractors - Get contractor recommendations
  http.post(`${API_BASE_URL}/recommendations/contractors`, async ({ request }) => {
    const body = (await request.json()) as any;

    // Filter by job type if specified
    let recommendations = [...mockRankedContractors];

    if (body.jobTypeId) {
      const jobTypeName = mockJobTypes.find((jt) => jt.id === body.jobTypeId)?.name;
      if (jobTypeName) {
        recommendations = recommendations.filter((r) => r.jobType === jobTypeName);
      }
    }

    // Return top N (default 5)
    const topN = body.topN || 5;
    return HttpResponse.json(recommendations.slice(0, topN));
  }),
];
