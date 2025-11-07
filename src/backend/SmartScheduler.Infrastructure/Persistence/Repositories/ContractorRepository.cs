using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Infrastructure.Persistence.Repositories;

/// <summary>
/// COMPLETE implementation of IContractorRepository with ALL CRUD methods.
/// Provides data access for Contractor entities using EF Core.
/// </summary>
public class ContractorRepository : IContractorRepository
{
    private readonly ApplicationDbContext _context;

    public ContractorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a contractor by their unique GUID identifier.
    /// </summary>
    public async Task<Contractor?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contractors
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a contractor by their formatted ID (e.g., "CTR-001").
    /// </summary>
    public async Task<Contractor?> GetByFormattedIdAsync(
        string formattedId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contractors
            .FirstOrDefaultAsync(
                c => c.FormattedId == formattedId,
                cancellationToken);
    }

    /// <summary>
    /// Retrieves all contractors from the database.
    /// </summary>
    public async Task<IEnumerable<Contractor>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Contractors
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all contractors that match a specific job type.
    /// </summary>
    public async Task<IEnumerable<Contractor>> GetByJobTypeAsync(
        Guid jobTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contractors
            .Where(c => c.JobTypeId == jobTypeId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all active contractors (IsActive = true).
    /// </summary>
    public async Task<IEnumerable<Contractor>> GetActiveContractorsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Contractors
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the next sequence number for formatted ID generation.
    /// Finds the highest existing sequence number and increments by 1.
    /// </summary>
    public async Task<int> GetNextSequenceNumberAsync(
        CancellationToken cancellationToken = default)
    {
        var contractors = await _context.Contractors
            .Select(c => c.FormattedId)
            .ToListAsync(cancellationToken);

        if (!contractors.Any())
            return 1;

        var maxSequence = contractors
            .Select(ExtractSequenceNumber)
            .Where(n => n.HasValue)
            .Max(n => n!.Value);

        return maxSequence + 1;
    }

    /// <summary>
    /// Adds a new contractor to the database.
    /// </summary>
    public async Task AddAsync(
        Contractor contractor,
        CancellationToken cancellationToken = default)
    {
        await _context.Contractors.AddAsync(contractor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing contractor in the database.
    /// </summary>
    public async Task UpdateAsync(
        Contractor contractor,
        CancellationToken cancellationToken = default)
    {
        _context.Contractors.Update(contractor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a contractor with the specified ID exists.
    /// </summary>
    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contractors
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves contractors with pagination support.
    /// </summary>
    public async Task<(IEnumerable<Contractor> contractors, int totalCount)>
        GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        var query = _context.Contractors.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var contractors = await query
            .OrderBy(c => c.FormattedId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (contractors, totalCount);
    }

    /// <summary>
    /// Searches contractors with advanced filtering and pagination.
    /// </summary>
    public async Task<(IEnumerable<Contractor> contractors, int totalCount)>
        SearchAsync(
            Guid? jobTypeId,
            decimal? minRating,
            decimal? maxRating,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        var query = _context.Contractors.AsQueryable();

        if (jobTypeId.HasValue)
            query = query.Where(c => c.JobTypeId == jobTypeId.Value);

        if (minRating.HasValue)
            query = query.Where(c => c.Rating >= minRating.Value);

        if (maxRating.HasValue)
            query = query.Where(c => c.Rating <= maxRating.Value);

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var contractors = await query
            .OrderBy(c => c.FormattedId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (contractors, totalCount);
    }

    /// <summary>
    /// Retrieves all weekly schedules for a specific contractor.
    /// </summary>
    public async Task<IEnumerable<WeeklySchedule>>
        GetSchedulesByContractorIdAsync(
            Guid contractorId,
            CancellationToken cancellationToken = default)
    {
        return await _context.WeeklySchedules
            .Where(s => s.ContractorId == contractorId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a job type by its unique identifier.
    /// </summary>
    public async Task<JobType?> GetJobTypeByIdAsync(
        Guid jobTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.JobTypes
            .FirstOrDefaultAsync(jt => jt.Id == jobTypeId, cancellationToken);
    }

    /// <summary>
    /// Extracts the numeric sequence from a formatted ID (e.g., "CTR-001" → 1).
    /// Returns null if the format is invalid.
    /// </summary>
    private static int? ExtractSequenceNumber(string formattedId)
    {
        var parts = formattedId.Split('-');
        if (parts.Length != 2)
            return null;

        return int.TryParse(parts[1], out var number) ? number : null;
    }
}
