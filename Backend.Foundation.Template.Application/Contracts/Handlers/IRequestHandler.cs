using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Contracts.Handlers;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken ct = default);
}
