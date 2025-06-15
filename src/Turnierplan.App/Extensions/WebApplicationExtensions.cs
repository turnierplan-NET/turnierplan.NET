using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            logger.LogWarning("An initial user was created. You can log in using \"{Email}\" and the password \"{Password}\". IMMEDIATELY change this password when running in a production environment!", initialEmail, initialPassword);
        }
        else
        {
            logger.LogInformation("Database contains {UserCount} user(s). No administrator account was created.", userCount);
        }
    }

    private sealed record DatabaseMigrator;
}
