namespace SmartScheduler.Domain.ValueObjects;

/// <summary>
/// Immutable value object representing a contractor's calculated score
/// with breakdown of component scores.
/// </summary>
public class ContractorScore : IComparable<ContractorScore>
{
    public Guid ContractorId { get; }
    public decimal OverallScore { get; }
    public decimal AvailabilityScore { get; }
    public decimal RatingScore { get; }
    public decimal DistanceScore { get; }
    public TimeSlot? BestAvailableSlot { get; }

    public ContractorScore(
        Guid contractorId,
        decimal overallScore,
        decimal availabilityScore,
        decimal ratingScore,
        decimal distanceScore,
        TimeSlot? bestAvailableSlot = null)
    {
        if (overallScore < 0 || overallScore > 1)
            throw new ArgumentOutOfRangeException(
                nameof(overallScore),
                "Overall score must be between 0 and 1");

        if (availabilityScore < 0 || availabilityScore > 1)
            throw new ArgumentOutOfRangeException(
                nameof(availabilityScore),
                "Availability score must be between 0 and 1");

        if (ratingScore < 0 || ratingScore > 1)
            throw new ArgumentOutOfRangeException(
                nameof(ratingScore),
                "Rating score must be between 0 and 1");

        if (distanceScore < 0 || distanceScore > 1)
            throw new ArgumentOutOfRangeException(
                nameof(distanceScore),
                "Distance score must be between 0 and 1");

        ContractorId = contractorId;
        OverallScore = overallScore;
        AvailabilityScore = availabilityScore;
        RatingScore = ratingScore;
        DistanceScore = distanceScore;
        BestAvailableSlot = bestAvailableSlot;
    }

    /// <summary>
    /// Compares scores for sorting. Higher scores come first (descending).
    /// Tie-breaker: AvailabilityScore, then RatingScore, then ContractorId.
    /// </summary>
    public int CompareTo(ContractorScore? other)
    {
        if (other == null) return 1;

        // Primary: Overall score (descending)
        var scoreComparison = other.OverallScore.CompareTo(OverallScore);
        if (scoreComparison != 0) return scoreComparison;

        // Tie-breaker 1: Availability score (descending)
        var availabilityComparison = other.AvailabilityScore.CompareTo(AvailabilityScore);
        if (availabilityComparison != 0) return availabilityComparison;

        // Tie-breaker 2: Rating score (descending)
        var ratingComparison = other.RatingScore.CompareTo(RatingScore);
        if (ratingComparison != 0) return ratingComparison;

        // Tie-breaker 3: ContractorId (ascending for stable sort)
        return ContractorId.CompareTo(other.ContractorId);
    }

    public override string ToString()
    {
        return $"Score: {OverallScore:F2} " +
               $"(Avail: {AvailabilityScore:F2}, " +
               $"Rating: {RatingScore:F2}, " +
               $"Dist: {DistanceScore:F2})";
    }
}
