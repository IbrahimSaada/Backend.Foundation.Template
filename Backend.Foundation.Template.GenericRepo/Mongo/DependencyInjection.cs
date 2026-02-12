using Backend.Foundation.Template.Abstractions.Clock;
using Backend.Foundation.Template.Abstractions.Persistence;
using Backend.Foundation.Template.GenericRepo.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace Backend.Foundation.Template.GenericRepo.Mongo;

public static class DependencyInjection
{
    public static IServiceCollection AddGenericMongoRepositories(
        this IServiceCollection services,
        IMongoDatabase database)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(database);

        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddSingleton(database);
        services.TryAddSingleton<IMongoCollectionNameResolver, DefaultMongoCollectionNameResolver>();

        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
        services.AddScoped(typeof(IRepository<,>), typeof(MongoRepository<,>));

        return services;
    }
}
