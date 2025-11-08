using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Services;

/// <summary>
/// Domain service for calculating contractor scores based on
/// availability, rating, and distance.
/// </summary>
public interface IScoringService
{
    /// <summary>
    /// Ranks contractors based on weighted scoring algorithm.
    /// Filters out contractors with no availability or excessive distance (>50mi).
    /// </summary>
    /// <param name="contractors">List of contractors to rank</param>
    /// <param name="targetDate">Target date for the job</param>
    /// <param name="targetTime">Desired start time for the job</param>
    /// <param name="jobLocation">Location of the job site</param>
    /// <param name="requiredDurationHours">Estimated job duration in hours</param>
    /// <param name="weights">Scoring weights (optional, uses default if null)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of contractor scores sorted by overall score descending</returns>
    Task<List<ContractorScore>> RankContractorsAsync(
        List<Contractor> contractors,
        DateOnly targetDate,
        TimeOnly targetTime,
        Location jobLocation,
        double requiredDurationHours,
        ScoringWeights? weights = null,
        CancellationToken cancellationToken = default);
}
