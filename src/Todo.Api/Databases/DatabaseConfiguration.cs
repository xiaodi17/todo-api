using Newtonsoft.Json;

namespace Todo.Api.Databases;

public class DatabaseConfiguration
{
    public string ConnectionString { get; set; } = "";

    public static DatabaseConfiguration Create(IConfiguration configuration)
    {
        var name = configuration["DatabaseName"];
        var host = configuration["DatabaseHost"];
        var port = configuration["DatabasePort"];

        var credentials = JsonConvert.DeserializeObject<Credentials>(configuration["DatabaseCredentials"]);

        var connectionString = $"Host={host};Database={name};Port={port};Username={credentials?.Username};Password={credentials?.Password}";
        return new DatabaseConfiguration { ConnectionString = connectionString };
    }

    private class Credentials
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}