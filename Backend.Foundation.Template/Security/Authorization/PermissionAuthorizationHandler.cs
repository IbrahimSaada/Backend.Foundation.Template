using Backend.Foundation.Template.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Foundation.Template.Security.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionEvaluator _permissionEvaluator;

    public PermissionAuthorizationHandler(
        ICurrentUser currentUser,
        IPermissionEvaluator permissionEvaluator)
    {
        _currentUser = currentUser;
        _permissionEvaluator = permissionEvaluator;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission = await _permissionEvaluator.HasPermissionAsync(
            _currentUser,
            requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
