namespace Backend.Foundation.Template.Domain.Entities;

public abstract class SoftDeletableEntity : SoftDeletableEntity<Guid>
{
    protected SoftDeletableEntity()
    {
        Id = Guid.NewGuid();
    }
}
