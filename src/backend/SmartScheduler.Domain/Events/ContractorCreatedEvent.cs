namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a new contractor is created.
/// </summary>
public class ContractorCreatedEvent : DomainEvent
{
    public Guid ContractorId { get; }
    public string FormattedContractorId { get; }
    public string Name { get; }
    public Guid JobTypeId { get; }

    public ContractorCreatedEvent(
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
