using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Errors;
using Backend.Foundation.Template.Application.Contracts.Requests;
using Backend.Foundation.Template.Application.Contracts.Validation;

namespace Backend.Foundation.Template.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IReadOnlyList<IRequestValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IRequestValidator<TRequest>> validators)
    {
        _validators = validators.ToArray();
    }

    public async Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        if (_validators.Count == 0)
        {
            return await next();
        }

        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, ct);
            if (!result.IsValid)
            {
                failures.AddRange(result.Errors);
            }
        }

        if (failures.Count == 0)
        {
            return await next();
        }

        var details = string.Join("; ", failures.Select(x =>
            string.IsNullOrWhiteSpace(x.PropertyName)
                ? x.ErrorMessage
                : $"{x.PropertyName}: {x.ErrorMessage}"));

        return Result<TResponse>.Failure(ApplicationErrors.Validation(details));
    }
}
