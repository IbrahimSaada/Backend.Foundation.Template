using System.Collections.Concurrent;
using System.Reflection;
using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Behaviors;
using Backend.Foundation.Template.Application.Contracts.Handlers;
using Backend.Foundation.Template.Application.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Foundation.Template.Application.Dispatching;

internal sealed class RequestDispatcher : IRequestDispatcher
{
    private static readonly MethodInfo DispatchTypedMethodDefinition =
        typeof(RequestDispatcher).GetMethod(nameof(DispatchTypedAsync), BindingFlags.Instance | BindingFlags.NonPublic) ??
        throw new InvalidOperationException("Dispatch method metadata was not found.");

    private static readonly ConcurrentDictionary<(Type RequestType, Type ResponseType), MethodInfo> DispatchMethodCache = new();

    private readonly IServiceProvider _serviceProvider;

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<Result<TResponse>> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var key = (request.GetType(), typeof(TResponse));
        var dispatchMethod = DispatchMethodCache.GetOrAdd(
            key,
            static value => DispatchTypedMethodDefinition.MakeGenericMethod(value.RequestType, value.ResponseType));

        return (Task<Result<TResponse>>)dispatchMethod.Invoke(this, new object[] { request, ct })!;
    }

    private async Task<Result<TResponse>> DispatchTypedAsync<TRequest, TResponse>(
        object requestObject,
        CancellationToken ct)
        where TRequest : IRequest<TResponse>
    {
        var request = (TRequest)requestObject;
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToArray();

        RequestHandlerDelegate<TResponse> next = () => handler.Handle(request, ct);

        foreach (var behavior in behaviors)
        {
            var currentNext = next;
            next = () => behavior.Handle(request, currentNext, ct);
        }

        return await next();
    }
}
