using MediatR;
using SmartScheduler.Application.DTOs;
using SmartScheduler.Application.Queries;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.QueryHandlers;

/// <summary>
/// Handles GetJobByIdQuery by retrieving job and mapping to DTO.
/// </summary>
public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobDto?>
{
    private readonly IJobRepository _jobRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly IJobTypeRepository _jobTypeRepository;

    public GetJobByIdQueryHandler(
        IJobRepository jobRepository,
        IContractorRepository contractorRepository,
        IJobTypeRepository jobTypeRepository)
    {
        _jobRepository = jobRepository;
        _contractorRepository = contractorRepository;
        _jobTypeRepository = jobTypeRepository;
    }

    public async Task<JobDto?> Handle(
        GetJobByIdQuery request,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(
            request.JobId,
            cancellationToken);

        if (job == null)
        {
            return null;
        }

        return await MapToDto(job, cancellationToken);
    }

    private async Task<JobDto> MapToDto(
        Domain.Entities.Job job,
        CancellationToken cancellationToken)
    {
        var jobType = await _jobTypeRepository.GetByIdAsync(
            job.JobTypeId,
            cancellationToken);

        var dto = new JobDto
        {
            Id = job.Id,
            FormattedId = job.FormattedId,
            JobTypeId = job.JobTypeId,
            JobTypeName = jobType?.Name ?? string.Empty,
            CustomerId = job.CustomerId,
            CustomerName = job.CustomerName,
            Latitude = job.Location.Latitude,
            Longitude = job.Location.Longitude,
            DesiredDate = job.DesiredDate,
            DesiredTime = job.DesiredTime,
            EstimatedDurationHours = job.EstimatedDurationHours,
            Status = job.Status,
            AssignedContractorId = job.AssignedContractorId,
            ScheduledStartTime = job.ScheduledStartTime,
            CreatedAt = job.CreatedAt,
            CompletedAt = job.CompletedAt
        };

        if (job.AssignedContractorId.HasValue)
        {
            var contractor = await _contractorRepository.GetByIdAsync(
                job.AssignedContractorId.Value,
                cancellationToken);

            if (contractor != null)
            {
                dto.AssignedContractorName = contractor.Name;
                dto.AssignedContractorFormattedId = contractor.FormattedId;
            }
        }

        return dto;
    }
}
