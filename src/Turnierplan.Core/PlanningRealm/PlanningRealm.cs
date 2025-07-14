using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class PlanningRealm : Entity<long>, IEntityWithRoleAssignments<PlanningRealm>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<PlanningRealm>> _roleAssignments = new();
    internal readonly List<TournamentClass> _tournamentClasses = new();
    internal readonly List<InvitationLink> _invitationLinks = new();
    internal readonly List<Application> _applications = new();

    public PlanningRealm(Organization.Organization organization, string name)
    {
        organization._planningRealms.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Organization = organization;
        CreatedAt = DateTime.UtcNow;
        Name = name;
    }

    internal PlanningRealm(long id, PublicId.PublicId publicId, DateTime createdAt, string name)
    {
        Id = id;
        PublicId = publicId;
        CreatedAt = createdAt;
        Name = name;
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public IReadOnlyList<RoleAssignment<PlanningRealm>> RoleAssignments => _roleAssignments.AsReadOnly();

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public IReadOnlyList<TournamentClass> TournamentClasses => _tournamentClasses.AsReadOnly();

    public IReadOnlyList<InvitationLink> InvitationLinks => _invitationLinks.AsReadOnly();

    public IReadOnlyList<Application> Applications => _applications.AsReadOnly();

    public RoleAssignment<PlanningRealm> AddRoleAssignment(Role role, Principal principal)
    {
        var roleAssignment = new RoleAssignment<PlanningRealm>(this, role, principal);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public void RemoveRoleAssignment(RoleAssignment<PlanningRealm> roleAssignment)
    {
        _roleAssignments.Remove(roleAssignment);
    }
}
