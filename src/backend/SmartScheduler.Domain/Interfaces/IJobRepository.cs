using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Domain.Interfaces;

/// <summary>
/// Repository interface for Job aggregate root.
/// Defined in Domain layer, implemented in Infrastructure layer.
/// </summary>
public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Job?> GetByFormattedIdAsync(string formattedId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetByStatusAsync(JobStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetByContractorAsync(Guid contractorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetByJobTypeAsync(Guid jobTypeId, CancellationToken cancellationToken = default);
    Task<int> GetNextSequenceNumberAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
