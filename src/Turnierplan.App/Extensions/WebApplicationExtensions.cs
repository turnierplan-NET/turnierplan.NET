using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Turnierplan.App.Constants;
using Turnierplan.App.Options;
using Turnierplan.Core.User;
using Turnierplan.Dal;
using Turnierplan.Dal.Extensions;

namespace Turnierplan.App.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        if (string.IsNullOrWhiteSpace(app.Configuration.GetDatabaseConnectionString()) && !app.Configuration.UseInMemoryDatabase())
        {
            return;
        }

        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();
        var context = scope.ServiceProvider.GetRequiredService<TurnierplanContext>();

        if (context.Database.IsNpgsql())
        {
            // If the database is in-memory, no migration or downgrade check is necessary

            await context.Database.MigrateAsync();

            await EnsureNoDowngradeAsync(logger, context);
        }

        await EnsureInitialUserCreatedAsync(scope.ServiceProvider, logger, context);
    }

    private static async Task EnsureNoDowngradeAsync(ILogger<DatabaseMigrator> logger, TurnierplanContext context)
    {
        const string schema = TurnierplanContext.Schema;

        if (!TurnierplanVersion.IsVersionAvailable)
        {
            throw new InvalidOperationException("Downgrade check failed because the current version is not available.");
        }

        /* The version check works by creating a special table which stores all turnierplan.NET versions that have ever
         * been run on this database along with the corresponding timestamp. Then, we try to insert a new row with the
         * current version - doing nothing if a row for that specific version already exists. Finally, we query the row
         * with the most recent version. If that version does not match the version we are currently running a version
         * downgrade has occurred, and we stop the application from continuing execution. */

        // The transaction is necessary because otherwise, we will save invalid
        // version history entries to the database in the case of a version downgrade.
        await context.Database.BeginTransactionAsync();

        var versionParameter = new NpgsqlParameter("version", TurnierplanVersion.Version);
        var majorParameter = new NpgsqlParameter("major", TurnierplanVersion.Major);
        var minorParameter = new NpgsqlParameter("minor", TurnierplanVersion.Minor);
        var patchParameter = new NpgsqlParameter("patch", TurnierplanVersion.Patch);

        await context.Database.ExecuteSqlRawAsync($"""
CREATE TABLE IF NOT EXISTS {schema}."__TPVersionHistory" (
    "Version"   text NOT NULL UNIQUE,
    "Major"     integer NOT NULL,
    "Minor"     integer NOT NULL,
    "Patch"     integer NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL
);

INSERT INTO {schema}."__TPVersionHistory" ("Version", "Major", "Minor", "Patch", "Timestamp")
    VALUES (@version, @major, @minor, @patch, now())
    ON CONFLICT DO NOTHING;
""", versionParameter, majorParameter, minorParameter, patchParameter);

        var mostRecentVersion = await context.Database.SqlQueryRaw<VersionHistory>(
                $"SELECT * FROM {schema}.\"__TPVersionHistory\"")
            .OrderByDescending(x => x.Major)
            .ThenByDescending(x => x.Minor)
            .ThenByDescending(x => x.Patch)
            .FirstAsync();

        if (!mostRecentVersion.Version.Equals(TurnierplanVersion.Version))
        {
            logger.LogCritical("Detected version downgrade from '{MostRecentVersion}' to '{CurrentVersion}'.", mostRecentVersion.Version, TurnierplanVersion.Version);
            Environment.Exit(1);

            return;
        }

        // Commit only after we know that the current version is "valid"
        await context.Database.CommitTransactionAsync();
    }

    private static async Task EnsureInitialUserCreatedAsync(IServiceProvider serviceProvider, ILogger<DatabaseMigrator> logger, TurnierplanContext context)
    {
        var userCount = await context.Users.CountAsync();

        if (userCount == 0)
        {
            var options = serviceProvider.GetRequiredService<IOptions<TurnierplanOptions>>().Value;

            var overwriteInitialUserPassword = !string.IsNullOrWhiteSpace(options.InitialUserPassword);

            var initialUserName = string.IsNullOrWhiteSpace(options.InitialUserName) ? "admin" : options.InitialUserName;
            var initialUserPassword = overwriteInitialUserPassword ? options.InitialUserPassword! : Guid.NewGuid().ToString();

            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();

            var initialUser = new User(initialUserName)
            {
                FullName = "Administrator",
                IsAdministrator = true
            };

            initialUser.UpdatePassword(passwordHasher.HashPassword(initialUser, initialUserPassword));

            await context.Users.AddAsync(initialUser);
            await context.SaveChangesAsync();

            if (overwriteInitialUserPassword)
            {
                // Don't log the password if it was specified using an environment variable
                logger.LogInformation("An initial user was created: You can log in using \"{Name}\" and the password \"****\" (set by environment variable). This is NOT recommended in a production environment!", initialUserName);
            }
            else
            {
                logger.LogInformation("An initial user was created: You can log in using \"{Name}\" and the password \"{Password}\". IMMEDIATELY change this password when running in a production environment!", initialUserName, initialUserPassword);
            }
        }
        else
        {
            logger.LogInformation("Database contains {UserCount} user(s). No administrator account was created.", userCount);
        }
    }

    private sealed record DatabaseMigrator;

    private sealed record VersionHistory(string Version, int Major, int Minor, int Patch);
}
