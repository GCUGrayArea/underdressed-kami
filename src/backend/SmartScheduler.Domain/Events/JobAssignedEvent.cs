namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a contractor is assigned to a job.
/// </summary>
public class JobAssignedEvent : DomainEvent
{
    public Guid JobId { get; }
    public string FormattedJobId { get; }
    public Guid ContractorId { get; }
    public DateTime ScheduledStartTime { get; }

    public JobAssignedEvent(
        Guid jobId,
        string formattedJobId,
        Guid contractorId,
        DateTime scheduledStartTime)
    {
        JobId = jobId;
        FormattedJobId = formattedJobId;
        ContractorId = contractorId;
        ScheduledStartTime = scheduledStartTime;
    }
}
