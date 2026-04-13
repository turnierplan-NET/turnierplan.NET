using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.App.Options;

namespace Turnierplan.App.Security;

internal sealed class JwtAuthenticationHandler : AuthenticationHandler<IdentityOptions>
{
    private readonly ISigningKeyProvider _signingKeyProvider;

    public JwtAuthenticationHandler(
        IOptionsMonitor<IdentityOptions> options,
        ISigningKeyProvider signingKeyProvider,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _signingKeyProvider = signingKeyProvider;
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

        var signingKey = await _signingKeyProvider.GetSigningKeyAsync(CancellationToken.None);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
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
