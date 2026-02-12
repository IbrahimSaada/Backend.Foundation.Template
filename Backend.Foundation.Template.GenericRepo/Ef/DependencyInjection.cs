using Backend.Foundation.Template.Abstractions.Clock;
using Backend.Foundation.Template.Abstractions.Persistence;
using Backend.Foundation.Template.GenericRepo.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backend.Foundation.Template.GenericRepo.Ef;

public static class DependencyInjection
{
    public static IServiceCollection AddGenericEfRepositories<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IClock, SystemClock>();
        services.AddScoped<DbContext>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

        return services;
    }
}
