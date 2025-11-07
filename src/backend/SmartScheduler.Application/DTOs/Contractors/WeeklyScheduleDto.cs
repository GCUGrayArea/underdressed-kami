namespace SmartScheduler.Application.DTOs.Contractors;

/// <summary>
/// DTO for contractor working hours schedule entry.
/// Represents a single time block for a specific day of the week.
/// </summary>
public class WeeklyScheduleDto
{
    public Guid Id { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public double DurationHours { get; init; }

    public WeeklyScheduleDto() { }

    public WeeklyScheduleDto(
        Guid id,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        Id = id;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        DurationHours = (endTime - startTime).TotalHours;
    }
}
