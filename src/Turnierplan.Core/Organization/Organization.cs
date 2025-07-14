using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Organization;

public sealed class Organization : Entity<long>, IEntityWithRoleAssignments<Organization>
{
    internal readonly List<RoleAssignment<Organization>> _roleAssignments = new();
    internal readonly List<ApiKey.ApiKey> _apiKeys = new();
    internal readonly List<Folder.Folder> _folders = new();
    internal readonly List<Image.Image> _images = new();
    internal readonly List<PlanningRealm.PlanningRealm> _planningRealms = new();
    internal readonly List<Tournament.Tournament> _tournaments = new();
    internal readonly List<Venue.Venue> _venues = new();

    public Organization(string name)
    {
        Id = 0;
        PublicId = new PublicId.PublicId();
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    internal Organization(long id, PublicId.PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public IReadOnlyList<RoleAssignment<Organization>> RoleAssignments => _roleAssignments.AsReadOnly();

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public IReadOnlyList<ApiKey.ApiKey> ApiKeys => _apiKeys.AsReadOnly();

    public IReadOnlyList<Folder.Folder> Folders => _folders.AsReadOnly();

    public IReadOnlyList<Image.Image> Images => _images.AsReadOnly();

    public IReadOnlyList<PlanningRealm.PlanningRealm> PlanningRealms => _planningRealms.AsReadOnly();

    public IReadOnlyList<Tournament.Tournament> Tournaments => _tournaments.AsReadOnly();

    public IReadOnlyList<Venue.Venue> Venues => _venues.AsReadOnly();

    public RoleAssignment<Organization> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<Organization>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<Organization> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }
}
