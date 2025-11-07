using MediatR;

namespace SmartScheduler.Application.Commands;

/// <summary>
/// Command to assign a contractor to a job.
/// Validates contractor exists, job type matches, and publishes JobAssignedEvent.
/// </summary>
public class AssignContractorCommand : IRequest<Unit>
{
    public Guid JobId { get; set; }
    public Guid ContractorId { get; set; }
    public DateTime ScheduledStartTime { get; set; }
}
