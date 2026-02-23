using Backend.Foundation.Template.Abstractions.Caching;
using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Caching;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Behaviors;

public sealed class QueryCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheStore _cacheStore;
    private readonly ICacheKeyFactory _cacheKeyFactory;

    public QueryCachingBehavior(
        ICacheStore cacheStore,
        ICacheKeyFactory cacheKeyFactory)
    {
        _cacheStore = cacheStore;
        _cacheKeyFactory = cacheKeyFactory;
    }

    public async Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        if (request is not ICacheableQuery<TResponse> cacheableQuery ||
            cacheableQuery.BypassCache)
        {
            return await next();
        }

        var cacheKey = _cacheKeyFactory.Create(cacheableQuery.CacheCategory, cacheableQuery.CacheKey);
        var cached = await _cacheStore.GetAsync<CachedQueryPayload<TResponse>>(cacheKey, ct);
        if (cached is not null)
        {
            return Result<TResponse>.Success(cached.Value);
        }

        var result = await next();
        if (result.IsFailure)
        {
            return result;
        }

        var cacheEntryOptions = new CacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheableQuery.AbsoluteExpirationRelativeToNow
        };

        await _cacheStore.SetAsync(
            cacheKey,
            new CachedQueryPayload<TResponse>(result.Value),
            cacheEntryOptions,
            ct);

        return result;
    }

    private sealed record CachedQueryPayload<T>(T Value);
}
