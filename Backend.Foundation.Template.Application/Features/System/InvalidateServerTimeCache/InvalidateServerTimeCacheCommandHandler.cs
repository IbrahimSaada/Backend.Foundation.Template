using Backend.Foundation.Template.Abstractions.Messaging;
using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Handlers;
using Backend.Foundation.Template.Application.Contracts.Requests;
using Backend.Foundation.Template.Application.Features.System.GetServerTime;

namespace Backend.Foundation.Template.Application.Features.System.InvalidateServerTimeCache;

public sealed class InvalidateServerTimeCacheCommandHandler : ICommandHandler<InvalidateServerTimeCacheCommand>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public InvalidateServerTimeCacheCommandHandler(IIntegrationEventPublisher integrationEventPublisher)
    {
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task<Result<Unit>> Handle(InvalidateServerTimeCacheCommand request, CancellationToken ct = default)
    {
        await _integrationEventPublisher.PublishAsync(
            new ServerTimeCacheInvalidatedIntegrationEvent(GetServerTimeQuery.CacheKeyValue),
            ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
