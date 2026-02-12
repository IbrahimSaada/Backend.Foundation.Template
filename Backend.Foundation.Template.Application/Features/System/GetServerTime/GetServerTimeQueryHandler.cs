using Backend.Foundation.Template.Abstractions.Clock;
using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Handlers;

namespace Backend.Foundation.Template.Application.Features.System.GetServerTime;

public sealed class GetServerTimeQueryHandler : IQueryHandler<GetServerTimeQuery, DateTime>
{
    private readonly IClock _clock;

    public GetServerTimeQueryHandler(IClock clock)
    {
        _clock = clock;
    }

    public Task<Result<DateTime>> Handle(GetServerTimeQuery request, CancellationToken ct = default)
    {
        return Task.FromResult(Result<DateTime>.Success(_clock.UtcNow));
    }
}
