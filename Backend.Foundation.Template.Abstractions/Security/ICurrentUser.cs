namespace Backend.Foundation.Template.Abstractions.Security;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string? SubjectId { get; }

    string? UserName { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Permissions { get; }
}
