namespace SmartScheduler.Application.DTOs.Recommendations;

/// <summary>
/// DTO representing the breakdown of a contractor's score components.
/// Provides transparency into how the overall score was calculated.
/// </summary>
public class ScoreBreakdownDto
{
    public decimal AvailabilityScore { get; set; }
    public decimal RatingScore { get; set; }
    public decimal DistanceScore { get; set; }
    public decimal OverallScore { get; set; }
}
