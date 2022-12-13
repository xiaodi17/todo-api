using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Todo.Tests.Infrastructure;

public class TodoApiHost
{
    private readonly IHost _host;

    public TodoApiHost(Dictionary<string, string> settings)
    {
        var port = GetNextAvailablePort();
        var args = new[] {"--urls", $"http://0.0.0.0:{port}", "--environment", "Local"};
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configuration) => { configuration.AddInMemoryCollection(settings); })
            .Build()
            .RunAsync();

        Client = new HttpClient {BaseAddress = new Uri($"http://localhost:{port}")};
        Client.DefaultRequestHeaders.Add("traceparent", settings["TestName"]);
        Client.DefaultRequestHeaders.Add("SystemId", "Tests");
        WaitForApiAsync();
    }

    public HttpClient Client { get; }

    // public async Task Setup(CancellationToken cancellationToken)
    // {
    //     //var _ = _host.RunAsync(cancellationToken);
    //      WaitForApiAsync(cancellationToken);
    // }

    private void WaitForApiAsync()
    {
        for (var _ = 0; _ < 20; _++)
        {
            try
            {
                Thread.Sleep(1000);
                var response = Client.GetAsync("/healthcheck").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Information($"Waiting for API - {ex.Message}");
            }
        }

        throw new Exception("Failed to connect to API.");
    }

    // Copied from https://github.com/aspnet/KestrelHttpServer/blob/47f1db20e063c2da75d9d89653fad4eafe24446c/test/Microsoft.AspNetCore.Server.Kestrel.FunctionalTests/AddressRegistrationTests.cs#L508
    private static int GetNextAvailablePort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Let the OS assign the next available port. Unless we cycle through all ports
        // on a test run, the OS will always increment the port number when making these calls.
        // This prevents races in parallel test runs where a test is already bound to
        // a given port, and a new test is able to bind to the same port due to port
        // reuse being enabled by default by the OS.
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint) socket.LocalEndPoint)!.Port;
    }
}