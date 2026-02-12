using Backend.Foundation.Template.Abstractions.Clock;

namespace Backend.Foundation.Template.GenericRepo.Common;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
