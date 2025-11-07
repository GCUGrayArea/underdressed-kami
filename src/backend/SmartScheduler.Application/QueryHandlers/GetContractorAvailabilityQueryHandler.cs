using MediatR;
using SmartScheduler.Application.DTOs;
using SmartScheduler.Application.Queries;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.Services;

namespace SmartScheduler.Application.QueryHandlers;

/// <summary>
/// Handler for GetContractorAvailabilityQuery.
/// Orchestrates repository calls and delegates to AvailabilityService.
/// </summary>
public class GetContractorAvailabilityQueryHandler
    : IRequestHandler<GetContractorAvailabilityQuery, AvailabilityDto>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IAvailabilityService _availabilityService;

    public GetContractorAvailabilityQueryHandler(
        IContractorRepository contractorRepository,
        IJobRepository jobRepository,
        IAvailabilityService availabilityService)
    {
        _contractorRepository = contractorRepository;
        _jobRepository = jobRepository;
        _availabilityService = availabilityService;
    }

    public async Task<AvailabilityDto> Handle(
        GetContractorAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        // Validate contractor exists
        var contractor = await _contractorRepository.GetByIdAsync(
            request.ContractorId,
            cancellationToken);

        if (contractor == null)
        {
            throw new InvalidOperationException(
                $"Contractor with ID {request.ContractorId} not found");
        }

        // Get contractor's working hours for target date
        var allSchedules = await _contractorRepository
            .GetSchedulesByContractorIdAsync(
                request.ContractorId,
                cancellationToken);

        var targetDayOfWeek = request.TargetDate.DayOfWeek;
        var workingHours = allSchedules
            .Where(s => s.DayOfWeek == targetDayOfWeek)
            .ToList();

        // Get existing jobs for that date
        var existingJobs = await _jobRepository.GetByContractorAndDateAsync(
            request.ContractorId,
            request.TargetDate,
            cancellationToken);

        // Calculate availability
        var availableSlots = _availabilityService.CalculateAvailability(
            workingHours,
            existingJobs,
            request.RequiredDurationHours);

        // Map to DTO
        return new AvailabilityDto
        {
            ContractorId = contractor.Id,
            ContractorFormattedId = contractor.FormattedId,
            ContractorName = contractor.Name,
            TargetDate = request.TargetDate.Date,
            AvailableSlots = availableSlots.Select(slot => new TimeSlotDto
            {
                Start = slot.Start,
                End = slot.End,
                DurationHours = slot.DurationHours
            }).ToList()
        };
    }
}
