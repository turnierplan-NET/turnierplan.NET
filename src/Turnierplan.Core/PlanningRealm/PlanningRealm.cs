using Turnierplan.Core.Entity;
using Turnierplan.Core.RoleAssignment;

namespace Turnierplan.Core.PlanningRealm;

public sealed class PlanningRealm : Entity<long>, IEntityWithRoleAssignments<PlanningRealm>, IEntityWithOrganization
{
    internal readonly List<RoleAssignment<PlanningRealm>> _roleAssignments = [];
    internal readonly List<TournamentClass> _tournamentClasses = [];
    internal readonly List<InvitationLink> _invitationLinks = [];
    internal readonly List<Application> _applications = [];
    internal readonly List<Label> _labels = [];

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

    public IReadOnlyList<Label> Labels => _labels.AsReadOnly();

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

    public TournamentClass AddTournamentClass(string name)
    {
        var tournamentClass = new TournamentClass(name);
        _tournamentClasses.Add(tournamentClass);

        return tournamentClass;
    }

    public InvitationLink AddInvitationLink(string name)
    {
        var invitationLink = new InvitationLink(name);
        _invitationLinks.Add(invitationLink);

        invitationLink.PlanningRealm = this;

        return invitationLink;
    }

    public Application AddApplication(InvitationLink? sourceLink, string contact)
    {
        var tag = GenerateApplicationTag();
        var application = new Application(this, sourceLink, tag, contact);
        _applications.Add(application);

        return application;
    }

    public Label AddLabel(string name)
    {
        var label = new Label(name);
        _labels.Add(label);

        return label;
    }

    public void RemoveTournamentClass(TournamentClass tournamentClass)
    {
        _tournamentClasses.Remove(tournamentClass);
    }

    public void RemoveInvitationLink(InvitationLink invitationLink)
    {
        _invitationLinks.Remove(invitationLink);
    }

    public void RemoveLabel(Label label)
    {
        foreach (var team in _applications.SelectMany(x => x._teams))
        {
            team._labels.Remove(label);
        }

        _labels.Remove(label);
    }

    private int GenerateApplicationTag()
    {
        var attempt = 1;
        const int maxAttempts = 10;

        int tag;
        bool isDuplicate;

        do
        {
            // create a random 6-digit tag
            tag = 100_000 + Random.Shared.Next(900_000);

            isDuplicate = _applications.Any(x => x.Tag == tag);
        } while (isDuplicate && attempt++ < maxAttempts);

        return tag;
    }
}
