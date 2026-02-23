namespace Backend.Foundation.Template.Abstractions.Caching;

public sealed class CacheEntryOptions
{
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }
}
