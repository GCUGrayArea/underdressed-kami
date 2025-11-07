using MediatR;
using SmartScheduler.Application.DTOs.Contractors;
using SmartScheduler.Application.Queries.Contractors;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.QueryHandlers.Contractors;

/// <summary>
/// Handler for retrieving a single contractor by ID with full details.
/// Includes job type, location, and weekly schedule information.
/// </summary>
public class GetContractorByIdQueryHandler
    : IRequestHandler<GetContractorByIdQuery, ContractorDto?>
{
    private readonly IContractorRepository _repository;

    public GetContractorByIdQueryHandler(IContractorRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ContractorDto?> Handle(
        GetContractorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var contractor = await _repository.GetByIdAsync(
            request.ContractorId,
            cancellationToken);

        if (contractor == null)
            return null;

        var jobType = await _repository.GetJobTypeByIdAsync(
            contractor.JobTypeId,
            cancellationToken);

        var schedules = await _repository.GetSchedulesByContractorIdAsync(
            contractor.Id,
            cancellationToken);

        return MapToDto(contractor, jobType?.Name ?? string.Empty, schedules);
    }

    private static ContractorDto MapToDto(
        Domain.Entities.Contractor contractor,
        string jobTypeName,
        IEnumerable<Domain.Entities.WeeklySchedule> schedules)
    {
        var location = new LocationDto(
            contractor.BaseLocation.Latitude,
            contractor.BaseLocation.Longitude,
            contractor.BaseLocation.Address);

        var scheduleDtos = schedules
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .Select(s => new WeeklyScheduleDto(
                s.Id,
                s.DayOfWeek,
                s.StartTime,
                s.EndTime))
            .ToList();

        return new ContractorDto(
            contractor.Id,
            contractor.FormattedId,
            contractor.Name,
            contractor.JobTypeId,
            jobTypeName,
            contractor.Rating,
            location,
            contractor.IsActive,
            scheduleDtos,
            contractor.Phone,
            contractor.Email);
    }
}
