using MediatR;
using SmartScheduler.Application.DTOs.Contractors;

namespace SmartScheduler.Application.Queries.Contractors;

/// <summary>
/// Query to retrieve a single contractor by ID.
/// Returns full contractor details including location and schedule.
/// </summary>
public class GetContractorByIdQuery : IRequest<ContractorDto?>
{
    public Guid ContractorId { get; init; }

    public GetContractorByIdQuery(Guid contractorId)
    {
        ContractorId = contractorId;
    }
}
