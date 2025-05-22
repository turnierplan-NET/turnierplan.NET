using Microsoft.AspNetCore.Authentication;

namespace Turnierplan.App.Options;

internal sealed class IdentityOptions : AuthenticationSchemeOptions
{
    public string? StoragePath { get; init; } = string.Empty;

    public TimeSpan AccessTokenLifetime { get; init; } = TimeSpan.Zero;

    public TimeSpan RefreshTokenLifetime { get; init; } = TimeSpan.Zero;
}

