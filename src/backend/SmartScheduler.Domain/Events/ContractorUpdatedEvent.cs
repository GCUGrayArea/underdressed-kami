namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a contractor's details are updated.
/// </summary>
public class ContractorUpdatedEvent : DomainEvent
{
    public Guid ContractorId { get; }
    public string FormattedContractorId { get; }
    public string Name { get; }
    public Guid JobTypeId { get; }

    public ContractorUpdatedEvent(
        Guid contractorId,
        string formattedContractorId,
        string name,
        Guid jobTypeId)
    {
        ContractorId = contractorId;
        FormattedContractorId = formattedContractorId;
        Name = name;
        JobTypeId = jobTypeId;
    }
}
