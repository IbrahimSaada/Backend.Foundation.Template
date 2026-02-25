using Backend.Foundation.Template.Abstractions.Observability;

namespace Backend.Foundation.Template.Infrastructure.Observability;

internal static class ObservabilityServiceCollectionExtensions
{
    public static IServiceCollection AddTemplateObservability(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpContextAccessor();
        services.AddScoped<ICorrelationContext, HttpCorrelationContext>();

        return services;
    }
}
