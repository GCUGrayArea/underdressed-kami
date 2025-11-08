using System.ComponentModel.DataAnnotations;

namespace SmartScheduler.WebApi.Models.Requests;

/// <summary>
/// Request model for creating a new job.
/// Includes validation attributes for automatic model validation.
/// </summary>
public class CreateJobRequest
{
    [Required(ErrorMessage = "Job type is required")]
    public Guid JobTypeId { get; set; }

    [Required(ErrorMessage = "Customer ID is required")]
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "Customer ID must be between 1 and 100 characters")]
    public string CustomerId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(200, MinimumLength = 1,
        ErrorMessage = "Customer name must be between 1 and 200 characters")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location latitude is required")]
    [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "Location longitude is required")]
    [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    [StringLength(500, ErrorMessage = "Address must not exceed 500 characters")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Desired date is required")]
    public DateTime DesiredDate { get; set; }

    public TimeOnly? DesiredTime { get; set; }

    [Required(ErrorMessage = "Estimated duration is required")]
    [Range(0.1, 24.0,
        ErrorMessage = "Estimated duration must be between 0.1 and 24 hours")]
    public decimal EstimatedDurationHours { get; set; }
}
