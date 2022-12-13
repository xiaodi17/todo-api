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

    /// <summary>
    /// Get a list of todoitems with optional query filters
    /// </summary>
    /// <returns></returns>
    [HttpGet("todoitems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<TodoItem>> Get([FromQuery] TodoItemsQuery query)
    {
        var response = await _todoService.Get(query);
        return response;
    }
    
    [HttpPost("todoitems")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    
    [HttpGet("healthcheck")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Get()
    {
        return Ok();
    }
}