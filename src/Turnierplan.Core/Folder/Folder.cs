using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Folder;

public sealed class Folder : Entity<long>, IEntityWithRoleAssignments<Folder>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<Folder>> _roleAssignments = new();
    internal readonly List<Tournament.Tournament> _tournaments = new();

    public Folder(Organization.Organization organization, string name)
    {
        organization._folders.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Organization = organization;
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    internal Folder(long id, PublicId.PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public IReadOnlyList<RoleAssignment<Folder>> RoleAssignments => _roleAssignments.AsReadOnly();

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public IReadOnlyList<Tournament.Tournament> Tournaments => _tournaments.AsReadOnly();

    public RoleAssignment<Folder> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<Folder>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<Folder> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }
}
