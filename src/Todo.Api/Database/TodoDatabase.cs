using System.Data.Common;
using System.Reflection;
using DbUp;
using Npgsql;

namespace Todo.Api.Databases;

public class TodoDatabase
{
    private readonly DatabaseConfiguration _config;
    private readonly ILogger<TodoDatabase> _logger;

    public TodoDatabase(DatabaseConfiguration config, ILogger<TodoDatabase> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<DbConnection> CreateAndOpenConnection(CancellationToken stoppingToken = default)
    {
        var connection = new NpgsqlConnection(_config.ConnectionString);
        await connection.OpenAsync(stoppingToken);

        return connection;
    }

    public async Task ExecuteInTransaction(Func<DbConnection, DbTransaction, Task> action, CancellationToken cancellationToken = default)
    {
        await using var conn = await CreateAndOpenConnection(cancellationToken);
        await using var transaction = await conn.BeginTransactionAsync(cancellationToken);
        try
        {
            await action(conn, transaction);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Exception while executing transaction - rolling back");
            try
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Error rolling back transaction");
            }

            throw;
        }
    }

    public void UpgradeIfNecessary()
    {
        _logger.LogInformation("Upgrading Database");
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_config.ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToAutodetectedLog()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Failed to upgrade the database");
        }
    }
}