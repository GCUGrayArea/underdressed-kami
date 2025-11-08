using MediatR;
using SmartScheduler.Application.DTOs.Recommendations;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.Queries.Recommendations;

/// <summary>
/// Query to get ranked contractors for a job based on weighted scoring.
/// </summary>
public class GetRankedContractorsQuery : IRequest<List<RankedContractorDto>>
{
    public Guid JobTypeId { get; set; }
    public DateOnly TargetDate { get; set; }
    public TimeOnly TargetTime { get; set; }
    public Location JobLocation { get; set; } = null!;
    public double RequiredDurationHours { get; set; }
    public int TopN { get; set; } = 5;
}
