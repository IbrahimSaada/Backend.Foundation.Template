namespace Backend.Foundation.Template.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOnUtc { get; }
}
