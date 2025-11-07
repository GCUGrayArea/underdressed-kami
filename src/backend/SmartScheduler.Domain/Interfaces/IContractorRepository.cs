using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Domain.Interfaces;

/// <summary>
/// Repository interface for Contractor aggregate root.
/// Defined in Domain layer, implemented in Infrastructure layer.
/// </summary>
public interface IContractorRepository
{
    Task<Contractor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contractor?> GetByFormattedIdAsync(string formattedId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contractor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Contractor>> GetByJobTypeAsync(Guid jobTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contractor>> GetActiveContractorsAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextSequenceNumberAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Contractor contractor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Contractor contractor, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Contractor> contractors, int totalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Contractor> contractors, int totalCount)> SearchAsync(Guid? jobTypeId, decimal? minRating, decimal? maxRating, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeeklySchedule>> GetSchedulesByContractorIdAsync(Guid contractorId, CancellationToken cancellationToken = default);
    Task<JobType?> GetJobTypeByIdAsync(Guid jobTypeId, CancellationToken cancellationToken = default);
}
