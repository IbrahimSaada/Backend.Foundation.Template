namespace Backend.Foundation.Template.Application.Contracts.Requests;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}

public interface ICommand : ICommand<Unit>
{
}
