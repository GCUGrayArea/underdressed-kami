using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for JobType aggregate root.
/// </summary>
public class JobTypeRepository : IJobTypeRepository
{
    private readonly ApplicationDbContext _context;

    public JobTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.JobTypes
            .FirstOrDefaultAsync(jt => jt.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<JobType>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.JobTypes
            .Where(jt => jt.IsActive)
            .OrderBy(jt => jt.Name)
            .ToListAsync(cancellationToken);
    }
}
