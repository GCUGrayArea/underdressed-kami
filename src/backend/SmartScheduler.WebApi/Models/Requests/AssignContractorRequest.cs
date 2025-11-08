using System.ComponentModel.DataAnnotations;

namespace SmartScheduler.WebApi.Models.Requests;

/// <summary>
/// Request model for assigning a contractor to a job.
/// Includes validation attributes for automatic model validation.
/// </summary>
public class AssignContractorRequest
{
    [Required(ErrorMessage = "Contractor ID is required")]
    public Guid ContractorId { get; set; }

    [Required(ErrorMessage = "Scheduled start time is required")]
    public DateTime ScheduledStartTime { get; set; }
}
