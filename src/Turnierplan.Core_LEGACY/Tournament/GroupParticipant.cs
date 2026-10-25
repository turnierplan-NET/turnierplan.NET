namespace Turnierplan.Core.Tournament;

public sealed class GroupParticipant : IComparableTeam
{
    internal GroupParticipant(Group group, Team team, int order, int priority)
    {
        Group = group;
        Team = team;
        Order = order;
        Priority = priority;
        Statistics = new TeamGroupStatistics();
    }

    internal GroupParticipant(int order, int priority)
    {
        Order = order;
        Priority = priority;
        Statistics = new TeamGroupStatistics();
    }

    public Group Group { get; internal set; } = null!;

    public Team Team { get; internal set; } = null!;

    public int Order { get; set; }

    public int Priority { get; set; }

    public TeamGroupStatistics Statistics { get; internal set; }

    bool IComparableTeam.HasAssociatedGroup => true;

    Group IComparableTeam.AssociatedGroup => Group;
}
