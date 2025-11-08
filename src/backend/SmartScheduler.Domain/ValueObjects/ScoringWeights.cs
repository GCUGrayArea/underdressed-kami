namespace SmartScheduler.Domain.ValueObjects;

/// <summary>
/// Immutable value object representing the weights used in contractor scoring.
/// Weights must sum to 1.0 to ensure normalized scoring.
/// </summary>
public class ScoringWeights
{
    public decimal AvailabilityWeight { get; }
    public decimal RatingWeight { get; }
    public decimal DistanceWeight { get; }

    /// <summary>
    /// Creates scoring weights with validation.
    /// </summary>
    /// <param name="availabilityWeight">Weight for availability score (0-1)</param>
    /// <param name="ratingWeight">Weight for rating score (0-1)</param>
    /// <param name="distanceWeight">Weight for distance score (0-1)</param>
    public ScoringWeights(
        decimal availabilityWeight,
        decimal ratingWeight,
        decimal distanceWeight)
    {
        if (availabilityWeight < 0 || availabilityWeight > 1)
            throw new ArgumentOutOfRangeException(
                nameof(availabilityWeight),
                "Weight must be between 0 and 1");

        if (ratingWeight < 0 || ratingWeight > 1)
            throw new ArgumentOutOfRangeException(
                nameof(ratingWeight),
                "Weight must be between 0 and 1");

        if (distanceWeight < 0 || distanceWeight > 1)
            throw new ArgumentOutOfRangeException(
                nameof(distanceWeight),
                "Weight must be between 0 and 1");

        var sum = availabilityWeight + ratingWeight + distanceWeight;
        if (Math.Abs(sum - 1.0m) > 0.001m)
            throw new ArgumentException(
                $"Weights must sum to 1.0 (current sum: {sum})");

        AvailabilityWeight = availabilityWeight;
        RatingWeight = ratingWeight;
        DistanceWeight = distanceWeight;
    }

    /// <summary>
    /// Creates default scoring weights: 40% availability, 30% rating, 30% distance.
    /// </summary>
    public static ScoringWeights Default()
    {
        return new ScoringWeights(0.4m, 0.3m, 0.3m);
    }

    public override string ToString()
    {
        return $"Availability: {AvailabilityWeight:P0}, " +
               $"Rating: {RatingWeight:P0}, " +
               $"Distance: {DistanceWeight:P0}";
    }
}
