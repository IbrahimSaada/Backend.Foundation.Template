using Backend.Foundation.Template.Abstractions.Caching;
using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Caching;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheStore _cacheStore;
    private readonly ICacheKeyFactory _cacheKeyFactory;

    public CacheInvalidationBehavior(
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
        var result = await next();

        if (result.IsFailure ||
            request is not ICommand<TResponse> ||
            request is not IInvalidatesCache invalidatingRequest)
        {
            return result;
        }

        if (invalidatingRequest.CacheInvalidationItems.Count == 0)
        {
            return result;
        }

        var distinctItems = invalidatingRequest.CacheInvalidationItems
            .Where(item =>
                !string.IsNullOrWhiteSpace(item.Category) &&
                !string.IsNullOrWhiteSpace(item.Key))
            .Distinct()
            .ToArray();

        foreach (var item in distinctItems)
        {
            var cacheKey = _cacheKeyFactory.Create(item.Category, item.Key);
            await _cacheStore.RemoveAsync(cacheKey, ct);
        }

        return result;
    }
}
