using System.Security.Claims;

namespace Backend.Foundation.Template.Security.Configuration;

public sealed class AuthorizationMappingOptions
{
    public const string SectionName = "AuthorizationMapping";

    public bool IncludeKeycloakRealmRoles { get; set; } = true;

    public List<string> RoleClaimTypes { get; set; } = new()
    {
        ClaimTypes.Role,
        "role",
        "roles"
    };

    public List<string> PermissionClaimTypes { get; set; } = new()
    {
        "permission",
        "permissions",
        "scope",
        "scp"
    };

    public Dictionary<string, string[]> RolePermissions { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}
