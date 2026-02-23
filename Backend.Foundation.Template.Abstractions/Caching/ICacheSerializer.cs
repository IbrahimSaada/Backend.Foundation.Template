namespace Backend.Foundation.Template.Abstractions.Caching;

public interface ICacheSerializer
{
    byte[] Serialize<T>(T value);

    T? Deserialize<T>(ReadOnlyMemory<byte> payload);
}
