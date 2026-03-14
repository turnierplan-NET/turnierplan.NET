using System.IdentityModel.Tokens.Jwt;
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
        if (!Request.Cookies.ContainsKey(CookieNames.AccessTokenCookieName))
        {
            return AuthenticateResult.NoResult();
        }

        string token;

        try
        {
            token = Request.Cookies.Single(x => x.Key.Equals(CookieNames.AccessTokenCookieName)).Value;
        }
        catch
        {
            return AuthenticateResult.Fail("Missing or malformed access token cookie.");
        }

        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Empty access token cookie.");
        }

        try
        {
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

            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var tokenType = claimsPrincipal.Claims.Single(x => x.Type.Equals(ClaimTypes.TokenType)).Value;

            if (!tokenType.Equals(JwtTokenTypes.Access))
            {
                return AuthenticateResult.Fail("Incorrect token type.");
            }

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Invalid token: {ex.Message}");
        }
    }
}
