namespace Turnierplan.Server.Infrastructure.Authentication;

internal static class AuthenticationTypes
{
    public const string ApiKey = $"turnierplan_{nameof(ApiKey)}";
    public const string Session = $"turnierplan_{nameof(Session)}";
}
