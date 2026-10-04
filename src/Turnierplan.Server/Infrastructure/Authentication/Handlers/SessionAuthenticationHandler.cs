using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Authentication.Extensions;
using Turnierplan.Server.Infrastructure.Database;

namespace Turnierplan.Server.Infrastructure.Authentication.Handlers;

internal sealed class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TurnierplanContext _turnierplanContext;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TurnierplanContext turnierplanContext)
        : base(options, logger, encoder)
    {
        _turnierplanContext = turnierplanContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Session.TryGetUserId(out var userId))
        {
            return AuthenticateResult.NoResult();
        }

        var user = await _turnierplanContext.Users.FindAsync(userId);

        if (user is null)
        {
            return AuthenticateResult.Fail($"The user with ID '{userId}' does not exist.");
        }

        // TODO: Check if user is suspended or similar?
        // TODO: Check user security stamp against stamp in session

        List<Claim> claims = [
            new(ClaimTypes.PrincipalId, user.PrincipalId.ToString()),
            new(ClaimTypes.PrincipalKind, nameof(PrincipalKind.User))
        ];

        if (user.AllowCreateOrganization)
        {
            claims.Add(new Claim(ClaimTypes.AllowCreateOrganization, ClaimValues.True));
        }

        if (user.IsAdministrator)
        {
            claims.Add(new Claim(ClaimTypes.IsAdministrator, ClaimValues.True));
        }

        var identity = new ClaimsIdentity(claims, authenticationType: AuthenticationTypes.Session);

        var principal = new ClaimsPrincipal([ identity ]);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
