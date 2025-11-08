namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a contractor's working hours schedule is updated.
/// Signals that availability calculations may need to be recalculated.
/// </summary>
public class ScheduleUpdatedEvent : DomainEvent
{
    /// <summary>
    /// ID of the contractor whose schedule was updated.
    /// </summary>
    public Guid ContractorId { get; }

    /// <summary>
    /// Optional description of the schedule change.
    /// </summary>
    public string? ChangeDescription { get; }

    public ScheduleUpdatedEvent(Guid contractorId, string? changeDescription = null)
    {
        ContractorId = contractorId;
        ChangeDescription = changeDescription;
    }
}
