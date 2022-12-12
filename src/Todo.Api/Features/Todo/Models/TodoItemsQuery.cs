namespace Todo.Api.Features.Todo.Models;

public class TodoItemsQuery
{
    /// <summary> Page number filter. Default 1 </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary> Page size filter. Default 10 </summary>
    public int PageSize { get; set; } = 10;
}