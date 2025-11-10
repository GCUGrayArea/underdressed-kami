using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Job aggregate root.
/// Handles persistence operations for jobs.
/// </summary>
public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<Job?> GetByFormattedIdAsync(
        string formattedId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .FirstOrDefaultAsync(
                j => j.FormattedId == formattedId,
                cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetByStatusAsync(
        JobStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .Where(j => j.Status == status)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetByContractorAsync(
        Guid contractorId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .Where(j => j.AssignedContractorId == contractorId)
            .OrderBy(j => j.ScheduledStartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetByJobTypeAsync(
        Guid jobTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .Where(j => j.JobTypeId == jobTypeId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextSequenceNumberAsync(
        CancellationToken cancellationToken = default)
    {
        var maxFormattedId = await _context.Jobs
            .Select(j => j.FormattedId)
            .OrderByDescending(f => f)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(maxFormattedId))
        {
            return 1;
        }

        var parts = maxFormattedId.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[1], out var number))
        {
            return number + 1;
        }

        return 1;
    }

    public async Task<IEnumerable<Job>> GetByContractorAndDateAsync(
        Guid contractorId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        // Ensure targetDate is UTC to satisfy PostgreSQL timestamp with time zone
        var targetDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

        return await _context.Jobs
            .Where(j => j.AssignedContractorId == contractorId)
            .Where(j => j.ScheduledStartTime.HasValue)
            .Where(j => j.ScheduledStartTime!.Value.Date == targetDate)
            .OrderBy(j => j.ScheduledStartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Job job,
        CancellationToken cancellationToken = default)
    {
        await _context.Jobs.AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Job job,
        CancellationToken cancellationToken = default)
    {
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .AnyAsync(j => j.Id == id, cancellationToken);
    }
}
