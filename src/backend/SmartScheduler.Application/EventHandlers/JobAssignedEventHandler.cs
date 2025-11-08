using MediatR;
using Microsoft.Extensions.Logging;
using SmartScheduler.Domain.Events;

namespace SmartScheduler.Application.EventHandlers;

/// <summary>
/// Event handler for JobAssignedEvent that performs business logic when a job is assigned.
/// Currently logs the assignment. Can be extended to trigger notifications, analytics, etc.
/// </summary>
public class JobAssignedEventHandler : INotificationHandler<JobAssignedEvent>
{
    private readonly ILogger<JobAssignedEventHandler> _logger;

    public JobAssignedEventHandler(ILogger<JobAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(JobAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Job assigned: JobId={JobId}, ContractorId={ContractorId}, ScheduledStart={ScheduledStart}",
            notification.JobId,
            notification.ContractorId,
            notification.ScheduledStartTime);

        // Future enhancements:
        // - Send notification to contractor via email/SMS
        // - Update analytics/reporting database
        // - Trigger SignalR broadcast to connected clients
        // - Invalidate availability cache for this contractor

        return Task.CompletedTask;
    }
}
