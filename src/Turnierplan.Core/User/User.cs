using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.User;

public sealed class User : Entity<Guid>
{
    internal readonly List<Organization.Organization> _organizations = new();

    public User(string name, string email)
    {
        email = email.Trim();

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        EMail = email;
        NormalizedEMail = NormalizeEmail(email);
        PasswordHash = string.Empty;
        IsAdministrator = false;
        LastPasswordChange = DateTime.MinValue;
        SecurityStamp = Guid.Empty;
    }

    internal User(Guid id, DateTime createdAt, string name, string eMail, string normalizedEMail, string passwordHash, bool isAdministrator, DateTime lastPasswordChange, Guid securityStamp)
    {
        Id = id;
        CreatedAt = createdAt;
        Name = name;
        EMail = eMail;
        NormalizedEMail = normalizedEMail;
        PasswordHash = passwordHash;
        IsAdministrator = isAdministrator;
        LastPasswordChange = lastPasswordChange;
        SecurityStamp = securityStamp;
    }

    public override Guid Id { get; protected set; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public string EMail { get; private set; }

    public string NormalizedEMail { get; private set; }

    public string PasswordHash { get; private set; }

    public bool IsAdministrator { get; set; }

    public DateTime LastPasswordChange { get; private set; }

    public DateTime? LastLogin { get; private set; }

    public Guid SecurityStamp { get; private set; }

    public IReadOnlyList<Organization.Organization> Organizations => _organizations.AsReadOnly();

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

    public void UpdateEmail(string newEmail)
    {
        EMail = newEmail;
        NormalizedEMail = NormalizeEmail(newEmail);
        SecurityStamp = Guid.NewGuid();
    }

    public static string NormalizeEmail(string email) => email.Trim().ToUpper();
}
