using MediatR;
using SmartScheduler.Application.Commands;
using SmartScheduler.Application.Common;
using SmartScheduler.Domain.Events;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.CommandHandlers;

/// <summary>
/// Handles deactivating a contractor (soft delete).
/// </summary>
public class DeactivateContractorCommandHandler
    : IRequestHandler<DeactivateContractorCommand, Result>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPublisher _publisher;

    public DeactivateContractorCommandHandler(
        IContractorRepository contractorRepository,
        IPublisher publisher)
    {
        _contractorRepository = contractorRepository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(
        DeactivateContractorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve contractor
            var contractor = await _contractorRepository
                .GetByIdAsync(request.ContractorId, cancellationToken);

            if (contractor == null)
                return Result.Failure("Contractor not found");

            // Deactivate contractor
            contractor.Deactivate();

            // Save changes
            await _contractorRepository
                .UpdateAsync(contractor, cancellationToken);

            // Publish domain event
            var domainEvent = new ContractorDeactivatedEvent(
                contractor.Id,
                contractor.FormattedId);

            await _publisher.Publish(domainEvent, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                $"Failed to deactivate contractor: {ex.Message}");
        }
    }
}
