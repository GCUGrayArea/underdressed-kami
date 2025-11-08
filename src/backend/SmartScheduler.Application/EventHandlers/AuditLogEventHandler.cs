using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Events;
using SmartScheduler.Domain.Interfaces;

namespace SmartScheduler.Application.EventHandlers;

/// <summary>
/// Event handler that logs ALL domain events to the audit log.
/// This creates a complete audit trail of all domain events in the system.
/// Handles any event that inherits from DomainEvent.
/// </summary>
public class AuditLogEventHandler :
    INotificationHandler<JobAssignedEvent>,
    INotificationHandler<JobCreatedEvent>,
    INotificationHandler<ContractorCreatedEvent>,
    INotificationHandler<ContractorUpdatedEvent>,
    INotificationHandler<ContractorDeactivatedEvent>,
    INotificationHandler<ScheduleUpdatedEvent>,
    INotificationHandler<ContractorRatedEvent>
{
    private readonly IDomainEventLogRepository _eventLogRepository;
    private readonly ILogger<AuditLogEventHandler> _logger;

    public AuditLogEventHandler(
        IDomainEventLogRepository eventLogRepository,
        ILogger<AuditLogEventHandler> logger)
    {
        _eventLogRepository = eventLogRepository;
        _logger = logger;
    }

    public async Task Handle(JobAssignedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    public async Task Handle(JobCreatedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    public async Task Handle(ContractorCreatedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    public async Task Handle(ContractorUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    public async Task Handle(ContractorDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    public async Task Handle(ScheduleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    public async Task Handle(ContractorRatedEvent notification, CancellationToken cancellationToken)
    {
        await LogEventAsync(notification, cancellationToken);
    }

    private async Task LogEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            var eventType = domainEvent.GetType().Name;
            var eventData = JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            var eventLog = new DomainEventLog(
                domainEvent.EventId,
                eventType,
                eventData,
                domainEvent.OccurredAt);

            await _eventLogRepository.AddAsync(eventLog, cancellationToken);

            _logger.LogInformation(
                "Domain event logged: {EventType} (EventId: {EventId})",
                eventType,
                domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to log domain event: {EventType} (EventId: {EventId})",
                domainEvent.GetType().Name,
                domainEvent.EventId);
            // Don't throw - audit logging should not break business logic
        }
    }
}
