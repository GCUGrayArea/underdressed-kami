using MediatR;
using SmartScheduler.Application.Commands;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Events;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.CommandHandlers;

/// <summary>
/// Handles CreateJobCommand by creating a new job and publishing JobCreatedEvent.
/// </summary>
public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Guid>
{
    private readonly IJobRepository _jobRepository;
    private readonly IMediator _mediator;

    public CreateJobCommandHandler(
        IJobRepository jobRepository,
        IMediator mediator)
    {
        _jobRepository = jobRepository;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(
        CreateJobCommand request,
        CancellationToken cancellationToken)
    {
        var sequenceNumber = await _jobRepository.GetNextSequenceNumberAsync(
            cancellationToken);

        var formattedId = $"JOB-{sequenceNumber:D3}";
        var location = new Location(request.Latitude, request.Longitude);

        var job = new Job(
            formattedId,
            request.JobTypeId,
            request.CustomerId,
            request.CustomerName,
            location,
            request.DesiredDate,
            request.EstimatedDurationHours,
            request.DesiredTime);

        await _jobRepository.AddAsync(job, cancellationToken);

        var jobCreatedEvent = new JobCreatedEvent(
            job.Id,
            job.FormattedId,
            job.JobTypeId,
            job.DesiredDate);

        await _mediator.Publish(jobCreatedEvent, cancellationToken);

        return job.Id;
    }
}
