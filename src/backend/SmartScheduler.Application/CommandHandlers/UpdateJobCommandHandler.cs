using MediatR;
using SmartScheduler.Application.Commands;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.CommandHandlers;

/// <summary>
/// Handles UpdateJobCommand by updating job details.
/// </summary>
public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, Unit>
{
    private readonly IJobRepository _jobRepository;

    public UpdateJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<Unit> Handle(
        UpdateJobCommand request,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.JobId, cancellationToken);

        if (job == null)
        {
            throw new InvalidOperationException($"Job {request.JobId} not found");
        }

        var newLocation = new Location(request.Latitude, request.Longitude);

        job.UpdateDetails(
            newLocation,
            request.DesiredDate,
            request.EstimatedDurationHours,
            request.DesiredTime);

        await _jobRepository.UpdateAsync(job, cancellationToken);

        return Unit.Value;
    }
}
