using Todo.Api.Databases;
using Todo.Api.Features.Todo.Models;

namespace Todo.Api.Features.Todo;

public class TodoService
{
    private readonly TodoRepository _todoRepository;
    private readonly TodoDatabase _todoDatabase;

    public TodoService(TodoRepository todoRepository, TodoDatabase todoDatabase)
    {
        _todoRepository = todoRepository;
        _todoDatabase = todoDatabase;
    }

    public async Task<TodoItem> Create(CreateTodoItemRequest createInstructionRequest)
    {
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Name = createInstructionRequest.Name,
            IsComplete = createInstructionRequest.IsComplete
        };
        
        await using var databaseConnection = await _todoDatabase.CreateAndOpenConnection();
        await _todoDatabase.ExecuteInTransaction(async (connection, transaction) =>
        {
            await _todoRepository.Create(todoItem, connection, transaction);
        });

        return todoItem;
    }

    public async Task<IEnumerable<TodoItem>> Get(TodoItemsQuery query)
    {
        await using var databaseConnection = await _todoDatabase.CreateAndOpenConnection();
        var items = await _todoRepository.GetByParams(query, databaseConnection);

        return items;
    }
}