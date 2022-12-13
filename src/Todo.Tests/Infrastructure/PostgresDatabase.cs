using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DbUp;
using DbUp.Engine.Output;
using Npgsql;
using Serilog;

namespace Todo.Tests.Infrastructure;

public class PostgresDatabase : IAsyncDisposable
{
    private readonly string _name;
    private readonly string _connectionString;
    
    public PostgresDatabase(Dictionary<string, string> settings)
    {
        _name = settings["DatabaseName"];
        _connectionString = $"Server={settings["DatabaseHost"]};User Id={settings["DatabaseUsername"]};Password={settings["DatabasePassword"]};";
        
    }
    
    public async Task Setup(CancellationToken cancellationToken)
    {
        await WaitTillReady(cancellationToken);
        EnsureDatabase.For.PostgresqlDatabase(_connectionString + $"Database={_name}", new AutodetectUpgradeLog());
    }

    public async ValueTask DisposeAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync($"REVOKE CONNECT ON DATABASE {_name} FROM public", connection);
        await connection.ExecuteAsync($"ALTER DATABASE {_name} CONNECTION LIMIT 0", connection);
        var sql = $@"SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE pid <> pg_backend_pid()
                AND datname = '{_name}'";
        await connection.ExecuteAsync(sql, connection);
        await connection.ExecuteAsync($"drop database {_name}", connection);
    }
    
    private async Task WaitTillReady(CancellationToken cancellationToken)
    {
        for (var _ = 0; _ < 20; _++)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                Log.Information("Database connection established.");
                return;

            }
            catch (Exception ex)
            {
                Log.Information("Waiting for postgres db - {Message}", ex.Message);
            }
        }
        throw new Exception("Failed to connect to database.");
    }
}