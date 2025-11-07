namespace SmartScheduler.Domain.Entities;

/// <summary>
/// Represents a contractor's working hours for a specific day of the week.
/// Multiple entries per day allow for split shifts (e.g., 9-12, 2-5).
/// </summary>
public class WeeklySchedule
{
    public Guid Id { get; private set; }
    public Guid ContractorId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    // EF Core constructor
    private WeeklySchedule() { }

    public WeeklySchedule(
        Guid contractorId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (contractorId == Guid.Empty)
            throw new ArgumentException("Contractor ID cannot be empty", nameof(contractorId));

        if (endTime <= startTime)
            throw new ArgumentException(
                "End time must be after start time",
                nameof(endTime));

        Id = Guid.NewGuid();
        ContractorId = contractorId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public void UpdateTimes(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException(
                "End time must be after start time",
                nameof(endTime));

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Checks if a specific time falls within this schedule slot.
    /// </summary>
    public bool ContainsTime(TimeOnly time)
    {
        return time >= StartTime && time < EndTime;
    }

    /// <summary>
    /// Gets the duration of this work period in hours.
    /// </summary>
    public double DurationHours()
    {
        return (EndTime - StartTime).TotalHours;
    }
}
