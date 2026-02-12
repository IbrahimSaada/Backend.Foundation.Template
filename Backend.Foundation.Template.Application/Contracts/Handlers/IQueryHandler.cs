using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Contracts.Handlers;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
