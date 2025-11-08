namespace SmartScheduler.Domain.Events;

/// <summary>
/// Domain event published when a contractor's rating is updated.
/// This affects scoring calculations for future recommendations.
/// </summary>
public class ContractorRatedEvent : DomainEvent
{
    /// <summary>
    /// ID of the contractor whose rating was updated.
    /// </summary>
    public Guid ContractorId { get; }

    /// <summary>
    /// Previous rating value (0.0-5.0).
    /// </summary>
    public decimal OldRating { get; }

    /// <summary>
    /// New rating value (0.0-5.0).
    /// </summary>
    public decimal NewRating { get; }

    /// <summary>
    /// Optional ID of the job that triggered the rating update.
    /// </summary>
    public Guid? RelatedJobId { get; }

    public ContractorRatedEvent(
        Guid contractorId,
        decimal oldRating,
        decimal newRating,
        Guid? relatedJobId = null)
    {
        ContractorId = contractorId;
        OldRating = oldRating;
        NewRating = newRating;
        RelatedJobId = relatedJobId;
    }
}
