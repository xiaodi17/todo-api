using Dapper;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Todo.Api.Databases;
using Todo.Api.Features.Todo;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(Log.Logger);
DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton(DatabaseConfiguration.Create(builder.Configuration));
builder.Services.AddSingleton<TodoDatabase>();
builder.Services.AddTodoFeature();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.Services
    .GetRequiredService<TodoDatabase>()
    .UpgradeIfNecessary();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthcheck");

app.Run();

public partial class Program
{
    
}