using MediatR;
using SmartScheduler.Application.Commands;
using SmartScheduler.Application.Common;
using SmartScheduler.Domain.Events;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.CommandHandlers;

/// <summary>
/// Handles updating an existing contractor's details.
/// </summary>
public class UpdateContractorCommandHandler
    : IRequestHandler<UpdateContractorCommand, Result>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPublisher _publisher;

    public UpdateContractorCommandHandler(
        IContractorRepository contractorRepository,
        IPublisher publisher)
    {
        _contractorRepository = contractorRepository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(
        UpdateContractorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve contractor
            var contractor = await _contractorRepository
                .GetByIdAsync(request.ContractorId, cancellationToken);

            if (contractor == null)
                return Result.Failure("Contractor not found");

            // Create location value object
            var location = new Location(
                request.BaseLocation.Latitude,
                request.BaseLocation.Longitude,
                request.BaseLocation.Address);

            // Update contractor details
            contractor.UpdateDetails(
                request.Name,
                request.JobTypeId,
                location,
                request.Phone,
                request.Email);

            // Save changes
            await _contractorRepository
                .UpdateAsync(contractor, cancellationToken);

            // Publish domain event
            var domainEvent = new ContractorUpdatedEvent(
                contractor.Id,
                contractor.FormattedId,
                contractor.Name,
                contractor.JobTypeId);

            await _publisher.Publish(domainEvent, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"Failed to update contractor: {ex.Message}");
        }
    }
}
