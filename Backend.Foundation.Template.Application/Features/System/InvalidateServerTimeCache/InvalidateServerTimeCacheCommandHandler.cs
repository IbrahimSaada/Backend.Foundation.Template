using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Handlers;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Features.System.InvalidateServerTimeCache;

public sealed class InvalidateServerTimeCacheCommandHandler : ICommandHandler<InvalidateServerTimeCacheCommand>
{
    public Task<Result<Unit>> Handle(InvalidateServerTimeCacheCommand request, CancellationToken ct = default)
    {
        return Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
}
