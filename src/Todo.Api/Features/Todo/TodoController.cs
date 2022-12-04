using Microsoft.AspNetCore.Mvc;
using Todo.Api.Features.Todo.Models;
using ILogger = Serilog.ILogger;

namespace Todo.Api.Features.Todo;

[ApiController]
public class TodoController : ControllerBase
{
    private static readonly TodoItem[] TodoItems = new[]
    {
         new TodoItem
         {
             Id = 1,
             Name = "Clean"
         },
         new TodoItem
         {
             Id = 2,
             Name = "Study"
         }
    };
    private readonly ILogger _logger;

    public TodoController(ILogger logger)
    {
        _logger = logger;
    }

    [HttpGet("todoitems")]
    public IEnumerable<TodoItem> Get()
    {
        return TodoItems;
    }
}