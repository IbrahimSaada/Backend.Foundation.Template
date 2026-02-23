using Microsoft.AspNetCore.Authorization;

namespace Backend.Foundation.Template.Security.Authorization;

internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException("Permission is required.", nameof(permission));
        }

        Permission = permission.Trim();
    }

    public string Permission { get; }
}
