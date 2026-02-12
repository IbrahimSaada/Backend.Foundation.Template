namespace Backend.Foundation.Template.Application.Contracts.Validation;

public sealed record ValidationFailure(
    string PropertyName,
    string ErrorMessage,
    string? ErrorCode = null);
