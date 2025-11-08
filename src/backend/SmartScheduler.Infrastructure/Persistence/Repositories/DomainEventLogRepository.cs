using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for persisting domain event audit logs.
/// </summary>
public class DomainEventLogRepository : IDomainEventLogRepository
{
    private readonly ApplicationDbContext _context;

    public DomainEventLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DomainEventLog eventLog, CancellationToken cancellationToken = default)
    {
        await _context.DomainEventLogs.AddAsync(eventLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<DomainEventLog>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.DomainEventLogs
            .Where(e => e.OccurredAt >= startDate && e.OccurredAt <= endDate)
            .OrderBy(e => e.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DomainEventLog>> GetByEventTypeAsync(
        string eventType,
        CancellationToken cancellationToken = default)
    {
        return await _context.DomainEventLogs
            .Where(e => e.EventType == eventType)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DomainEventLog>> GetRecentAsync(
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return await _context.DomainEventLogs
            .OrderByDescending(e => e.OccurredAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
