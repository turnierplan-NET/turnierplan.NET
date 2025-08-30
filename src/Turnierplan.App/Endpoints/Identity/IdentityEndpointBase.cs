using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.App.Options;
using Turnierplan.App.Security;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.User;
using ClaimTypes = Turnierplan.App.Security.ClaimTypes;

namespace Turnierplan.App.Endpoints.Identity;

internal abstract class IdentityEndpointBase<TResponse> : EndpointBase<TResponse>
{
    protected readonly ISigningKeyProvider _signingKeyProvider;
    private readonly IOptionsMonitor<IdentityOptions> _options;

    protected IdentityEndpointBase(IOptionsMonitor<IdentityOptions> options, ISigningKeyProvider signingKeyProvider)
    {
        _options = options;
        _signingKeyProvider = signingKeyProvider;
    }

    protected string CreateTokenForUser(User user, bool isRefreshToken)
    {
        var claims = new List<Claim>();

        if (isRefreshToken)
        {
            claims.Add(new Claim(ClaimTypes.TokenType, JwtTokenTypes.Refresh));
            claims.Add(new Claim(ClaimTypes.SecurityStamp, user.SecurityStamp.ToString()));
            claims.Add(new Claim(ClaimTypes.UserId, user.Id.ToString()));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.TokenType, JwtTokenTypes.Access));
            claims.Add(new Claim(ClaimTypes.DisplayName, user.Name));
            claims.Add(new Claim(ClaimTypes.EMailAddress, user.EMail));
            claims.Add(new Claim(ClaimTypes.UserId, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.PrincipalId, user.PrincipalId.ToString()));
            claims.Add(new Claim(ClaimTypes.PrincipalKind, nameof(PrincipalKind.User)));

            if (user.IsAdministrator)
            {
                claims.Add(new Claim(ClaimTypes.Administrator, "true"));
            }
        }

        var identityOptions = _options.CurrentValue;
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow + (isRefreshToken ? identityOptions.RefreshTokenLifetime : identityOptions.AccessTokenLifetime),
            SigningCredentials = new SigningCredentials(_signingKeyProvider.GetSigningKey(), _signingKeyProvider.GetSigningAlgorithm())
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    protected void AddResponseCookieForToken(HttpContext context, string token, bool isRefreshToken)
    {
        var cookieName = isRefreshToken ? CookieNames.RefreshTokenCookieName : CookieNames.AccessTokenCookieName;
        var cookieExpires = DateTimeOffset.UtcNow + (isRefreshToken ? _options.CurrentValue.RefreshTokenLifetime : _options.CurrentValue.AccessTokenLifetime);

        AddCookie(isRefreshToken ? "/api/identity/refresh" : "/api");

        return;

        void AddCookie(string path)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Path = path,
                Expires = cookieExpires
            };

            context.Response.Cookies.Append(cookieName, token, cookieOptions);
        }
    }

    /// <summary>
    /// Password is considered secure only if all the following conditions are met:
    /// <ul>
    /// <li>Does not start or end with white-space</li>
    /// <li>Length is at least 10 characters</li>
    /// <li>Contains at least one upper-case ASCII letter</li>
    /// <li>Contains at least one lower-case ASCII letter</li>
    /// <li>Contains at least one digit</li>
    /// <li>Contains at least one punctuation or symbol character</li>
    /// </ul>
    /// </summary>
    protected static bool IsPasswordInsecure(string password)
    {
        var lengthBeforeTrim = password.Length;

        password = password.Trim();

        if (lengthBeforeTrim != password.Length || password.Length < 10)
        {
            return true;
        }

        var upper = password.Count(char.IsAsciiLetterUpper);
        var lower = password.Count(char.IsAsciiLetterLower);
        var digits = password.Count(char.IsDigit);
        var punctuation = password.Count(char.IsPunctuation);
        var symbols = password.Count(char.IsSymbol);

        return upper == 0 || lower == 0 || digits == 0 || (punctuation == 0 && symbols == 0);
    }

    // All identity endpoints should use this artificial delay at the start of every request

#if DEBUG
    protected static Task IdentityDelay() => Task.CompletedTask;
#else
    protected static Task IdentityDelay() => Task.Delay(1000 + Random.Shared.Next(500));
#endif
}
