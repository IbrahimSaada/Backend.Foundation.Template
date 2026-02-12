using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Behaviors;

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default);
}
