using SmartScheduler.Application.DTOs.Contractors;

namespace SmartScheduler.Application.DTOs.Recommendations;

/// <summary>
/// DTO representing a ranked contractor with score and availability details.
/// </summary>
public class RankedContractorDto
{
    public Guid ContractorId { get; set; }
    public string FormattedId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public Contractors.LocationDto BaseLocation { get; set; } = null!;
    public double DistanceMiles { get; set; }
    public TimeSlotDto? BestAvailableSlot { get; set; }
    public ScoreBreakdownDto ScoreBreakdown { get; set; } = null!;
}

/// <summary>
/// DTO representing a time slot.
/// </summary>
public class TimeSlotDto
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public double DurationHours { get; set; }
}
