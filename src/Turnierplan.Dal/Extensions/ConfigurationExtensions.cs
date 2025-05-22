using Microsoft.Extensions.Configuration;

namespace Turnierplan.Dal.Extensions;

public static class ConfigurationExtensions
{
    public static string? GetDatabaseConnectionString(this IConfiguration configuration)
    {
        return configuration.GetSection("Database").GetValue<string>("ConnectionString");
    }
}
