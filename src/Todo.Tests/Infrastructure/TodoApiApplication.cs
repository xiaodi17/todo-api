using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Todo.Tests.Infrastructure;

public class TodoApiApplication : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string> _settings;
    public TodoApiApplication(Dictionary<string, string> settings)
    {
        _settings = settings;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (context, configuration) => { configuration.AddInMemoryCollection(_settings); });
        
        return base.CreateHost(builder);
    }
}