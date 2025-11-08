namespace SmartScheduler.WebApi.Hubs;

/// <summary>
/// Strongly-typed interface for SignalR client methods.
/// Defines methods that server can invoke on connected clients.
/// </summary>
public interface ISchedulingClient
{
    /// <summary>
    /// Called when a job is assigned to a contractor.
    /// </summary>
    /// <param name="jobId">ID of the assigned job.</param>
    /// <param name="formattedJobId">Human-readable job ID (e.g., "JOB-001").</param>
    /// <param name="contractorId">ID of the assigned contractor.</param>
    /// <param name="scheduledStartTime">When the job is scheduled to start.</param>
    Task ReceiveJobAssigned(
        Guid jobId,
        string formattedJobId,
        Guid contractorId,
        DateTime scheduledStartTime);

    /// <summary>
    /// Called when a contractor's schedule is updated.
    /// </summary>
    /// <param name="contractorId">ID of the contractor whose schedule changed.</param>
    /// <param name="changeDescription">Optional description of the change.</param>
    Task ReceiveScheduleUpdated(Guid contractorId, string? changeDescription);

    /// <summary>
    /// Called when a contractor's rating is updated.
    /// </summary>
    /// <param name="contractorId">ID of the contractor whose rating changed.</param>
    /// <param name="oldRating">Previous rating value (0.0-5.0).</param>
    /// <param name="newRating">New rating value (0.0-5.0).</param>
    /// <param name="relatedJobId">Optional job that triggered the rating update.</param>
    Task ReceiveContractorRated(
        Guid contractorId,
        decimal oldRating,
        decimal newRating,
        Guid? relatedJobId);
}
