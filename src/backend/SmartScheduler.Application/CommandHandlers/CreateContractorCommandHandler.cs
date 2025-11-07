using MediatR;
using SmartScheduler.Application.Commands;
using SmartScheduler.Application.Common;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Events;
using SmartScheduler.Domain.Interfaces;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Application.CommandHandlers;

/// <summary>
/// Handles the creation of a new contractor.
/// </summary>
public class CreateContractorCommandHandler
    : IRequestHandler<CreateContractorCommand, Result<Guid>>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPublisher _publisher;

    public CreateContractorCommandHandler(
        IContractorRepository contractorRepository,
        IPublisher publisher)
    {
        _contractorRepository = contractorRepository;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(
        CreateContractorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get next sequence number for formatted ID
            var sequenceNumber = await _contractorRepository
                .GetNextSequenceNumberAsync(cancellationToken);
            var formattedId = $"CTR-{sequenceNumber:D3}";

            // Create location value object
            var location = new Location(
                request.BaseLocation.Latitude,
                request.BaseLocation.Longitude,
                request.BaseLocation.Address);

            // Create contractor entity
            var contractor = new Contractor(
                formattedId,
                request.Name,
                request.JobTypeId,
                location,
                request.Phone,
                request.Email,
                request.Rating);

            // Save to repository
            await _contractorRepository.AddAsync(contractor, cancellationToken);

            // Publish domain event
            var domainEvent = new ContractorCreatedEvent(
                contractor.Id,
                contractor.FormattedId,
                contractor.Name,
                contractor.JobTypeId);

            await _publisher.Publish(domainEvent, cancellationToken);

            return Result.Success(contractor.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(
                $"Failed to create contractor: {ex.Message}");
        }
    }
}
