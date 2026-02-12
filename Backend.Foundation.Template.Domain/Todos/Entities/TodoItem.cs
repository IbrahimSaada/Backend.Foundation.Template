using Backend.Foundation.Template.Domain.Entities;
using Backend.Foundation.Template.Domain.Todos.ValueObjects;

namespace Backend.Foundation.Template.Domain.Todos.Entities;

// Aggregate entity for one todo task.
// Inherits ID, audit fields, and soft-delete behavior from shared base classes.
public sealed class TodoItem : SoftDeletableEntity
{
    // Value object backing field. Keeps validation/equality rules centralized.
    private TodoTitle _title = null!;

    // Convenience projection for consumers that need a plain string.
    public string Title => _title.Value;

    // Completion state is controlled only by Complete/Reopen methods.
    public bool IsCompleted { get; private set; }

    // Required by ORMs/serializers.
    private TodoItem()
    {
    }

    // Main constructor enforces invariants through TodoTitle value object.
    public TodoItem(string title)
    {
        SetTitle(title);
    }

    // Domain behavior for changing title while preserving validation rules.
    public void Rename(string title)
    {
        SetTitle(title);
    }

    // Marks todo as completed.
    public void Complete()
    {
        IsCompleted = true;
    }

    // Reopens a completed todo.
    public void Reopen()
    {
        IsCompleted = false;
    }

    // Central place for title conversion into value object.
    private void SetTitle(string title)
    {
        _title = TodoTitle.Create(title);
    }
}
