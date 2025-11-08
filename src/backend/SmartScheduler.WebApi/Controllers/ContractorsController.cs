using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartScheduler.Application.Commands;
using SmartScheduler.Application.Queries.Contractors;
using SmartScheduler.WebApi.Models.Requests;
using SmartScheduler.WebApi.Models.Responses;

namespace SmartScheduler.WebApi.Controllers;

/// <summary>
/// API controller for contractor management operations.
/// Thin controller delegating to MediatR commands and queries.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContractorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContractorsController> _logger;

    public ContractorsController(
        IMediator mediator,
        ILogger<ContractorsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new contractor.
    /// </summary>
    /// <param name="request">Contractor creation data</param>
    /// <returns>201 Created with contractor ID and location header</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ContractorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContractor(
        [FromBody] CreateContractorRequest request)
    {
        _logger.LogInformation(
            "Creating contractor: {Name}, JobType: {JobTypeId}",
            request.Name,
            request.JobTypeId);

        var command = MapToCreateCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Contractor creation failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        var contractorId = result.Value;
        var getQuery = new GetContractorByIdQuery(contractorId);
        var contractorDto = await _mediator.Send(getQuery);

        if (contractorDto == null)
        {
            return CreatedAtAction(
                nameof(GetContractorById),
                new { id = contractorId },
                new { id = contractorId });
        }

        var response = new ContractorResponse(contractorDto);
        return CreatedAtAction(
            nameof(GetContractorById),
            new { id = contractorId },
            response);
    }

    /// <summary>
    /// Retrieves all contractors with pagination and filtering.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="jobTypeId">Filter by job type</param>
    /// <param name="minRating">Minimum rating filter</param>
    /// <param name="maxRating">Maximum rating filter</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of contractors</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContractors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? jobTypeId = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] decimal? maxRating = null,
        [FromQuery] bool? isActive = null)
    {
        if (HasFilters(jobTypeId, minRating, maxRating, isActive))
        {
            var searchQuery = new SearchContractorsQuery(
                jobTypeId,
                minRating,
                maxRating,
                isActive,
                page,
                pageSize);

            var searchResult = await _mediator.Send(searchQuery);
            return Ok(searchResult);
        }

        var query = new GetAllContractorsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a contractor by ID.
    /// </summary>
    /// <param name="id">Contractor ID</param>
    /// <returns>Contractor details or 404 if not found</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContractorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContractorById(Guid id)
    {
        var query = new GetContractorByIdQuery(id);
        var contractorDto = await _mediator.Send(query);

        if (contractorDto == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Contractor not found",
                Detail = $"Contractor with ID {id} not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        var response = new ContractorResponse(contractorDto);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing contractor.
    /// </summary>
    /// <param name="id">Contractor ID</param>
    /// <param name="request">Updated contractor data</param>
    /// <returns>200 OK or 404 if not found</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateContractor(
        Guid id,
        [FromBody] UpdateContractorRequest request)
    {
        _logger.LogInformation(
            "Updating contractor: {ContractorId}",
            id);

        var command = MapToUpdateCommand(id, request);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Contractor not found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return BadRequest(new ProblemDetails
            {
                Title = "Contractor update failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok();
    }

    /// <summary>
    /// Soft-deletes a contractor (marks as inactive).
    /// </summary>
    /// <param name="id">Contractor ID</param>
    /// <returns>204 No Content or 404 if not found</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContractor(Guid id)
    {
        _logger.LogInformation(
            "Soft-deleting contractor: {ContractorId}",
            id);

        var command = new DeactivateContractorCommand { ContractorId = id };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Contractor not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }

    private static bool HasFilters(
        Guid? jobTypeId,
        decimal? minRating,
        decimal? maxRating,
        bool? isActive)
    {
        return jobTypeId.HasValue
            || minRating.HasValue
            || maxRating.HasValue
            || isActive.HasValue;
    }

    private static CreateContractorCommand MapToCreateCommand(
        CreateContractorRequest request)
    {
        return new CreateContractorCommand
        {
            Name = request.Name,
            JobTypeId = request.JobTypeId,
            BaseLocation = request.BaseLocation,
            Phone = request.Phone,
            Email = request.Email,
            Rating = request.Rating
        };
    }

    private static UpdateContractorCommand MapToUpdateCommand(
        Guid contractorId,
        UpdateContractorRequest request)
    {
        return new UpdateContractorCommand
        {
            ContractorId = contractorId,
            Name = request.Name,
            JobTypeId = request.JobTypeId,
            BaseLocation = request.BaseLocation,
            Phone = request.Phone,
            Email = request.Email
        };
    }
}
