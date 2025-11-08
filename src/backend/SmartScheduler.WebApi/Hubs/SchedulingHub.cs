using Microsoft.AspNetCore.SignalR;

namespace SmartScheduler.WebApi.Hubs;

/// <summary>
/// SignalR hub for real-time scheduling updates.
/// Thin hub with no business logic - purely messaging infrastructure.
/// Broadcasts domain events to connected dispatcher and contractor clients.
/// </summary>
public class SchedulingHub : Hub<ISchedulingClient>
{
    private readonly ILogger<SchedulingHub> _logger;

    public SchedulingHub(ILogger<SchedulingHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var userIdentifier = Context.UserIdentifier ?? "Anonymous";
        var remoteIp = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString()
            ?? "Unknown";

        _logger.LogInformation(
            "SignalR client connected: ConnectionId={ConnectionId}, User={User}, RemoteIP={RemoteIP}",
            connectionId,
            userIdentifier,
            remoteIp);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var userIdentifier = Context.UserIdentifier ?? "Anonymous";

        if (exception != null)
        {
            _logger.LogWarning(
                exception,
                "SignalR client disconnected with error: ConnectionId={ConnectionId}, User={User}",
                connectionId,
                userIdentifier);
        }
        else
        {
            _logger.LogInformation(
                "SignalR client disconnected: ConnectionId={ConnectionId}, User={User}",
                connectionId,
                userIdentifier);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
