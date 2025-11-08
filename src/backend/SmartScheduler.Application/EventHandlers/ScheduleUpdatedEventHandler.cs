using MediatR;
using Microsoft.Extensions.Logging;
using SmartScheduler.Domain.Events;

namespace SmartScheduler.Application.EventHandlers;

/// <summary>
/// Event handler for ScheduleUpdatedEvent that performs business logic when a contractor's schedule changes.
/// Currently logs the update. Can be extended to invalidate caches, notify affected jobs, etc.
/// </summary>
public class ScheduleUpdatedEventHandler : INotificationHandler<ScheduleUpdatedEvent>
{
    private readonly ILogger<ScheduleUpdatedEventHandler> _logger;

    public ScheduleUpdatedEventHandler(ILogger<ScheduleUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ScheduleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Contractor schedule updated: ContractorId={ContractorId}, ChangeDescription={ChangeDescription}",
            notification.ContractorId,
            notification.ChangeDescription ?? "No description");

        // Future enhancements:
        // - Invalidate availability cache for this contractor
        // - Re-calculate recommendations for pending jobs
        // - Notify dispatcher of schedule conflicts
        // - Trigger SignalR broadcast for real-time UI updates

        return Task.CompletedTask;
    }
}
