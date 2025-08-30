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

        await context.Database.MigrateAsync().ConfigureAwait(false);

        var userCount = await context.Users.CountAsync().ConfigureAwait(false);

        if (userCount == 0)
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<TurnierplanOptions>>().Value;

            var overwriteInitialUserPassword = !string.IsNullOrWhiteSpace(options.InitialUserPassword);

            var initialUserName = string.IsNullOrWhiteSpace(options.InitialUserName) ? "Administrator" : options.InitialUserName;
            var initialUserEmail = string.IsNullOrWhiteSpace(options.InitialUserName) ? "admin@example.com" : options.InitialUserEmail;
            var initialUserPassword = overwriteInitialUserPassword ? options.InitialUserPassword! : Guid.NewGuid().ToString();

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var initialUser = new User(initialUserName, initialUserEmail)
            {
                IsAdministrator = true
            };

            initialUser.UpdatePassword(passwordHasher.HashPassword(initialUser, initialUserPassword));

            await context.Users.AddAsync(initialUser).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            // Don't log the password if it was specified using an environment variable
            var passwordForLogging = overwriteInitialUserPassword ? "****" : initialUserPassword;

            logger.LogInformation("An initial user \"{Name}\" was created. You can log in using \"{Email}\" and the password \"{Password}\". IMMEDIATELY change this password when running in a production environment!", initialUserName, initialUserEmail, passwordForLogging);
        }
        else
        {
            logger.LogInformation("Database contains {UserCount} user(s). No administrator account was created.", userCount);
        }
    }

    private sealed record DatabaseMigrator;
}
