using MediatR;
using SmartScheduler.Application.DTOs.Common;
using SmartScheduler.Application.DTOs.Contractors;

namespace SmartScheduler.Application.Queries.Contractors;

/// <summary>
/// Query to search and filter contractors with multiple criteria.
/// Supports filtering by job type, rating range, and active status.
/// Returns paginated results.
/// </summary>
public class SearchContractorsQuery : IRequest<PagedResult<ContractorListItemDto>>
{
    public Guid? JobTypeId { get; init; }
    public decimal? MinRating { get; init; }
    public decimal? MaxRating { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public SearchContractorsQuery() { }

    public SearchContractorsQuery(
        Guid? jobTypeId = null,
        decimal? minRating = null,
        decimal? maxRating = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20)
    {
        JobTypeId = jobTypeId;
        MinRating = minRating;
        MaxRating = maxRating;
        IsActive = isActive;
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 20;
    }
}
