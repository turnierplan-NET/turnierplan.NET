using System.Globalization;

namespace Turnierplan.Server.Domain.Users;

internal sealed class User : EntityBase<long>
{
    public User(string userName)
    {
        Id = 0;
        PrincipalId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UserName = userName;
        NormalizedUserName = Normalize(userName);
        FullName = null;
        EMail = null;
        NormalizedEMail = null;
        PasswordHash = string.Empty;
        IsAdministrator = false;
        LastPasswordChange = DateTime.MinValue;
        SecurityStamp = Guid.Empty;
    }

    public User(long id, Guid principalId, DateTime createdAt, string userName, string normalizedUserName, string? fullName, string? eMail, string? normalizedEMail, string passwordHash, bool isAdministrator, bool allowCreateOrganization, DateTime lastPasswordChange, Guid securityStamp)
    {
        Id = id;
        PrincipalId = principalId;
        CreatedAt = createdAt;
        UserName = userName;
        NormalizedUserName = normalizedUserName;
        FullName = fullName;
        EMail = eMail;
        NormalizedEMail = normalizedEMail;
        PasswordHash = passwordHash;
        IsAdministrator = isAdministrator;
        AllowCreateOrganization = allowCreateOrganization;
        LastPasswordChange = lastPasswordChange;
        SecurityStamp = securityStamp;
    }

    public override long Id { get; protected set; }

    public Guid PrincipalId { get; }

    public DateTime CreatedAt { get; }

    public string UserName { get; private set; }

    public string NormalizedUserName { get; private set; }

    public string? FullName { get; set; }

    public string? EMail { get; private set; }

    public string? NormalizedEMail { get; private set; }

    public string PasswordHash { get; private set; }

    public bool IsAdministrator { get; private set; }

    public bool AllowCreateOrganization { get; private set; }

    public DateTime LastPasswordChange { get; private set; }

    public DateTime? LastLogin { get; private set; }

    public Guid SecurityStamp { get; private set; }

    public void UpdateLastLoginTime()
    {
        LastLogin = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        LastPasswordChange = DateTime.UtcNow;
        SecurityStamp = Guid.NewGuid();
    }

    public void SetUserName(string userName)
    {
        UserName = userName.Trim();
        NormalizedUserName = Normalize(userName);
    }

    public void SetEmailAddress(string? newEmail)
    {
        if (newEmail is null)
        {
            EMail = null;
            NormalizedEMail = null;
        }
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newEmail);

            EMail = newEmail.Trim();
            NormalizedEMail = Normalize(newEmail);
        }

        SecurityStamp = Guid.NewGuid();
    }

    public void SetIsAdministrator(bool isAdministrator)
    {
        IsAdministrator = isAdministrator;

        if (isAdministrator)
        {
            AllowCreateOrganization = true;
        }
    }

    public void SetAllowCreateOrganization(bool allowCreateOrganization)
    {
        if (IsAdministrator && !allowCreateOrganization)
        {
            throw new TurnierplanDomainException($"'{nameof(AllowCreateOrganization)}' cannot be set to false for an administrator user.");
        }

        AllowCreateOrganization = allowCreateOrganization;
    }

    public static string Normalize(string value) => value.Trim().ToUpper(CultureInfo.InvariantCulture);
}
