using Turnierplan.Core.Exceptions;
using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.PlanningRealm;

public sealed class ApplicationTeam : Entity<long>
{
    internal readonly List<Label> _labels = [];

    internal ApplicationTeam(long id, string name)
    {
        Id = id;
        Name = name;
    }

    internal ApplicationTeam(Application application, TournamentClass @class, string name)
    {
        Id = 0;
        Application = application;
        Class = @class;
        Name = name;
    }

    public override long Id { get; protected set; }

    public string Name { get; private set; }

    public Application Application { get; internal set; } = null!;

    public TournamentClass Class { get; internal set; } = null!;

    public IReadOnlyList<Label> Labels => _labels.AsReadOnly();

    public TeamLink? TeamLink { get; internal set; }

    public void SetName(string name)
    {
        name = name.Trim();

        if (!Name.Equals(name))
        {
            Application.AddChangeLog(ApplicationChangeLogType.TeamRenamed, Name, name);
        }

        Name = name;

        if (TeamLink is not null)
        {
            TeamLink.Team.Name = name;
        }
    }

    // TODO: Change log for labels?

    public void AddLabel(Label label)
    {
        if (_labels.Contains(label))
        {
            throw new TurnierplanException("The specified label is already assigned to this application team.");
        }

        _labels.Add(label);
    }

    public void RemoveAllLabels()
    {
        _labels.Clear();
    }
}
