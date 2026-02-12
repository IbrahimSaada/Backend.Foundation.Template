using System.Reflection;
using Backend.Foundation.Template.Application.Behaviors;
using Backend.Foundation.Template.Application.Contracts.Handlers;
using Backend.Foundation.Template.Application.Contracts.Validation;
using Backend.Foundation.Template.Application.Dispatching;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Foundation.Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var assembly = typeof(DependencyInjection).Assembly;

        RegisterClosedGenericImplementations(services, assembly, typeof(IRequestHandler<,>));
        RegisterClosedGenericImplementations(services, assembly, typeof(IRequestValidator<>));

        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        return services;
    }

    private static void RegisterClosedGenericImplementations(
        IServiceCollection services,
        Assembly assembly,
        Type openGenericServiceType)
    {
        var candidates = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && !type.IsGenericTypeDefinition)
            .ToArray();

        foreach (var implementationType in candidates)
        {
            var serviceTypes = implementationType
                .GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == openGenericServiceType)
                .ToArray();

            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType);
            }
        }
    }
}
