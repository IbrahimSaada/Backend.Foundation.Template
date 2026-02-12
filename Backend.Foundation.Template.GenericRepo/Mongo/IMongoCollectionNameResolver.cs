namespace Backend.Foundation.Template.GenericRepo.Mongo;

public interface IMongoCollectionNameResolver
{
    string Resolve<TEntity>();
}
