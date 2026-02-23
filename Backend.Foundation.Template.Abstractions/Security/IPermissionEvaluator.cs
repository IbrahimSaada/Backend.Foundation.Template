namespace Backend.Foundation.Template.Abstractions.Security;

public interface IPermissionEvaluator
{
    Task<bool> HasPermissionAsync(
        ICurrentUser currentUser,
        string permission,
        CancellationToken ct = default);
}
