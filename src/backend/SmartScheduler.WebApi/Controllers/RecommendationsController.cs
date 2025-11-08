using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartScheduler.Application.Queries.Recommendations;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.WebApi.Models.Requests;
using SmartScheduler.WebApi.Models.Responses;

namespace SmartScheduler.WebApi.Controllers;

/// <summary>
/// API controller for contractor recommendation operations.
/// Provides ranked contractor lists based on availability, rating, and distance.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecommendationsController> _logger;

    public RecommendationsController(
        IMediator mediator,
        ILogger<RecommendationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets ranked contractor recommendations for a job.
    /// Returns top N contractors sorted by weighted score.
    /// </summary>
    /// <param name="request">Job requirements and preferences</param>
    /// <returns>Ranked list of contractors with scores and availability</returns>
    [HttpPost("contractors")]
    [ProducesResponseType(typeof(List<ContractorRecommendationResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetContractorRecommendations(
        [FromBody] ContractorRecommendationRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Getting contractor recommendations: JobType={JobTypeId}, " +
            "Date={Date}, Time={Time}, TopN={TopN}",
            request.JobTypeId,
            request.DesiredDate,
            request.DesiredTime,
            request.TopN);

        var query = MapToQuery(request);
        var rankedContractors = await _mediator.Send(query);

        stopwatch.Stop();

        _logger.LogInformation(
            "Contractor recommendations returned {Count} results " +
            "in {ElapsedMs}ms",
            rankedContractors.Count,
            stopwatch.ElapsedMilliseconds);

        if (rankedContractors.Count == 0)
        {
            return Ok(new List<ContractorRecommendationResponse>());
        }

        var response = rankedContractors
            .Select(dto => new ContractorRecommendationResponse(dto))
            .ToList();

        return Ok(response);
    }

    private static GetRankedContractorsQuery MapToQuery(
        ContractorRecommendationRequest request)
    {
        var location = new Location(
            request.Location.Latitude,
            request.Location.Longitude,
            request.Location.Address);

        return new GetRankedContractorsQuery
        {
            JobTypeId = request.JobTypeId,
            TargetDate = request.DesiredDate,
            TargetTime = request.DesiredTime,
            JobLocation = location,
            RequiredDurationHours = request.EstimatedDurationHours,
            TopN = request.TopN
        };
    }
}
