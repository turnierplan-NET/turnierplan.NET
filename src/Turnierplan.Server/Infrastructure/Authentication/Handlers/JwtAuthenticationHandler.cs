using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.Server.Infrastructure.Authentication.Options;
using Turnierplan.Server.Options;

namespace Turnierplan.Server.Infrastructure.Authentication.Handlers;

// TODO: Refactor to session-based authentication

internal static class CookieNames
{
    public const string AccessTokenCookieName = "acc_token";
    public const string RefreshTokenCookieName = "rfs_token";
}

internal static class JwtTokenTypes
{
    public const string Access = "acc";
    public const string Refresh = "rfs";
}

internal sealed class JwtAuthenticationHandler : AuthenticationHandler<IdentityOptions>
{
    private const int SigningKeySizeBytes = 512 / 8;
    private static readonly SymmetricSecurityKey __signingKey = new(RandomNumberGenerator.GetBytes(SigningKeySizeBytes)); // Use a temp in-memory key bc this handler will be removed anyway

    public JwtAuthenticationHandler(
        IOptionsMonitor<IdentityOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? token = null;

        foreach (var (cookieName, cookieValue) in Request.Cookies)
        {
            if (cookieName.Equals(CookieNames.AccessTokenCookieName))
            {
                token = cookieValue;
                break;
            }
        }

        if (token is null)
        {
            return AuthenticateResult.NoResult();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail("Invalid authentication token provided");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = __signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal claimsPrincipal;

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch (SecurityTokenArgumentException ex)
        {
            return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
        }
        catch (SecurityTokenException ex)
        {
            return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
        }

        var tokenType = claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.TokenType))?.Value;

        if (!Equals(tokenType, JwtTokenTypes.Access))
        {
            return AuthenticateResult.Fail("Incorrect token type.");
        }

        var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
