using Turnierplan.Core.Entity;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.ApiKey;

public sealed class ApiKey : Entity<long>, IEntityWithRoleAssignments<ApiKey>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<ApiKey>> _roleAssignments = [];
    internal readonly List<ApiKeyRequest> _requests = [];

    public ApiKey(Organization.Organization organization, string name, string? description, DateTime expiryDate)
    {
        organization._apiKeys.Add(this);

        Id = 0;
        PrincipalId = Guid.NewGuid();
        PublicId = new PublicId.PublicId();
        Organization = organization;
        Name = name;
        Description = description ?? string.Empty;
        SecretHash = string.Empty;
        CreatedAt = DateTime.UtcNow;
        ExpiryDate = expiryDate;
        IsActive = true;
    }

    internal ApiKey(long id, Guid principalId, PublicId.PublicId publicId, string name, string description, string secretHash, DateTime createdAt, DateTime expiryDate, bool isActive)
    {
        Id = id;
        PrincipalId = principalId;
        PublicId = publicId;
        Name = name;
        Description = description;
        SecretHash = secretHash;
        CreatedAt = createdAt;
        ExpiryDate = expiryDate;
        IsActive = isActive;
    }

    public override long Id { get; protected set; }

    public Guid PrincipalId { get; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public IReadOnlyList<RoleAssignment<ApiKey>> RoleAssignments => _roleAssignments.AsReadOnly();

    public string Name { get; }

    public string Description { get; }

    public string SecretHash { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime ExpiryDate { get; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

    public bool IsActive { get; set; }

    public IReadOnlyList<ApiKeyRequest> Requests => _requests.AsReadOnly();

    public RoleAssignment<ApiKey> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<ApiKey>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<ApiKey> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }

    public void AssignNewSecret(Func<string, string> secretHashFunc, out string plainTextSecret)
    {
        plainTextSecret = GenerateSecret();
        SecretHash = secretHashFunc(plainTextSecret);
    }

    public void AddRequest(ApiKeyRequest request)
    {
        _requests.Add(request);
    }

    private static string GenerateSecret()
    {
        const string alphabet = "ABCDEFGHIJKMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const int length = 32;

        var value = Enumerable.Range(0, length)
            .Select(_ => alphabet[Random.Shared.Next(alphabet.Length)])
            .ToArray();

        return new string(value);
    }
}
