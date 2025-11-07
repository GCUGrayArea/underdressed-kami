using MediatR;
using SmartScheduler.Application.Commands;
using SmartScheduler.Domain.Events;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.CommandHandlers;

/// <summary>
/// Handles AssignContractorCommand by assigning contractor to job.
/// Validates contractor exists and job type matches before assignment.
/// </summary>
public class AssignContractorCommandHandler : IRequestHandler<AssignContractorCommand, Unit>
{
    private readonly IJobRepository _jobRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly IMediator _mediator;

    public AssignContractorCommandHandler(
        IJobRepository jobRepository,
        IContractorRepository contractorRepository,
        IMediator mediator)
    {
        _jobRepository = jobRepository;
        _contractorRepository = contractorRepository;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(
        AssignContractorCommand request,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(
            request.JobId,
            cancellationToken);

        if (job == null)
        {
            throw new InvalidOperationException(
                $"Job {request.JobId} not found");
        }

        var contractor = await _contractorRepository.GetByIdAsync(
            request.ContractorId,
            cancellationToken);

        if (contractor == null)
        {
            throw new InvalidOperationException(
                $"Contractor {request.ContractorId} not found");
        }

        if (job.JobTypeId != contractor.JobTypeId)
        {
            throw new InvalidOperationException(
                "Contractor job type does not match job requirements");
        }

        job.AssignToContractor(
            request.ContractorId,
            request.ScheduledStartTime);

        await _jobRepository.UpdateAsync(job, cancellationToken);

        var jobAssignedEvent = new JobAssignedEvent(
            job.Id,
            job.FormattedId,
            contractor.Id,
            request.ScheduledStartTime);

        await _mediator.Publish(jobAssignedEvent, cancellationToken);

        return Unit.Value;
    }
}
