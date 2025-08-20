using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Application : Entity<long>
{
    internal readonly List<ApplicationTeam> _teams = new();

    internal Application(long id, int tag, DateTime createdAt, string name, string notes, string contact, string? contactEmail, string? contactTelephone, string? comment)
    {
        Id = id;
        Tag = tag;
        CreatedAt = createdAt;
        Name = name;
        Notes = notes;
        Contact = contact;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
        Comment = comment;
    }

    internal Application(InvitationLink? sourceLink, int tag, string name, string contact)
    {
        Id = 0;
        SourceLink = sourceLink;
        Tag = tag;
        CreatedAt = DateTime.UtcNow;
        Name = name;
        Notes = string.Empty;
        Contact = contact;
    }

    public override long Id { get; protected set; }

    public InvitationLink? SourceLink { get; internal set; }

    public int Tag { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }

    public string Contact { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTelephone { get; set; }

    public string? Comment { get; set; }

    public IReadOnlyList<ApplicationTeam> Teams => _teams.AsReadOnly();

    public void AddTeam(TournamentClass tournamentClass)
    {
        var team = new ApplicationTeam(tournamentClass);
        _teams.Add(team);
    }
}
