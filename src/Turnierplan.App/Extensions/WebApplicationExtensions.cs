using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Turnierplan.App.Constants;
using Turnierplan.Core.User;
using Turnierplan.Dal;
using Turnierplan.Dal.Extensions;

namespace Turnierplan.App.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        if (string.IsNullOrWhiteSpace(app.Configuration.GetDatabaseConnectionString()))
        {
            return;
        }

        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();
        var context = scope.ServiceProvider.GetRequiredService<TurnierplanContext>();

        await context.Database.MigrateAsync().ConfigureAwait(false);

        await EnsureNoDowngradeAsync(context, logger).ConfigureAwait(false);

        var userCount = await context.Users.CountAsync().ConfigureAwait(false);

        if (userCount == 0)
        {
            const string initialEmail = "admin@example.com";
            var initialPassword = Guid.NewGuid().ToString();

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var initialUser = new User("Administrator", initialEmail)
            {
                IsAdministrator = true
            };

            initialUser.UpdatePassword(passwordHasher.HashPassword(initialUser, initialPassword));

            await context.Users.AddAsync(initialUser).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            logger.LogInformation("An initial user was created. You can log in using \"{Email}\" and the password \"{Password}\". IMMEDIATELY change this password when running in a production environment!", initialEmail, initialPassword);
        }
        else
        {
            logger.LogInformation("Database contains {UserCount} user(s). No administrator account was created.", userCount);
        }
    }

    private static async Task EnsureNoDowngradeAsync(TurnierplanContext context, ILogger<DatabaseMigrator> logger)
    {
        const string schema = TurnierplanContext.Schema;

        if (!TurnierplanMetadata.IsVersionAvailable)
        {
            throw new InvalidOperationException("Downgrade check failed because the current version is not available.");
        }

        /* The version check works by creating a special table which stores all turnierplan.NET versions that have ever
         * been run on this database along with the corresponding timestamp. Then, we try to insert a new row with the
         * current version - doing nothing if a row for that specific version already exists. Finally, we query the row
         * with the most recent version. If that version does not match the version we are currently running, a version
         * downgrade has occurred, and we stop the application from continuing execution. */

        // The transaction is necessary because otherwise, we will save invalid
        // version history entries to the database in the case of a version downgrade.
        await context.Database.BeginTransactionAsync().ConfigureAwait(false);

        var versionParameter = new NpgsqlParameter("version", TurnierplanMetadata.Version);
        var majorParameter = new NpgsqlParameter("major", TurnierplanMetadata.Major);
        var minorParameter = new NpgsqlParameter("minor", TurnierplanMetadata.Minor);
        var patchParameter = new NpgsqlParameter("patch", TurnierplanMetadata.Patch);

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
""", versionParameter, majorParameter, minorParameter, patchParameter).ConfigureAwait(false);

        var mostRecentVersion = await context.Database.SqlQueryRaw<VersionHistory>($"SELECT * FROM {schema}.\"__TPVersionHistory\"")
            .OrderByDescending(x => x.Major)
            .ThenByDescending(x => x.Minor)
            .ThenByDescending(x => x.Patch)
            .FirstAsync()
            .ConfigureAwait(false);;

        if (!mostRecentVersion.Version.Equals(TurnierplanMetadata.Version))
        {
            logger.LogCritical("Detected version downgrade from '{MostRecentVersion}' to '{CurrentVersion}'.", mostRecentVersion.Version, TurnierplanMetadata.Version);
            Environment.Exit(1);

            return;
        }

        // Commit only after we know that the current version is "valid"
        await context.Database.CommitTransactionAsync().ConfigureAwait(false);
    }

    private sealed record DatabaseMigrator;

    private sealed record VersionHistory(string Version, int Major, int Minor, int Patch);
}
