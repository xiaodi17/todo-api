using Microsoft.AspNetCore.Mvc;
using Todo.Api.Features.Todo.Models;
using Todo.Api.Infrastructure;
using ILogger = Serilog.ILogger;

namespace Todo.Api.Features.Todo;

[ApiController]
public class TodoController : ControllerBase
{
    private static readonly TodoItem[] TodoItems = new[]
    {
         new TodoItem
         {
             Id = new Guid(),
             Name = "Clean"
         },
         new TodoItem
         {
             Id = new Guid(),
             Name = "Study"
         }
    };
    private readonly ILogger _logger;
    private readonly TodoService _todoService;

    public TodoController(ILogger logger, TodoService todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    [HttpGet("todoitems")]
    public IEnumerable<TodoItem> Get()
    {
        return TodoItems;
    }
    
    [HttpPost("todoitems")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateTodoItemRequest request)
    {
        TodoItem? todoItem = default;

        try
        {
            todoItem = await _todoService.Create(request);
        }
        catch (DuplicateException)
        {
            // intentionally swallow the exception
        }


        return Created($"todo/{todoItem.Id}", todoItem);
    }
}