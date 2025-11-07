namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a contractor is deactivated (soft delete).
/// </summary>
public class ContractorDeactivatedEvent : DomainEvent
{
    public Guid ContractorId { get; }
    public string FormattedContractorId { get; }

    public ContractorDeactivatedEvent(
        Guid contractorId,
        string formattedContractorId)
    {
        ContractorId = contractorId;
        FormattedContractorId = formattedContractorId;
    }
}
