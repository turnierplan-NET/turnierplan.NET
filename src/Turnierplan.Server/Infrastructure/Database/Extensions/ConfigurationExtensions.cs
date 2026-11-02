namespace Turnierplan.Server.Infrastructure.Database.Extensions;

internal static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public bool UseInMemoryDatabase()
        {
            return configuration.GetSection("Database").GetValue("InMemory", false);
        }

        public string? GetDatabaseConnectionString()
        {
            return configuration.GetSection("Database").GetValue<string>("ConnectionString");
        }
    }
}
