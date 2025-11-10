using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartScheduler.Application.Commands;
using SmartScheduler.Application.Queries;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.WebApi.Models.Requests;
using SmartScheduler.WebApi.Models.Responses;

namespace SmartScheduler.WebApi.Controllers;

/// <summary>
/// API controller for job management operations.
/// Thin controller delegating to MediatR commands and queries.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        IMediator mediator,
        ILogger<JobsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new job request.
    /// </summary>
    /// <param name="request">Job creation data</param>
    /// <returns>201 Created with job ID and location header</returns>
    [HttpPost]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJob(
        [FromBody] CreateJobRequest request)
    {
        _logger.LogInformation(
            "Creating job: JobType={JobTypeId}, Customer={CustomerName}",
            request.JobTypeId,
            request.CustomerName);

        var command = MapToCreateCommand(request);
        var jobId = await _mediator.Send(command);

        var getQuery = new GetJobByIdQuery(jobId);
        var jobDto = await _mediator.Send(getQuery);

        if (jobDto == null)
        {
            return CreatedAtAction(
                nameof(GetJobById),
                new { id = jobId },
                new { id = jobId });
        }

        var response = new JobResponse(jobDto);
        return CreatedAtAction(
            nameof(GetJobById),
            new { id = jobId },
            response);
    }

    /// <summary>
    /// Retrieves all jobs with optional status filtering.
    /// </summary>
    /// <param name="status">Optional status filter</param>
    /// <returns>List of jobs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobs([FromQuery] JobStatus? status = null)
    {
        try
        {
            _logger.LogInformation(
                "STARLING: GET /api/jobs - Retrieving jobs with status filter: {Status}",
                status?.ToString() ?? "None");

            var query = new GetJobsByStatusQuery(status);

            _logger.LogInformation("STARLING: Sending query to MediatR handler");
            var jobDtos = await _mediator.Send(query);

            _logger.LogInformation("STARLING: Successfully retrieved {Count} jobs", jobDtos.Count());
            var responses = jobDtos.Select(dto => new JobResponse(dto));
            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "STARLING: ERROR in GET /api/jobs - {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a job by ID.
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>Job details or 404 if not found</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobById(Guid id)
    {
        var query = new GetJobByIdQuery(id);
        var jobDto = await _mediator.Send(query);

        if (jobDto == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Job not found",
                Detail = $"Job with ID {id} not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        var response = new JobResponse(jobDto);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing job.
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <param name="request">Updated job data</param>
    /// <returns>200 OK or 404 if not found</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateJob(
        Guid id,
        [FromBody] UpdateJobRequest request)
    {
        _logger.LogInformation("Updating job: {JobId}", id);

        var command = MapToUpdateCommand(id, request);

        try
        {
            await _mediator.Send(command);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Job not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }

    /// <summary>
    /// Assigns a contractor to a job.
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <param name="request">Contractor assignment data</param>
    /// <returns>200 OK or 404/400 if validation fails</returns>
    [HttpPost("{id:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignContractor(
        Guid id,
        [FromBody] AssignContractorRequest request)
    {
        _logger.LogInformation(
            "Assigning contractor {ContractorId} to job {JobId}",
            request.ContractorId,
            id);

        var command = new AssignContractorCommand
        {
            JobId = id,
            ContractorId = request.ContractorId,
            ScheduledStartTime = request.ScheduledStartTime
        };

        try
        {
            await _mediator.Send(command);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Assignment failed",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid assignment request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    private static CreateJobCommand MapToCreateCommand(
        CreateJobRequest request)
    {
        return new CreateJobCommand
        {
            JobTypeId = request.JobTypeId,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DesiredDate = request.DesiredDate,
            DesiredTime = request.DesiredTime,
            EstimatedDurationHours = request.EstimatedDurationHours
        };
    }

    private static UpdateJobCommand MapToUpdateCommand(
        Guid jobId,
        UpdateJobRequest request)
    {
        return new UpdateJobCommand
        {
            JobId = jobId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DesiredDate = request.DesiredDate,
            DesiredTime = request.DesiredTime,
            EstimatedDurationHours = request.EstimatedDurationHours
        };
    }
}
