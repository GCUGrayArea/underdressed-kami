namespace SmartScheduler.Domain.Entities;

/// <summary>
/// Entity for persisting domain events to create an audit trail.
/// Every domain event published in the system is logged here.
/// </summary>
public class DomainEventLog
{
    /// <summary>
    /// Primary key for the event log entry.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Unique identifier of the original domain event.
    /// </summary>
    public Guid EventId { get; private set; }

    /// <summary>
    /// Type name of the domain event (e.g., "JobAssignedEvent").
    /// </summary>
    public string EventType { get; private set; } = string.Empty;

    /// <summary>
    /// Serialized JSON payload of the event data.
    /// </summary>
    public string EventData { get; private set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// UTC timestamp when this log entry was created.
    /// </summary>
    public DateTime LoggedAt { get; private set; }

    /// <summary>
    /// Optional user identifier who triggered the event.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private DomainEventLog()
    {
    }

    /// <summary>
    /// Creates a new domain event log entry.
    /// </summary>
    public DomainEventLog(
        Guid eventId,
        string eventType,
        string eventData,
        DateTime occurredAt,
        string? userId = null)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        EventType = eventType;
        EventData = eventData;
        OccurredAt = occurredAt;
        LoggedAt = DateTime.UtcNow;
        UserId = userId;
    }
}
