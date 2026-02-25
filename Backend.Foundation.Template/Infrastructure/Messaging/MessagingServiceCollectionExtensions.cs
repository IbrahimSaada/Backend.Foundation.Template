using Backend.Foundation.Template.Abstractions.Messaging;
using Backend.Foundation.Template.Application.Features.System.InvalidateServerTimeCache;
using Backend.Foundation.Template.Infrastructure.Messaging.Consumers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backend.Foundation.Template.Infrastructure.Messaging;

internal static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddTemplateMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var provider = configuration[$"{MessagingOptions.SectionName}:Provider"];

        services.Configure<MessagingOptions>(configuration.GetSection(MessagingOptions.SectionName));
        services.Configure<OutboxDispatcherOptions>(configuration.GetSection(OutboxDispatcherOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.TryAddScoped<IOutboxStore, NoOpOutboxStore>();
        services.AddSingleton<IOutboxSerializer, SystemTextJsonOutboxSerializer>();
        services.AddScoped<IIntegrationEventPublisher, OutboxIntegrationEventPublisher>();
        services.AddScoped<IIntegrationEventConsumer<ServerTimeCacheInvalidatedIntegrationEvent>, ServerTimeCacheInvalidatedIntegrationEventConsumer>();

        if (string.Equals(provider, "RabbitMq", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        }
        else
        {
            services.AddSingleton<IMessageBus, NoOpMessageBus>();
        }

        services.AddHostedService<OutboxDispatcherHostedService>();
        services.AddHostedService<RabbitMqConsumerHostedService>();
        services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: new[] { "ready" });

        return services;
    }
}
