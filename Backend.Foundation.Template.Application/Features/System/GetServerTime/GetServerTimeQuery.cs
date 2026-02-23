using Backend.Foundation.Template.Abstractions.Caching;
using Backend.Foundation.Template.Application.Contracts.Caching;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Features.System.GetServerTime;

public sealed record GetServerTimeQuery(bool BypassCache = false) : ICacheableQuery<DateTime>
{
    public const string CacheKeyValue = "system.time.utc";

    public string CacheKey => CacheKeyValue;

    public string CacheCategory => CacheCategories.Query;

    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromSeconds(30);
}
