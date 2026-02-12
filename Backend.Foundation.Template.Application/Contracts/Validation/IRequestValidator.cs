namespace Backend.Foundation.Template.Application.Contracts.Validation;

public interface IRequestValidator<in TRequest>
{
    Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken ct = default);
}
