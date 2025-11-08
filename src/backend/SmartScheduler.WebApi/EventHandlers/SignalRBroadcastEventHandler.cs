using MediatR;
using Microsoft.AspNetCore.SignalR;
using SmartScheduler.Domain.Events;
using SmartScheduler.WebApi.Hubs;

namespace SmartScheduler.WebApi.EventHandlers;

/// <summary>
/// Event handler that broadcasts domain events to all connected SignalR clients.
/// Implements fire-and-forget pattern - broadcast failures don't fail event handling.
/// </summary>
public class SignalRBroadcastEventHandler :
    INotificationHandler<JobAssignedEvent>,
    INotificationHandler<ScheduleUpdatedEvent>,
    INotificationHandler<ContractorRatedEvent>
{
    private readonly IHubContext<SchedulingHub, ISchedulingClient> _hubContext;
    private readonly ILogger<SignalRBroadcastEventHandler> _logger;

    public SignalRBroadcastEventHandler(
        IHubContext<SchedulingHub, ISchedulingClient> hubContext,
        ILogger<SignalRBroadcastEventHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(JobAssignedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveJobAssigned(
                notification.JobId,
                notification.FormattedJobId,
                notification.ContractorId,
                notification.ScheduledStartTime);

            _logger.LogInformation(
                "SignalR broadcast sent: JobAssigned (EventId: {EventId}, JobId: {JobId})",
                notification.EventId,
                notification.FormattedJobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to broadcast JobAssigned event via SignalR (EventId: {EventId}). " +
                "This is non-fatal - event processing continues.",
                notification.EventId);
            // Don't throw - SignalR broadcast failures should not fail event processing
        }
    }

    public async Task Handle(ScheduleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveScheduleUpdated(
                notification.ContractorId,
                notification.ChangeDescription);

            _logger.LogInformation(
                "SignalR broadcast sent: ScheduleUpdated (EventId: {EventId}, ContractorId: {ContractorId})",
                notification.EventId,
                notification.ContractorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to broadcast ScheduleUpdated event via SignalR (EventId: {EventId}). " +
                "This is non-fatal - event processing continues.",
                notification.EventId);
        }
    }

    public async Task Handle(ContractorRatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveContractorRated(
                notification.ContractorId,
                notification.OldRating,
                notification.NewRating,
                notification.RelatedJobId);

            _logger.LogInformation(
                "SignalR broadcast sent: ContractorRated (EventId: {EventId}, ContractorId: {ContractorId}, " +
                "OldRating: {OldRating}, NewRating: {NewRating})",
                notification.EventId,
                notification.ContractorId,
                notification.OldRating,
                notification.NewRating);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to broadcast ContractorRated event via SignalR (EventId: {EventId}). " +
                "This is non-fatal - event processing continues.",
                notification.EventId);
        }
    }
}
