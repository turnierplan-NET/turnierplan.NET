using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Turnierplan.Server.Infrastructure.Database.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanContext(this IServiceCollection services, IConfiguration configuration)
    {
        string? inMemoryDatabaseName = null;

        services.AddDbContext<TurnierplanContext>((sp, options) =>
        {
            if (sp.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }

            if (configuration.UseInMemoryDatabase())
            {
                inMemoryDatabaseName ??= Guid.NewGuid().ToString();
                options.UseInMemoryDatabase(inMemoryDatabaseName);
                options.ConfigureWarnings(x => x.Log(InMemoryEventId.TransactionIgnoredWarning));
            }
            else
            {
                var connectionString = configuration.GetDatabaseConnectionString();

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(TurnierplanContext).Assembly.GetName().Name);
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationHistory", TurnierplanContext.Schema);
                });

                options.UseSnakeCaseNamingConvention();
            }
        });
    }
}
