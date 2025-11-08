using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Services;

/// <summary>
/// Domain service implementing the weighted scoring algorithm
/// for contractor ranking.
/// </summary>
public class ScoringService : IScoringService
{
    private readonly IAvailabilityService _availabilityService;
    private readonly IDistanceService _distanceService;
    private readonly IJobRepository _jobRepository;
    private readonly IContractorRepository _contractorRepository;

    public ScoringService(
        IAvailabilityService availabilityService,
        IDistanceService distanceService,
        IJobRepository jobRepository,
        IContractorRepository contractorRepository)
    {
        _availabilityService = availabilityService ??
            throw new ArgumentNullException(nameof(availabilityService));
        _distanceService = distanceService ??
            throw new ArgumentNullException(nameof(distanceService));
        _jobRepository = jobRepository ??
            throw new ArgumentNullException(nameof(jobRepository));
        _contractorRepository = contractorRepository ??
            throw new ArgumentNullException(nameof(contractorRepository));
    }

    /// <summary>
    /// Main orchestration method that ranks contractors.
    /// </summary>
    public async Task<List<ContractorScore>> RankContractorsAsync(
        List<Contractor> contractors,
        DateOnly targetDate,
        TimeOnly targetTime,
        Location jobLocation,
        double requiredDurationHours,
        ScoringWeights? weights = null,
        CancellationToken cancellationToken = default)
    {
        weights ??= ScoringWeights.Default();
        var scores = new List<ContractorScore>();

        foreach (var contractor in contractors)
        {
            var score = await CalculateContractorScoreAsync(
                contractor,
                targetDate,
                targetTime,
                jobLocation,
                requiredDurationHours,
                weights,
                cancellationToken);

            // Only include contractors with non-zero overall score
            if (score != null && score.OverallScore > 0)
            {
                scores.Add(score);
            }
        }

        // Sort by overall score descending (uses ContractorScore.CompareTo)
        scores.Sort();

        return scores;
    }

    /// <summary>
    /// Calculates score for a single contractor.
    /// Returns null if contractor has no availability or is too far.
    /// </summary>
    private async Task<ContractorScore?> CalculateContractorScoreAsync(
        Contractor contractor,
        DateOnly targetDate,
        TimeOnly targetTime,
        Location jobLocation,
        double requiredDurationHours,
        ScoringWeights weights,
        CancellationToken cancellationToken)
    {
        // Calculate distance first (may filter out contractor)
        var distanceMiles = await _distanceService.CalculateDistanceMilesAsync(
            contractor.BaseLocation,
            jobLocation,
            cancellationToken);

        var distanceScore = CalculateDistanceScore(distanceMiles);

        // Filter out contractors >50 miles (distance score = 0)
        if (distanceScore == 0)
            return null;

        // Get availability
        var availabilityResult = await CalculateAvailabilityScoreAsync(
            contractor.Id,
            targetDate,
            targetTime,
            requiredDurationHours,
            cancellationToken);

        // Filter out contractors with no availability
        if (availabilityResult.Score == 0)
            return null;

        // Calculate rating score
        var ratingScore = CalculateRatingScore(contractor.Rating);

        // Apply weights to calculate overall score
        var overallScore = ApplyWeights(
            availabilityResult.Score,
            ratingScore,
            distanceScore,
            weights);

        return new ContractorScore(
            contractor.Id,
            overallScore,
            availabilityResult.Score,
            ratingScore,
            distanceScore,
            availabilityResult.BestSlot);
    }

    /// <summary>
    /// Calculates availability score based on proximity to desired time.
    /// 1.0 = slot includes desired time
    /// 0.7 = slot within 2 hours of desired time
    /// 0.4 = slot same day but >2 hours difference
    /// 0.0 = no availability
    /// </summary>
    private async Task<(decimal Score, TimeSlot? BestSlot)> CalculateAvailabilityScoreAsync(
        Guid contractorId,
        DateOnly targetDate,
        TimeOnly targetTime,
        double requiredDurationHours,
        CancellationToken cancellationToken)
    {
        // Get contractor's working hours
        var allSchedules = await _contractorRepository.GetSchedulesByContractorIdAsync(
            contractorId,
            cancellationToken);

        // Filter to target day of week
        var dayOfWeek = targetDate.DayOfWeek;
        var workingHours = allSchedules
            .Where(s => s.DayOfWeek == dayOfWeek)
            .ToList();

        // Get contractor's jobs for the target date
        var targetDateTime = targetDate.ToDateTime(TimeOnly.MinValue);
        var existingJobs = await _jobRepository.GetByContractorAndDateAsync(
            contractorId,
            targetDateTime,
            cancellationToken);

        // Calculate available slots
        var availableSlots = _availabilityService.CalculateAvailability(
            workingHours,
            existingJobs,
            requiredDurationHours);

        if (!availableSlots.Any())
            return (0m, null);

        // Find best slot and calculate score
        return CalculateAvailabilityScoreFromSlots(availableSlots, targetTime);
    }

    /// <summary>
    /// Calculates availability score from available slots.
    /// </summary>
    private (decimal Score, TimeSlot? BestSlot) CalculateAvailabilityScoreFromSlots(
        IEnumerable<TimeSlot> slots,
        TimeOnly targetTime)
    {
        var slotList = slots.ToList();

        if (!slotList.Any())
            return (0m, null);

        // Find slot that includes target time (perfect match)
        var perfectMatch = slotList.FirstOrDefault(s => s.Contains(targetTime));
        if (perfectMatch != null)
        {
            return (1.0m, perfectMatch);
        }

        // Find slot closest to target time
        var closestSlot = slotList
            .OrderBy(s => Math.Abs((s.Start.ToTimeSpan() - targetTime.ToTimeSpan()).TotalHours))
            .First();

        // Calculate time difference in hours
        var timeDiff = Math.Abs(
            (closestSlot.Start.ToTimeSpan() - targetTime.ToTimeSpan()).TotalHours);

        if (timeDiff <= 2.0)
            return (0.7m, closestSlot);

        // Same day but > 2 hours difference
        return (0.4m, closestSlot);
    }

    /// <summary>
    /// Calculates rating score by normalizing to 0-1 scale.
    /// </summary>
    private decimal CalculateRatingScore(decimal rating)
    {
        return rating / 5.0m;
    }

    /// <summary>
    /// Calculates distance score with thresholds.
    /// 1.0 for <10 miles
    /// Linear decay from 1.0 to 0.3 for 10-30 miles
    /// 0.2 for 30-50 miles
    /// 0.0 for >50 miles
    /// </summary>
    private decimal CalculateDistanceScore(double distanceMiles)
    {
        if (distanceMiles > 50)
            return 0m;

        if (distanceMiles < 10)
            return 1.0m;

        if (distanceMiles <= 30)
        {
            // Linear decay from 1.0 to 0.3 over 20 miles (10-30)
            var decay = (decimal)((distanceMiles - 10) / 20.0);
            return 1.0m - (decay * 0.7m);
        }

        // 30-50 miles
        return 0.2m;
    }

    /// <summary>
    /// Applies weights to component scores to get overall score.
    /// </summary>
    private decimal ApplyWeights(
        decimal availabilityScore,
        decimal ratingScore,
        decimal distanceScore,
        ScoringWeights weights)
    {
        return (availabilityScore * weights.AvailabilityWeight) +
               (ratingScore * weights.RatingWeight) +
               (distanceScore * weights.DistanceWeight);
    }
}
