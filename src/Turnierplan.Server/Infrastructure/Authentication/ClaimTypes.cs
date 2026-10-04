namespace Turnierplan.Server.Infrastructure.Authentication;

internal static class ClaimTypes
{
    public const string AllowCreateOrganization = $"turnierplan_{nameof(AllowCreateOrganization)}";
    public const string IsAdministrator = $"turnierplan_{nameof(IsAdministrator)}";
    public const string PrincipalKind = $"turnierplan_{nameof(PrincipalKind)}";
    public const string PrincipalId = $"turnierplan_{nameof(PrincipalId)}";

    // TODO: Delete the ones no longer needed
    public const string EMailAddress = "mail";
    public const string FullName = "fullName";
    public const string SecurityStamp = "sst";
    public const string TokenType = "typ";
    public const string UserId = "uid";
    public const string UserName = "userName";
}
