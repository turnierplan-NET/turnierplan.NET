using Turnierplan.Core.Exceptions;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class Application : Entity<long>
{
    internal readonly List<ApplicationChangeLog> _changeLog = [];
    internal readonly List<ApplicationTeam> _teams = [];

    private string _notes;
    private string _contact;
    private string? _contactEmail;
    private string? _contactTelephone;
    private string? _comment;

    internal Application(long id, int tag, DateTime createdAt, string notes, string contact, string? contactEmail, string? contactTelephone, string? comment, Guid? formSession)
    {
        Id = id;
        Tag = tag;
        CreatedAt = createdAt;
        FormSession = formSession;

        _notes = notes;
        _contact = contact;
        _contactEmail = contactEmail;
        _contactTelephone = contactTelephone;
        _comment = comment;
    }

    internal Application(PlanningRealm planningRealm, InvitationLink? sourceLink, int tag, string contact)
    {
        Id = 0;
        PlanningRealm = planningRealm;
        SourceLink = sourceLink;
        Tag = tag;
        CreatedAt = DateTime.UtcNow;

        _notes = string.Empty;
        _contact = contact;
    }

    public override long Id { get; protected set; }

    public PlanningRealm PlanningRealm { get; internal set; } = null!;

    public InvitationLink? SourceLink { get; internal set; }

    public int Tag { get; set; }

    public DateTime CreatedAt { get; }

    public IReadOnlyList<ApplicationChangeLog> ChangeLog => _changeLog.AsReadOnly();

    public string Notes
    {
        get => _notes;
        set
        {
            var trimmed = value.Trim();

            if (!_notes.Equals(trimmed))
            {
                AddChangeLog(ApplicationChangeLogType.NotesChanged, _notes, trimmed);
            }

            _notes = trimmed;
        }
    }

    public string Contact
    {
        get => _contact;
        set
        {
            var trimmed = value.Trim();

            if (!_contact.Equals(trimmed))
            {
                AddChangeLog(ApplicationChangeLogType.ContactChanged, _contact, trimmed);
            }

            _contact = trimmed;
        }
    }

    public string? ContactEmail
    {
        get => _contactEmail;
        set
        {
            var trimmed = value?.Trim();

            if (trimmed is not null && trimmed.Length == 0)
            {
                throw new TurnierplanException("Contact email must be null or a non-empty string");
            }

            if (!Equals(_contactEmail, trimmed))
            {
                AddChangeLog(ApplicationChangeLogType.ContactEmailChanged, _contactEmail, trimmed);
            }

            _contactEmail = trimmed;
        }
    }

    public string? ContactTelephone
    {
        get => _contactTelephone;
        set
        {
            var trimmed = value?.Trim();

            if (trimmed is not null && trimmed.Length == 0)
            {
                throw new TurnierplanException("Contact telephone must be null or a non-empty string");
            }

            if (!Equals(_contactTelephone, trimmed))
            {
                AddChangeLog(ApplicationChangeLogType.ContactTelephoneChanged, _contactTelephone, trimmed);
            }

            _contactTelephone = trimmed;
        }
    }

    public string? Comment
    {
        get => _comment;
        set
        {
            var trimmed = value?.Trim();

            if (trimmed is not null && trimmed.Length == 0)
            {
                throw new TurnierplanException("Comment must be null or a non-empty string");
            }

            if (!Equals(_comment, trimmed))
            {
                AddChangeLog(ApplicationChangeLogType.CommentChanged, _comment, trimmed);
            }

            _comment = trimmed;
        }
    }

    public Guid? FormSession { get; set; }

    public IReadOnlyList<ApplicationTeam> Teams => _teams.AsReadOnly();

    public ApplicationTeam AddTeam(TournamentClass tournamentClass, string name)
    {
        var team = new ApplicationTeam(this, tournamentClass, name);
        _teams.Add(team);

        AddChangeLog(ApplicationChangeLogType.TeamAdded, [new ApplicationChangeLog.Property(ApplicationChangeLogProperty.TeamName, team.Name)]);

        return team;
    }

    public void RemoveTeam(ApplicationTeam team)
    {
        if (team.TeamLink is not null)
        {
            throw new TurnierplanException("Removing a team is not allowed while it is linked to a tournament.");
        }

        _teams.Remove(team);
        team.Application = null!;

        AddChangeLog(ApplicationChangeLogType.TeamRemoved, [new ApplicationChangeLog.Property(ApplicationChangeLogProperty.TeamName, team.Name)]);
    }

    public void ClearChangeLog()
    {
        _changeLog.Clear();
    }

    internal void AddChangeLog(ApplicationChangeLogType type, string? previousValue, string? newValue)
    {
        AddChangeLog(type, [
            new ApplicationChangeLog.Property(ApplicationChangeLogProperty.PreviousValue, previousValue ?? string.Empty),
            new ApplicationChangeLog.Property(ApplicationChangeLogProperty.NewValue, newValue ?? string.Empty)
        ]);
    }

    internal void AddChangeLog(ApplicationChangeLogType type, IEnumerable<ApplicationChangeLog.Property> properties)
    {
        _changeLog.Add(new ApplicationChangeLog(this, type, properties));
    }
}
