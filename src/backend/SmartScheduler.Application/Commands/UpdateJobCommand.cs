using MediatR;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.Commands;

/// <summary>
/// Command to update job details.
/// Can update location, desired date/time, and estimated duration.
/// </summary>
public class UpdateJobCommand : IRequest<Unit>
{
    public Guid JobId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DesiredDate { get; set; }
    public TimeOnly? DesiredTime { get; set; }
    public decimal EstimatedDurationHours { get; set; }
}
