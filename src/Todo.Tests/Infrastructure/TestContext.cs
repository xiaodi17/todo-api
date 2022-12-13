using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Todo.Tests.Infrastructure;

public class TestContext : IAsyncDisposable
{
    private readonly TodoApiHost _todoApiHost;

    public TestContext()
    {
        var settings = GetSettings();

        TodoDatabase = new PostgresDatabase(settings);
        _todoApiHost = new TodoApiHost(settings);
        Client = _todoApiHost.Client;
    }

    public HttpClient Client { get; }
    public PostgresDatabase TodoDatabase { get; }

    private Dictionary<string, string> GetSettings()
    {
        var environment = GetEnvironment();
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json")
            .AddJsonFile($"appSettings.{environment}.json", true, false)
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
            ["DatabaseName"] = $"todo_{testId}",
            ["DatabasePort"] = "5432",
            ["DatabaseUsername"] = "postgres",
            ["DatabasePassword"] = "123456",
            ["DatabaseCredentials"] = "{\"username\":\"postgres\",\"password\":\"123456\"}",
        };
    }
    
    private static string GetEnvironment() => Environment.GetEnvironmentVariable("IntegrationTestEnvironment") ?? "";
    
    public async ValueTask DisposeAsync()
    {
        await TodoDatabase.DisposeAsync();
    }
}