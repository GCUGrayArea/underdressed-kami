using MediatR;
using SmartScheduler.Application.DTOs.Common;
using SmartScheduler.Application.DTOs.Contractors;
using SmartScheduler.Application.Queries.Contractors;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.QueryHandlers.Contractors;

/// <summary>
/// Handler for searching contractors with multiple filter criteria.
/// Supports filtering by job type, rating range, and active status.
/// Returns paginated results.
/// </summary>
public class SearchContractorsQueryHandler
    : IRequestHandler<SearchContractorsQuery, PagedResult<ContractorListItemDto>>
{
    private readonly IContractorRepository _repository;

    public SearchContractorsQueryHandler(IContractorRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PagedResult<ContractorListItemDto>> Handle(
        SearchContractorsQuery request,
        CancellationToken cancellationToken)
    {
        var (contractors, totalCount) = await _repository.SearchAsync(
            request.JobTypeId,
            request.MinRating,
            request.MaxRating,
            request.IsActive,
            request.Page,
            request.PageSize,
            cancellationToken);

        if (totalCount == 0)
            return PagedResult<ContractorListItemDto>.Empty(request.Page, request.PageSize);

        var contractorsList = contractors.ToList();
        var items = await MapToDtos(contractorsList, cancellationToken);

        return new PagedResult<ContractorListItemDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }

    private async Task<List<ContractorListItemDto>> MapToDtos(
        List<Domain.Entities.Contractor> contractors,
        CancellationToken cancellationToken)
    {
        var dtos = new List<ContractorListItemDto>();

        foreach (var contractor in contractors)
        {
            var jobType = await _repository.GetJobTypeByIdAsync(
                contractor.JobTypeId,
                cancellationToken);

            dtos.Add(new ContractorListItemDto(
                contractor.Id,
                contractor.FormattedId,
                contractor.Name,
                jobType?.Name ?? string.Empty,
                contractor.Rating,
                contractor.IsActive,
                contractor.Phone,
                contractor.Email));
        }

        return dtos;
    }
}
