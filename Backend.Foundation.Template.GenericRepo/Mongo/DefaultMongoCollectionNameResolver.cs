using System.Reflection;

namespace Backend.Foundation.Template.GenericRepo.Mongo;

public sealed class DefaultMongoCollectionNameResolver : IMongoCollectionNameResolver
{
    public string Resolve<TEntity>()
    {
        var attribute = typeof(TEntity).GetCustomAttribute<MongoCollectionAttribute>();
        return attribute?.Name ?? typeof(TEntity).Name;
    }
}
