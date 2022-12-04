using System.Data.Common;
using Dapper;
using Npgsql;
using Todo.Api.Features.Todo.Models;
using Todo.Api.Infrastructure;

namespace Todo.Api.Features.Todo;

public class TodoRepository
{
    public async Task Create(TodoItem instruction, DbConnection connection, DbTransaction transaction)
    {
        try
        {
            var dbTodoItem = new
            {
                TodoId = instruction.Id,
                Name = instruction.Name,
                IsComplete = instruction.IsComplete
            };
            await connection.ExecuteAsync(@"
                INSERT INTO todo_items (
                        todo_id,
                        name,
                        is_complete
                    )
                    VALUES (
                        @TodoId,       
                        @Name,           
                        @IsComplete
                    )", dbTodoItem, transaction);

        }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new DuplicateException(e);
        }
    }
}