using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Dispatching;

public interface IRequestDispatcher
{
    Task<Result<TResponse>> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default);
}
