namespace Turnierplan.Server.Infrastructure.Authorization;

internal static class PolicyNames
{
    public const string Default = $"turnierplan_{nameof(Default)}";

    public const string CreateOrganization = $"turnierplan_{nameof(CreateOrganization)}";
    public const string Logout = $"turnierplan_{nameof(Logout)}";
}
