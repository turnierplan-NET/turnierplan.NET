using System.Globalization;
using Turnierplan.Core.Entity;

namespace Turnierplan.Core.User;

public sealed class User : Entity<Guid>
{
    public User(string userName)
    {

        Id = Guid.NewGuid();
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

    internal User(Guid id, Guid principalId, DateTime createdAt, string userName, string normalizedUserName, string? fullName, string? eMail, string? normalizedEMail, string passwordHash, bool isAdministrator, DateTime lastPasswordChange, Guid securityStamp)
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
        LastPasswordChange = lastPasswordChange;
        SecurityStamp = securityStamp;
    }

    public override Guid Id { get; protected set; }

    public Guid PrincipalId { get; }

    public DateTime CreatedAt { get; }

    public string UserName { get; private set; }

    public string NormalizedUserName { get; private set; }

    public string? FullName { get; set; }

    public string? EMail { get; private set; }

    public string? NormalizedEMail { get; private set; }

    public string PasswordHash { get; private set; }

    public bool IsAdministrator { get; set; }

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

    public static string Normalize(string value) => value.Trim().ToUpper(CultureInfo.InvariantCulture);
}
