using Backend.Foundation.Template.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Foundation.Template.Security.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = PermissionPolicyName.Build(permission);
    }
}
