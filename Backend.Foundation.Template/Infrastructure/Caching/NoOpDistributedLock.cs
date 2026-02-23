using Backend.Foundation.Template.Abstractions.Caching;

namespace Backend.Foundation.Template.Infrastructure.Caching;

internal sealed class NoOpDistributedLock : IDistributedLock
{
    public Task<IAsyncDisposable?> TryAcquireAsync(
        string key,
        TimeSpan leaseTime,
        CancellationToken ct = default)
    {
        return Task.FromResult<IAsyncDisposable?>(null);
    }
}
