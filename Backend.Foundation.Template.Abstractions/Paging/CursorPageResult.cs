namespace Backend.Foundation.Template.Abstractions.Paging;

public sealed record CursorPageResult<TItem>(
    IReadOnlyList<TItem> Items,
    string? NextCursor,
    int Returned,
    bool HasMore);
