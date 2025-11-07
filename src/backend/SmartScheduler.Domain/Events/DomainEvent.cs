namespace SmartScheduler.Domain.Events;

/// <summary>
/// Base class for all domain events in the system.
/// Domain events capture significant business occurrences and are published via MediatR.
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// UTC timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Unique identifier for this event instance.
    /// </summary>
    public Guid EventId { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
    }
}
