namespace Todo.Api.Features.Todo.Models;

public class TodoItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}