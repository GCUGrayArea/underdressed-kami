using MediatR;
using SmartScheduler.Application.Common;
using SmartScheduler.Application.DTOs;

namespace SmartScheduler.Application.Commands;

/// <summary>
/// Command to update an existing contractor's details.
/// </summary>
public class UpdateContractorCommand : IRequest<Result>
{
    public Guid ContractorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid JobTypeId { get; set; }
    public LocationDto BaseLocation { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
