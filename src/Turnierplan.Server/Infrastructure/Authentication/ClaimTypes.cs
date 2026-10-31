namespace Turnierplan.Server.Infrastructure.Authentication;

internal static class ClaimTypes
{
    public const string AllowCreateOrganization = $"turnierplan_{nameof(AllowCreateOrganization)}";
    public const string IsAdministrator = $"turnierplan_{nameof(IsAdministrator)}";
    public const string PrincipalKind = $"turnierplan_{nameof(PrincipalKind)}";
    public const string PrincipalId = $"turnierplan_{nameof(PrincipalId)}";

    public const string UserName = $"turnierplan_{nameof(UserName)}";
    public const string UserEMail = $"turnierplan_{nameof(UserEMail)}";
    public const string UserFullName = $"turnierplan_{nameof(UserFullName)}";
}
