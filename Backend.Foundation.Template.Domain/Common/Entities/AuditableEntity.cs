namespace Backend.Foundation.Template.Domain.Entities;

public abstract class AuditableEntity<TKey> : Entity<TKey> where TKey : notnull
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
