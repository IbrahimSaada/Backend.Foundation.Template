using Backend.Foundation.Template.Abstractions.Messaging;

namespace Backend.Foundation.Template.Application.Features.System.InvalidateServerTimeCache;

public sealed record ServerTimeCacheInvalidatedIntegrationEvent(string CacheKey) : IntegrationEventBase;
