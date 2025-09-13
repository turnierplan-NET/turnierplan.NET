using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Turnierplan.App.Options;
using Turnierplan.App.Security;
using Turnierplan.Core.User;

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
        IUserRepository userRepository)
    {
        Guid userIdFromToken;
        Guid securityStampFromToken;

        try
        {
            var cookie = context.Request.Cookies.Single(x => x.Key.Equals(CookieNames.RefreshTokenCookieName));

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKeyProvider.GetSigningKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var claimsPrincipal = tokenHandler.ValidateToken(cookie.Value, validationParameters, out _);

            var tokenType = claimsPrincipal.Claims.Single(x => x.Type.Equals(ClaimTypes.TokenType)).Value;
            if (!tokenType.Equals(JwtTokenTypes.Refresh))
            {
                return Results.Unauthorized();
            }

            userIdFromToken = Guid.Parse(claimsPrincipal.Claims.Single(x => x.Type.Equals(ClaimTypes.UserId)).Value);
            securityStampFromToken = Guid.Parse(claimsPrincipal.Claims.Single(x => x.Type.Equals(ClaimTypes.SecurityStamp)).Value);
        }
        catch
        {
            return Results.Unauthorized();
        }

        var user = await userRepository.GetByIdAsync(userIdFromToken);

        // If the security stamp has changed, that means the user has changed their password since the reset token was issued
        if (user is null || user.SecurityStamp != securityStampFromToken)
        {
            return Results.Ok(new RefreshEndpointResponse
            {
                Success = false
            });
        }

        var accessToken = CreateTokenForUser(user, false);
        var refreshToken = CreateTokenForUser(user, true);

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
