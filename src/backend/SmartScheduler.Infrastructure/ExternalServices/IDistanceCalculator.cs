using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.Infrastructure.ExternalServices.Models;

namespace SmartScheduler.Infrastructure.ExternalServices;

/// <summary>
/// Service interface for calculating distance and travel time between locations.
/// Abstracts the implementation details of distance calculation (API, cache, fallback).
/// </summary>
public interface IDistanceCalculator
{
    /// <summary>
    /// Calculates driving distance and estimated travel time between two locations.
    /// Results may come from cache, external API, or fallback calculation.
    /// </summary>
    /// <param name="origin">Starting location with coordinates</param>
    /// <param name="destination">Ending location with coordinates</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>
    /// Distance result containing:
    /// - Distance in miles
    /// - Duration in minutes
    /// - Metadata indicating source (API, cache, or fallback)
    /// </returns>
    Task<DistanceResult> CalculateDistanceAsync(
        Location origin,
        Location destination,
        CancellationToken cancellationToken = default);
}
