namespace Turnierplan.App.Security;

internal static class ClaimTypes
{
    public const string Administrator = "adm";
    public const string EMailAddress = "mail"; // TODO Check dependent usages
    public const string FullName = "fullName"; // TODO Check dependent usages
    public const string PrincipalKind = "principalkind";
    public const string PrincipalId = "principalid";
    public const string SecurityStamp = "sst";
    public const string TokenType = "typ";
    public const string UserId = "uid";
    public const string UserName = "userName"; // TODO Check dependent usages
}
