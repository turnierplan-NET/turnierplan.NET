using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain.Users;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;

namespace Turnierplan.Server.Infrastructure.Authentication.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/auth/login", async (
            [FromForm] string username,
            [FromForm] string password,
            TurnierplanContext turnierplanContext,
            IPasswordHasher<User> passwordHasher,
            HttpContext httpContext) =>
        {
#if !DEBUG
            await Task.Delay(1000 + Random.Shared.Next(500), cancellationToken);
#endif

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Results.BadRequest();
            }

            var normalizedUserName = User.Normalize(username);
            var user = await turnierplanContext.Users.Where(x => x.NormalizedUserName.Equals(normalizedUserName)).FirstOrDefaultAsync();

            if (user is null)
            {
                return Results.BadRequest();
            }

            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult is not PasswordVerificationResult.Success)
            {
                return Results.BadRequest();
            }

            user.UpdateLastLoginTime();

            await turnierplanContext.SaveChangesAsync();

            httpContext.Session.SetUser(user);

            return Results.NoContent();
        })
        .DisableAntiforgery()
        .WithName("Login")
        .WithTags("Authentication");

        builder.MapPost("/auth/logout", (HttpContext httpContext) =>
        {
            httpContext.Session.ResetUser();

            return Results.NoContent();
        })
        .DisableAntiforgery()
        .RequireAuthorization(PolicyNames.Logout)
        .WithName("Logout")
        .WithTags("Authentication");
    }
}
