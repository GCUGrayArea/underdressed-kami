using MediatR;
using SmartScheduler.Application.Common;
using SmartScheduler.Application.DTOs;
using SmartScheduler.Application.DTOs.Contractors;

namespace SmartScheduler.Application.Commands;

/// <summary>
/// Command to create a new contractor.
/// </summary>
public class CreateContractorCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public Guid JobTypeId { get; set; }
    public DTOs.LocationDto BaseLocation { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal Rating { get; set; } = 3.0m;
    public List<WeeklyScheduleDto> WeeklySchedule { get; set; } = new();
}
