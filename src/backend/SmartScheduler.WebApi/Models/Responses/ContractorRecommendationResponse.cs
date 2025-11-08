using SmartScheduler.Application.DTOs.Contractors;
using SmartScheduler.Application.DTOs.Recommendations;

namespace SmartScheduler.WebApi.Models.Responses;

/// <summary>
/// Response model for contractor recommendation API endpoint.
/// Maps from RankedContractorDto to API response format.
/// </summary>
public class ContractorRecommendationResponse
{
    public Guid ContractorId { get; init; }
    public string FormattedId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string JobType { get; init; } = string.Empty;
    public decimal Rating { get; init; }
    public LocationDto BaseLocation { get; init; } = null!;
    public double DistanceMiles { get; init; }
    public TimeSlotResponse? BestAvailableSlot { get; init; }
    public ScoreBreakdownResponse ScoreBreakdown { get; init; } = null!;

    public ContractorRecommendationResponse() { }

    public ContractorRecommendationResponse(RankedContractorDto dto)
    {
        ContractorId = dto.ContractorId;
        FormattedId = dto.FormattedId;
        Name = dto.Name;
        JobType = dto.JobType;
        Rating = dto.Rating;
        BaseLocation = new LocationDto
        {
            Latitude = dto.BaseLocation.Latitude,
            Longitude = dto.BaseLocation.Longitude,
            Address = dto.BaseLocation.Address
        };
        DistanceMiles = dto.DistanceMiles;
        BestAvailableSlot = dto.BestAvailableSlot != null
            ? new TimeSlotResponse(dto.BestAvailableSlot)
            : null;
        ScoreBreakdown = new ScoreBreakdownResponse(dto.ScoreBreakdown);
    }
}

/// <summary>
/// Time slot data for API responses.
/// </summary>
public class TimeSlotResponse
{
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
    public double DurationHours { get; init; }

    public TimeSlotResponse() { }

    public TimeSlotResponse(TimeSlotDto dto)
    {
        Start = dto.Start;
        End = dto.End;
        DurationHours = dto.DurationHours;
    }
}

/// <summary>
/// Score breakdown data for API responses.
/// Provides transparency into how contractor ranking was calculated.
/// </summary>
public class ScoreBreakdownResponse
{
    public decimal AvailabilityScore { get; init; }
    public decimal RatingScore { get; init; }
    public decimal DistanceScore { get; init; }
    public decimal OverallScore { get; init; }

    public ScoreBreakdownResponse() { }

    public ScoreBreakdownResponse(ScoreBreakdownDto dto)
    {
        AvailabilityScore = dto.AvailabilityScore;
        RatingScore = dto.RatingScore;
        DistanceScore = dto.DistanceScore;
        OverallScore = dto.OverallScore;
    }
}
