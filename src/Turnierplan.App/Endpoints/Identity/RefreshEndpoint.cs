using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.App.Options;
using Turnierplan.App.Security;
using Turnierplan.Dal.Repositories;
using ClaimTypes = Turnierplan.App.Security.ClaimTypes;

namespace Turnierplan.App.Endpoints.Identity;

internal sealed class RefreshEndpoint : IdentityEndpointBase<RefreshEndpoint.RefreshEndpointResponse>
{
    public RefreshEndpoint(IOptionsMonitor<IdentityOptions> options, ISigningKeyProvider signingKeyProvider)
        : base(options, signingKeyProvider)
    {
    }

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/identity/refresh";

    protected override Delegate Handler => Handle;

    protected override bool DisableAuthorization => true;

    private async Task<IResult> Handle(
        HttpContext context,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        string? token = null;

        foreach (var (cookieName, cookieValue) in context.Request.Cookies)
        {
            if (cookieName.Equals(CookieNames.RefreshTokenCookieName))
            {
                token = cookieValue;
                break;
            }
        }

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var signingKey = await _signingKeyProvider.GetSigningKeyAsync(cancellationToken);
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
        catch (SecurityTokenException)
        {
            return Results.Unauthorized();
        }

        var tokenType = claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.TokenType))?.Value;

        if (!Equals(tokenType, JwtTokenTypes.Refresh))
        {
            return Results.Unauthorized();
        }

        var userIdFromToken = claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.UserId))?.Value;
        var securityStampFromToken = claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.SecurityStamp))?.Value;

        if (string.IsNullOrWhiteSpace(userIdFromToken)
            || string.IsNullOrWhiteSpace(securityStampFromToken)
            || !Guid.TryParse(userIdFromToken, out var userIdFromTokenGuid)
            || !Guid.TryParse(securityStampFromToken, out var securityStampFromTokenGuid))
        {
            return Results.Unauthorized();
        }

        var user = await userRepository.GetByIdAsync(userIdFromTokenGuid);

        // If the security stamp has changed, that means the user has changed their password since the reset token was issued
        if (user is null || user.SecurityStamp != securityStampFromTokenGuid)
        {
            return Results.Ok(new RefreshEndpointResponse
            {
                Success = false
            });
        }

        var accessToken = await CreateTokenForUserAsync(user, false, cancellationToken);
        var refreshToken = await CreateTokenForUserAsync(user, true, cancellationToken);

        AddResponseCookieForToken(context, accessToken, false);
        AddResponseCookieForToken(context, refreshToken, true);

        return Results.Ok(new RefreshEndpointResponse
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    public sealed record RefreshEndpointResponse
    {
        public required bool Success { get; init; }

        public string? AccessToken { get; init; }

        public string? RefreshToken { get; init; }
    }
}
