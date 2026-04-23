using Microsoft.AspNetCore.Authentication;

namespace Turnierplan.App.Options;

internal sealed class IdentityOptions : AuthenticationSchemeOptions
{
    public string? SigningKey { get; init; }

    public string? StoragePath { get; init; }

    public TimeSpan AccessTokenLifetime { get; init; } = TimeSpan.Zero;

    public TimeSpan RefreshTokenLifetime { get; init; } = TimeSpan.Zero;

    public bool? UseInsecureCookies { get; init; }
}

