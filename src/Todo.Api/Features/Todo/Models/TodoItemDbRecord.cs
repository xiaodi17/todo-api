namespace Todo.Api.Features.Todo.Models;

public class TodoItemDbRecord
{
    public Guid TodoId { get; set; }
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}