namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a new job is created.
/// </summary>
public class JobCreatedEvent : DomainEvent
{
    public Guid JobId { get; }
    public string FormattedJobId { get; }
    public Guid JobTypeId { get; }
    public DateTime DesiredDate { get; }

    public JobCreatedEvent(
        Guid jobId,
        string formattedJobId,
        Guid jobTypeId,
        DateTime desiredDate)
    {
        JobId = jobId;
        FormattedJobId = formattedJobId;
        JobTypeId = jobTypeId;
        DesiredDate = desiredDate;
    }
}
