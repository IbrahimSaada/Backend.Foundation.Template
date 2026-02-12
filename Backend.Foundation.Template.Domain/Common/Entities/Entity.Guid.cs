namespace Backend.Foundation.Template.Domain.Entities;

public abstract class Entity : Entity<Guid>
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }
}
