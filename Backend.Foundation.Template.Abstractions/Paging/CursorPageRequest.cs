namespace Backend.Foundation.Template.Abstractions.Paging;

public sealed record CursorPageRequest(
    int Limit = 50,
    string? Cursor = null);
