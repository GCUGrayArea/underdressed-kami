using System.ComponentModel.DataAnnotations;
using SmartScheduler.Application.DTOs;

namespace SmartScheduler.WebApi.Models.Requests;

/// <summary>
/// Request model for updating an existing contractor.
/// Includes validation attributes for automatic model validation.
/// </summary>
public class UpdateContractorRequest
{
    [Required(ErrorMessage = "Contractor name is required")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job type is required")]
    public Guid JobTypeId { get; set; }

    [Required(ErrorMessage = "Base location is required")]
    public LocationDto BaseLocation { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string? Email { get; set; }

    public List<AddWeeklyScheduleRequest> WeeklySchedule { get; set; } = new();
}
