using Backend.Foundation.Template.Abstractions.Caching;
using Backend.Foundation.Template.Application.Contracts.Caching;
using Backend.Foundation.Template.Application.Contracts.Requests;
using Backend.Foundation.Template.Application.Features.System.GetServerTime;

namespace Backend.Foundation.Template.Application.Features.System.InvalidateServerTimeCache;

public sealed record InvalidateServerTimeCacheCommand : ICommand, IInvalidatesCache
{
    public IReadOnlyCollection<CacheInvalidationItem> CacheInvalidationItems =>
        new[]
        {
            new CacheInvalidationItem(CacheCategories.Query, GetServerTimeQuery.CacheKeyValue)
        };
}
