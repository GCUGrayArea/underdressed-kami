using MediatR;
using SmartScheduler.Application.DTOs.Common;
using SmartScheduler.Application.DTOs.Contractors;
using SmartScheduler.Application.Queries.Contractors;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.QueryHandlers.Contractors;

/// <summary>
/// Handler for retrieving all contractors with pagination.
/// Returns lightweight list items for efficient display.
/// </summary>
public class GetAllContractorsQueryHandler
    : IRequestHandler<GetAllContractorsQuery, PagedResult<ContractorListItemDto>>
{
    private readonly IContractorRepository _repository;

    public GetAllContractorsQueryHandler(IContractorRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PagedResult<ContractorListItemDto>> Handle(
        GetAllContractorsQuery request,
        CancellationToken cancellationToken)
    {
        var (contractors, totalCount) = await _repository.GetPagedAsync(
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
