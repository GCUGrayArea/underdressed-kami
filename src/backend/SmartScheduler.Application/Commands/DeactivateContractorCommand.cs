using MediatR;
using SmartScheduler.Application.Common;

namespace SmartScheduler.Application.Commands;

/// <summary>
/// Command to deactivate a contractor (soft delete).
/// </summary>
public class DeactivateContractorCommand : IRequest<Result>
{
    public Guid ContractorId { get; set; }
}
