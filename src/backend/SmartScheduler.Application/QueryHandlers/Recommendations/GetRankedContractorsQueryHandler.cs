using MediatR;
using SmartScheduler.Application.DTOs.Contractors;
using SmartScheduler.Application.DTOs.Recommendations;
using SmartScheduler.Application.Queries.Recommendations;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.Services;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.QueryHandlers.Recommendations;

/// <summary>
/// Handler for GetRankedContractorsQuery.
/// Orchestrates scoring service to rank contractors for a job.
/// </summary>
public class GetRankedContractorsQueryHandler
    : IRequestHandler<GetRankedContractorsQuery, List<RankedContractorDto>>
{
    private readonly IScoringService _scoringService;
    private readonly IContractorRepository _contractorRepository;
    private readonly IDistanceService _distanceService;

    public GetRankedContractorsQueryHandler(
        IScoringService scoringService,
        IContractorRepository contractorRepository,
        IDistanceService distanceService)
    {
        _scoringService = scoringService ??
            throw new ArgumentNullException(nameof(scoringService));
        _contractorRepository = contractorRepository ??
            throw new ArgumentNullException(nameof(contractorRepository));
        _distanceService = distanceService ??
            throw new ArgumentNullException(nameof(distanceService));
    }

    public async Task<List<RankedContractorDto>> Handle(
        GetRankedContractorsQuery request,
        CancellationToken cancellationToken)
    {
        // Get all active contractors with matching job type
        var allContractors = await _contractorRepository.GetActiveContractorsAsync(
            cancellationToken);

        var matchingContractors = allContractors
            .Where(c => c.JobTypeId == request.JobTypeId)
            .ToList();

        // If no contractors match, return empty list
        if (!matchingContractors.Any())
            return new List<RankedContractorDto>();

        // Rank contractors using scoring service
        var scores = await _scoringService.RankContractorsAsync(
            matchingContractors,
            request.TargetDate,
            request.TargetTime,
            request.JobLocation,
            request.RequiredDurationHours,
            null, // Use default weights
            cancellationToken);

        // Take top N
        var topScores = scores.Take(request.TopN).ToList();

        // Map to DTOs
        var results = new List<RankedContractorDto>();
        foreach (var score in topScores)
        {
            var contractor = matchingContractors.First(c => c.Id == score.ContractorId);
            var jobType = await _contractorRepository.GetJobTypeByIdAsync(
                contractor.JobTypeId,
                cancellationToken);

            // Get distance for DTO
            var distanceMiles = await _distanceService.CalculateDistanceMilesAsync(
                contractor.BaseLocation,
                request.JobLocation,
                cancellationToken);

            results.Add(MapToDto(contractor, jobType?.Name ?? "Unknown", score, distanceMiles));
        }

        return results;
    }

    /// <summary>
    /// Maps contractor and score to DTO.
    /// </summary>
    private RankedContractorDto MapToDto(
        Domain.Entities.Contractor contractor,
        string jobTypeName,
        ContractorScore score,
        double distanceMiles)
    {
        return new RankedContractorDto
        {
            ContractorId = contractor.Id,
            FormattedId = contractor.FormattedId,
            Name = contractor.Name,
            JobType = jobTypeName,
            Rating = contractor.Rating,
            BaseLocation = new LocationDto(
                contractor.BaseLocation.Latitude,
                contractor.BaseLocation.Longitude,
                contractor.BaseLocation.Address),
            DistanceMiles = distanceMiles,
            BestAvailableSlot = score.BestAvailableSlot != null
                ? new TimeSlotDto
                {
                    Start = score.BestAvailableSlot.Start,
                    End = score.BestAvailableSlot.End,
                    DurationHours = score.BestAvailableSlot.DurationHours
                }
                : null,
            ScoreBreakdown = new ScoreBreakdownDto
            {
                AvailabilityScore = score.AvailabilityScore,
                RatingScore = score.RatingScore,
                DistanceScore = score.DistanceScore,
                OverallScore = score.OverallScore
            }
        };
    }
}
