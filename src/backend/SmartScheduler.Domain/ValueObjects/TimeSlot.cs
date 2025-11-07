namespace SmartScheduler.Domain.ValueObjects;

/// <summary>
/// Immutable value object representing a time range.
/// Used to represent available or occupied time slots in scheduling.
/// </summary>
public class TimeSlot : IEquatable<TimeSlot>
{
    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    public TimeSlot(TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            throw new ArgumentException(
                "End time must be after start time",
                nameof(end));

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the duration of this time slot in hours.
    /// </summary>
    public double DurationHours => (End - Start).TotalHours;

    /// <summary>
    /// Checks if this time slot overlaps with another time slot.
    /// </summary>
    public bool Overlaps(TimeSlot other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return Start < other.End && End > other.Start;
    }

    /// <summary>
    /// Checks if a specific time falls within this time slot.
    /// </summary>
    public bool Contains(TimeOnly time)
    {
        return time >= Start && time < End;
    }

    /// <summary>
    /// Checks if this time slot can accommodate a job of given duration.
    /// </summary>
    public bool CanAccommodate(double requiredDurationHours)
    {
        return DurationHours >= requiredDurationHours;
    }

    public bool Equals(TimeSlot? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TimeSlot);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public override string ToString()
    {
        return $"{Start:HH:mm} - {End:HH:mm}";
    }

    public static bool operator ==(TimeSlot? left, TimeSlot? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TimeSlot? left, TimeSlot? right)
    {
        return !Equals(left, right);
    }
}
