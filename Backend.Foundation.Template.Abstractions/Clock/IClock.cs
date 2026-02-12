namespace Backend.Foundation.Template.Abstractions.Clock;

public interface IClock
{
    DateTime UtcNow { get; }
}
