using MediatR;
using SmartScheduler.Application.DTOs;

namespace SmartScheduler.Application.Queries;

/// <summary>
/// Query to retrieve available time slots for a contractor on a specific date.
/// </summary>
public class GetContractorAvailabilityQuery : IRequest<AvailabilityDto>
{
    public Guid ContractorId { get; set; }
    public DateTime TargetDate { get; set; }
    public double RequiredDurationHours { get; set; }
}
