namespace Backend.Foundation.Template.Domain.Entities;

public abstract class AggregateRoot : AggregateRoot<Guid>
{
    protected AggregateRoot()
    {
        Id = Guid.NewGuid();
    }
}
