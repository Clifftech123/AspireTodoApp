namespace AspireTodoApp.Server.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult<TDto> MapTo<TDto>(Func<T, TDto> map) => new()
    {
        Items = Items.Select(map).ToList(),
        TotalCount = TotalCount,
        Page = Page,
        PageSize = PageSize
    };
}
