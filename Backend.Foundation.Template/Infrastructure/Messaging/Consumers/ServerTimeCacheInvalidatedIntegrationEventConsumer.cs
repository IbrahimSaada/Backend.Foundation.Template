using Backend.Foundation.Template.Abstractions.Messaging;
using Backend.Foundation.Template.Application.Features.System.InvalidateServerTimeCache;

namespace Backend.Foundation.Template.Infrastructure.Messaging.Consumers;

internal sealed class ServerTimeCacheInvalidatedIntegrationEventConsumer
    : IIntegrationEventConsumer<ServerTimeCacheInvalidatedIntegrationEvent>
{
    private readonly ILogger<ServerTimeCacheInvalidatedIntegrationEventConsumer> _logger;

    public ServerTimeCacheInvalidatedIntegrationEventConsumer(
        ILogger<ServerTimeCacheInvalidatedIntegrationEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task ConsumeAsync(ServerTimeCacheInvalidatedIntegrationEvent integrationEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Consumed integration event {EventType}. CacheKey={CacheKey}, EventId={EventId}",
            nameof(ServerTimeCacheInvalidatedIntegrationEvent),
            integrationEvent.CacheKey,
            integrationEvent.EventId);

        return Task.CompletedTask;
    }
}
