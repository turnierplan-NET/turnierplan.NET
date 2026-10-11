namespace Turnierplan.Server.Infrastructure.Authorization;

internal static class PolicyNames
{
    public const string Default = $"turnierplan_{nameof(Default)}";
    public const string User = $"turnierplan_{nameof(User)}";

    public const string CreateOrganization = $"turnierplan_{nameof(CreateOrganization)}";
}
