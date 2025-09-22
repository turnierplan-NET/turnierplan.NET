using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Turnierplan.App.Options;
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

        if (context.Database.IsNpgsql())
        {
            // If the database is in-memory, no migration is necessary
            await context.Database.MigrateAsync();
        }

        var userCount = await context.Users.CountAsync();

        if (userCount == 0)
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<TurnierplanOptions>>().Value;

            var overwriteInitialUserPassword = !string.IsNullOrWhiteSpace(options.InitialUserPassword);

            var initialUserName = string.IsNullOrWhiteSpace(options.InitialUserName) ? "admin" : options.InitialUserName;
            var initialUserPassword = overwriteInitialUserPassword ? options.InitialUserPassword! : Guid.NewGuid().ToString();

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var initialUser = new User(initialUserName)
            {
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
}
