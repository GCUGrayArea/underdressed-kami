using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Services;

/// <summary>
/// Domain service interface for calculating distance between locations.
/// Implementation in Infrastructure layer delegates to IDistanceCalculator.
/// </summary>
public interface IDistanceService
{
    /// <summary>
    /// Calculates distance in miles between two locations.
    /// </summary>
    Task<double> CalculateDistanceMilesAsync(
        Location origin,
        Location destination,
        CancellationToken cancellationToken = default);
}
