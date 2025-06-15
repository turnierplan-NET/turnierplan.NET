using Turnierplan.Core.Exceptions;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.User;

public sealed class User : Entity<Guid>
{
    internal readonly List<Organization.Organization> _organizations = new();
    internal readonly List<Role> _roles = new();

    public User(string name, string email)
    {
        email = email.Trim();

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        EMail = email;
        NormalizedEMail = NormalizeEmail(email);
        PasswordHash = string.Empty;
        LastPasswordChange = DateTime.MinValue;
        SecurityStamp = Guid.Empty;
    }

    internal User(Guid id, DateTime createdAt, string name, string eMail, string normalizedEMail, string passwordHash, DateTime lastPasswordChange, Guid securityStamp)
    {
        Id = id;
        CreatedAt = createdAt;
        Name = name;
        EMail = eMail;
        NormalizedEMail = normalizedEMail;
        PasswordHash = passwordHash;
        LastPasswordChange = lastPasswordChange;
        SecurityStamp = securityStamp;
    }

    public override Guid Id { get; protected set; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public string EMail { get; private set; }

    public string NormalizedEMail { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTime LastPasswordChange { get; private set; }

    public DateTime? LastLogin { get; private set; }

    public Guid SecurityStamp { get; private set; }

    public IReadOnlyList<Organization.Organization> Organizations => _organizations.AsReadOnly();

    public IReadOnlyList<Role> Roles => _roles.AsReadOnly();

    public void AddRole(Role role)
    {
        if (_roles.Any(x => x.Id == role.Id))
        {
            throw new TurnierplanException($"Role {role.Id} is already assigned to the user.");
        }

        _roles.Add(role);
    }

    public void RemoveRole(Role role)
    {
        _roles.RemoveAll(x => x.Id == role.Id);
    }

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
