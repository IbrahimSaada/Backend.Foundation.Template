using Backend.Foundation.Template.Abstractions.Caching;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Backend.Foundation.Template.Infrastructure.Caching;

internal static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddTemplateCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));

        var options = configuration
                          .GetSection(RedisOptions.SectionName)
                          .Get<RedisOptions>() ??
                      new RedisOptions();

        if (!options.Enabled)
        {
            services.AddSingleton<ICacheKeyFactory, DefaultCacheKeyFactory>();
            services.AddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();
            services.AddSingleton<ICacheStore, NoOpCacheStore>();
            services.AddSingleton<IDistributedLock, NoOpDistributedLock>();
            services.AddSingleton<IIdempotencyStore, NoOpIdempotencyStore>();
            services.AddHealthChecks().AddCheck(
                "redis",
                () => HealthCheckResult.Healthy("Redis is disabled."),
                tags: new[] { "ready" });
            return services;
        }

        ValidateOptions(options);

        services.AddSingleton<ICacheKeyFactory, DefaultCacheKeyFactory>();
        services.AddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var resolvedOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            var configurationOptions = ConfigurationOptions.Parse(resolvedOptions.ConnectionString, true);
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.ConnectTimeout = Math.Max(1000, resolvedOptions.ConnectTimeoutMs);
            configurationOptions.AsyncTimeout = Math.Max(1000, resolvedOptions.ConnectTimeoutMs);

            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        services.AddSingleton<ICacheStore, RedisCacheStore>();
        services.AddSingleton<IDistributedLock, RedisDistributedLock>();
        services.AddSingleton<IIdempotencyStore, RedisIdempotencyStore>();
        services.AddHealthChecks().AddCheck<RedisHealthCheck>("redis", tags: new[] { "ready" });

        return services;
    }

    private static void ValidateOptions(RedisOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException(
                "Redis is enabled but Redis:ConnectionString is missing.");
        }

        if (options.DefaultTtlSeconds <= 0)
        {
            throw new InvalidOperationException(
                "Redis:DefaultTtlSeconds must be greater than zero.");
        }

        if (options.ConnectTimeoutMs <= 0)
        {
            throw new InvalidOperationException(
                "Redis:ConnectTimeoutMs must be greater than zero.");
        }
    }
}
