using Backend.Foundation.Template.Abstractions.Persistence;
using Backend.Foundation.Template.Abstractions.Results;
using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        if (request is not ICommand<TResponse>)
        {
            return await next();
        }

        Result<TResponse>? result = null;

        await _unitOfWork.ExecuteInTransactionAsync(async txCt =>
        {
            result = await next();

            if (result.IsFailure)
            {
                return false;
            }

            await _unitOfWork.SaveChangesAsync(txCt);
            return true;
        }, ct);

        return result!;
    }
}
