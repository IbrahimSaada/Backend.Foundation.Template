using Backend.Foundation.Template.Abstractions.Clock;

namespace Backend.Foundation.Template.Infrastructure;

internal sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
