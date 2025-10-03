using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Application : Entity<long>
{
    internal readonly List<ApplicationChangeLog> _changeLog = [];
    internal readonly List<ApplicationTeam> _teams = [];

    internal Application(long id, int tag, DateTime createdAt, string notes, string contact, string? contactEmail, string? contactTelephone, string? comment, Guid? formSession)
    {
        Id = id;
        Tag = tag;
        CreatedAt = createdAt;
        Notes = notes;
        Contact = contact;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
        Comment = comment;
        FormSession = formSession;
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

    public IReadOnlyList<ApplicationChangeLog> ChangeLog => _changeLog.AsReadOnly();

    public string Notes { get; private set; }

    public string Contact { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTelephone { get; set; }

    public string? Comment { get; set; }

    public Guid? FormSession { get; set; }

    public IReadOnlyList<ApplicationTeam> Teams => _teams.AsReadOnly();

    public void AddTeam(TournamentClass tournamentClass, string name)
    {
        var team = new ApplicationTeam(this, tournamentClass, name);
        _teams.Add(team);
    }

    public void SetNotes(string notes)
    {
        notes = notes.Trim();

        if (!Notes.Equals(notes))
        {
            AddChangeLog(ApplicationChangeLogType.NotesChanged, Notes, notes);
        }

        Notes = notes;
    }

    private void AddChangeLog(ApplicationChangeLogType type, string? oldValue, string? newValue)
    {
        _changeLog.Add(new ApplicationChangeLog(this, type, oldValue, newValue));
    }
}
