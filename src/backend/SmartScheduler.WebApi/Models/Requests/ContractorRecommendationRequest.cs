using System.ComponentModel.DataAnnotations;
using SmartScheduler.Application.DTOs;

namespace SmartScheduler.WebApi.Models.Requests;

/// <summary>
/// Request model for contractor recommendation API endpoint.
/// Includes all parameters needed to rank contractors for a job.
/// </summary>
public class ContractorRecommendationRequest
{
    [Required(ErrorMessage = "Job type is required")]
    public Guid JobTypeId { get; set; }

    [Required(ErrorMessage = "Desired date is required")]
    public DateOnly DesiredDate { get; set; }

    [Required(ErrorMessage = "Desired time is required")]
    public TimeOnly DesiredTime { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public LocationDto Location { get; set; } = null!;

    [Required(ErrorMessage = "Estimated duration is required")]
    [Range(0.1, 24.0, ErrorMessage = "Duration must be between 0.1 and 24 hours")]
    public double EstimatedDurationHours { get; set; }

    [Range(1, 20, ErrorMessage = "TopN must be between 1 and 20")]
    public int TopN { get; set; } = 5;
}
