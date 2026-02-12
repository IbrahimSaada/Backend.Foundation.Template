using Backend.Foundation.Template.Abstractions.Results;

namespace Backend.Foundation.Template.Application.Behaviors;

public delegate Task<Result<TResponse>> RequestHandlerDelegate<TResponse>();
