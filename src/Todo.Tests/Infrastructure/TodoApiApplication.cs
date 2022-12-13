using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.Api.Databases;

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
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => 
        {
            // We can further customize our application setup here.
            services.AddSingleton(
                DatabaseConfiguration.Create(new ConfigurationBuilder().AddInMemoryCollection(_settings).Build()));
        });
    }
}