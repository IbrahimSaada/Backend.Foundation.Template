namespace Backend.Foundation.Template.Abstractions.Caching;

public interface ICacheKeyFactory
{
    string Create(string category, params object?[] segments);
}
