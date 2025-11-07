using MediatR;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.Commands;

/// <summary>
/// Command to create a new job request.
/// Returns the created job ID on success.
/// </summary>
public class CreateJobCommand : IRequest<Guid>
{
    public Guid JobTypeId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DesiredDate { get; set; }
    public TimeOnly? DesiredTime { get; set; }
    public decimal EstimatedDurationHours { get; set; }
}
