using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Application : Entity<long>
{
    internal readonly List<ApplicationTeam> _teams = [];

    internal Application(long id, int tag, DateTime createdAt, string notes, string contact, string? contactEmail, string? contactTelephone, string? comment)
    {
        Id = id;
        Tag = tag;
        CreatedAt = createdAt;
        Notes = notes;
        Contact = contact;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
        Comment = comment;
    }

    internal Application(PlanningRealm planningRealm, InvitationLink? sourceLink, int tag, string contact)
    {
        Id = 0;
        PlanningRealm = planningRealm;
        SourceLink = sourceLink;
        Tag = tag;
        CreatedAt = DateTime.UtcNow;
        Notes = string.Empty;
        Contact = contact;
    }

    public override long Id { get; protected set; }

    public PlanningRealm PlanningRealm { get; internal set; } = null!;

    public InvitationLink? SourceLink { get; internal set; }

    public int Tag { get; set; }

    public DateTime CreatedAt { get; }

    public string Notes { get; set; }

    public string Contact { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTelephone { get; set; }

    public string? Comment { get; set; }

    public IReadOnlyList<ApplicationTeam> Teams => _teams.AsReadOnly();

    public void AddTeam(TournamentClass tournamentClass, string name)
    {
        var team = new ApplicationTeam(this, tournamentClass, name);
        _teams.Add(team);
    }
}
