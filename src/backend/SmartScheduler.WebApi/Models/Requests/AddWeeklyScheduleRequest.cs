using System.ComponentModel.DataAnnotations;

namespace SmartScheduler.WebApi.Models.Requests;

/// <summary>
/// Request model for adding a weekly schedule entry.
/// Represents a single time block for a specific day of the week.
/// </summary>
public class AddWeeklyScheduleRequest
{
    [Required(ErrorMessage = "Day of week is required")]
    [Range(0, 6, ErrorMessage = "Day of week must be between 0 (Sunday) and 6 (Saturday)")]
    public DayOfWeek DayOfWeek { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public TimeOnly EndTime { get; set; }
}
