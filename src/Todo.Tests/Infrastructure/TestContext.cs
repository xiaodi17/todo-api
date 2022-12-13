using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Todo.Tests.Infrastructure;

public class TestContext : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IDisposable _logContext;
    private readonly TodoApiHost _todoApiHost;
    private readonly PostgresDatabase _todoDatabase;

    public TestContext()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var settings = GetSettings();

        _todoDatabase = new PostgresDatabase(settings);
        _todoApiHost = new TodoApiHost(settings);
        Client = _todoApiHost.Client;
    }

    public async Task Setup()
    {
        await _todoDatabase.Setup(_cancellationTokenSource.Token);
        // await _todoApiHost.Setup(_cancellationTokenSource.Token);
    }
    
    public HttpClient Client { get; }

    private Dictionary<string, string> GetSettings()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json")
            .AddJsonFile($"appSettings.{GetEnvironment()}.json", true, false)
            .Build();

        var stackTrace = new StackTrace();
        var testName = stackTrace.GetFrame(5).GetMethod().Name;
        if (testName == "InvokeMethod")
        {
            testName = stackTrace.GetFrame(4).GetMethod().Name;
        }

        var testId = Guid.NewGuid().ToString("N");
        return new Dictionary<string, string>
        {
            ["TestId"] = testId,
            ["TestName"] = testName,
            ["DatabaseHost"] = config.GetValue<string>("DatabaseHost"),
            ["DatabaseName"] = $"db_{testId}",
            ["DatabasePort"] = "5432",
            ["DatabaseUsername"] = "postgres",
            ["DatabasePassword"] = "123456",
            ["DatabaseCredentials"] = "{\"username\":\"postgres\",\"password\":\"123456\"}",
        };
    }
    
    private static string GetEnvironment() => Environment.GetEnvironmentVariable("IntegrationTestEnvironment") ?? "";
    
    public async ValueTask DisposeAsync()
    {
        await _todoDatabase.DisposeAsync();
    }
}