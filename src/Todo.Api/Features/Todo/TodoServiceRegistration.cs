namespace Todo.Api.Features.Todo;

public static class TodoServiceRegistration
{
    public static void AddTodoFeature(this IServiceCollection services)
    {
        services.AddSingleton<TodoRepository>();
        services.AddTransient<TodoService>();
    }
}