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

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.ContainsKey(CookieNames.AccessTokenCookieName))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string token;

        try
        {
            token = Request.Cookies.Single(x => x.Key.Equals(CookieNames.AccessTokenCookieName)).Value;
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing or malformed access token cookie."));
        }

        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(AuthenticateResult.Fail("Empty access token cookie."));
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKeyProvider.GetSigningKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };

            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var tokenType = claimsPrincipal.Claims.Single(x => x.Type.Equals(ClaimTypes.TokenType)).Value;

            if (!tokenType.Equals(JwtTokenTypes.Access))
            {
                return Task.FromResult(AuthenticateResult.Fail("Incorrect token type."));
            }

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail($"Invalid token: {ex.Message}"));
        }
    }
}
