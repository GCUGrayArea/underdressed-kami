using SmartScheduler.Domain.Services;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.Infrastructure.ExternalServices;

namespace SmartScheduler.Infrastructure.Services;

/// <summary>
/// Implementation of IDistanceService that delegates to IDistanceCalculator.
/// Bridges Domain layer interface with Infrastructure implementation.
/// </summary>
public class DistanceService : IDistanceService
{
    private readonly IDistanceCalculator _distanceCalculator;

    public DistanceService(IDistanceCalculator distanceCalculator)
    {
        _distanceCalculator = distanceCalculator ??
            throw new ArgumentNullException(nameof(distanceCalculator));
    }

    public async Task<double> CalculateDistanceMilesAsync(
        Location origin,
        Location destination,
        CancellationToken cancellationToken = default)
    {
        var result = await _distanceCalculator.CalculateDistanceAsync(
            origin,
            destination,
            cancellationToken);

        return result.DistanceMiles;
    }
}
