using Backend.Foundation.Template.Abstractions.Caching;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Contracts.Caching;

public interface ICacheableQuery<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }

    string CacheCategory => CacheCategories.Query;

    TimeSpan? AbsoluteExpirationRelativeToNow => null;

    bool BypassCache => false;
}
