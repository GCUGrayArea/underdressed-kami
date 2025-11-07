using MediatR;
using SmartScheduler.Application.DTOs.Common;
using SmartScheduler.Application.DTOs.Contractors;

namespace SmartScheduler.Application.Queries.Contractors;

/// <summary>
/// Query to retrieve all contractors with pagination.
/// Returns lightweight list items for efficient list display.
/// </summary>
public class GetAllContractorsQuery : IRequest<PagedResult<ContractorListItemDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public GetAllContractorsQuery() { }

    public GetAllContractorsQuery(int page, int pageSize)
    {
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 20;
    }
}
