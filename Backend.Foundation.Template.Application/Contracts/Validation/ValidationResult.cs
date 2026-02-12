namespace Backend.Foundation.Template.Application.Contracts.Validation;

public sealed class ValidationResult
{
    private ValidationResult(IReadOnlyList<ValidationFailure> errors)
    {
        Errors = errors;
    }

    public IReadOnlyList<ValidationFailure> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    public static ValidationResult Success()
    {
        return new ValidationResult(Array.Empty<ValidationFailure>());
    }

    public static ValidationResult Failure(IEnumerable<ValidationFailure> errors)
    {
        var materialized = errors?.ToArray() ?? Array.Empty<ValidationFailure>();
        return new ValidationResult(materialized);
    }
}
