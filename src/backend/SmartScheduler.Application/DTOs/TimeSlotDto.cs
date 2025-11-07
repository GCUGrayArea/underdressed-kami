namespace SmartScheduler.Application.DTOs;

/// <summary>
/// DTO representing an available time slot.
/// </summary>
public class TimeSlotDto
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public double DurationHours { get; set; }
}
