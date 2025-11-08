using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Domain.Interfaces;

/// <summary>
/// Repository interface for persisting domain event audit logs.
/// </summary>
public interface IDomainEventLogRepository
{
    /// <summary>
    /// Adds a new domain event log entry.
    /// </summary>
    Task AddAsync(DomainEventLog eventLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves event logs within a date range.
    /// </summary>
    Task<List<DomainEventLog>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all event logs of a specific type.
    /// </summary>
    Task<List<DomainEventLog>> GetByEventTypeAsync(
        string eventType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent N event logs.
    /// </summary>
    Task<List<DomainEventLog>> GetRecentAsync(
        int count = 100,
        CancellationToken cancellationToken = default);
}
