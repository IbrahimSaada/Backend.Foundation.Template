using Backend.Foundation.Template.Domain.ValueObjects;

namespace Backend.Foundation.Template.Domain.Todos.ValueObjects;

// Value object for todo title. Encapsulates all title rules in one reusable place.
public sealed class TodoTitle : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    private TodoTitle(string value)
    {
        Value = value;
    }

    public static TodoTitle Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Todo title is required.", nameof(value));
        }

        var normalized = value.Trim();
        if (normalized.Length > MaxLength)
        {
            throw new ArgumentException($"Todo title cannot exceed {MaxLength} characters.", nameof(value));
        }

        return new TodoTitle(normalized);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
