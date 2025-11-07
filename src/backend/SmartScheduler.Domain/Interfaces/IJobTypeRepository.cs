using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Domain.Interfaces;

/// <summary>
/// Repository interface for JobType aggregate root.
/// Defined in Domain layer, implemented in Infrastructure layer.
/// </summary>
public interface IJobTypeRepository
{
    Task<JobType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<JobType>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
