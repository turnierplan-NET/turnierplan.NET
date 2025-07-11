using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class Realm : Entity<long>, IEntityWithRoleAssignments<Realm>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<Realm>> _roleAssignments = new();

    public Realm(Organization.Organization organization, string name)
    {
        organization._realms.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Organization = organization;
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    internal Realm(long id, PublicId.PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public IReadOnlyList<RoleAssignment<Realm>> RoleAssignments => _roleAssignments.AsReadOnly();

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    // TODO: Add properties
    // Turnierklasse
    // Anmeldungslinks
    // Anmeldungen

    public RoleAssignment<Realm> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<Realm>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<Realm> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }
}
