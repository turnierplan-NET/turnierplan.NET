using Turnierplan.Core.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.TeamSelectors;

public abstract class TeamSelectorTestBase
{
    protected readonly Tournament Tournament;
    protected readonly Team[] Teams;
    protected readonly Group GroupA, GroupB, GroupC;

    protected TeamSelectorTestBase()
    {
        Tournament = new Tournament(string.Empty);

        Teams = [.. Enumerable.Range(0, 12).Select(i => Tournament.AddTeam($"Team {i}"))];

        GroupA = Tournament.AddGroup('A');
        GroupB = Tournament.AddGroup('B');
        GroupC = Tournament.AddGroup('C');

        Tournament.AddGroupParticipant(GroupA, Teams[0]).Statistics = new TeamGroupStatistics { Position = 4, Points = 0 };
        Tournament.AddGroupParticipant(GroupA, Teams[1]).Statistics = new TeamGroupStatistics { Position = 2, Points = 6 };
        Tournament.AddGroupParticipant(GroupA, Teams[2]).Statistics = new TeamGroupStatistics { Position = 3, Points = 3 };
        Tournament.AddGroupParticipant(GroupA, Teams[3]).Statistics = new TeamGroupStatistics { Position = 1, Points = 9 };

        Tournament.AddGroupParticipant(GroupC, Teams[8]).Statistics = new TeamGroupStatistics { Position = 1, Points = 5 };
        Tournament.AddGroupParticipant(GroupC, Teams[9]).Statistics = new TeamGroupStatistics { Position = 2, Points = 4 };
        Tournament.AddGroupParticipant(GroupC, Teams[10]).Statistics = new TeamGroupStatistics { Position = 3, Points = 3 };
        Tournament.AddGroupParticipant(GroupC, Teams[11]).Statistics = new TeamGroupStatistics { Position = 4, Points = 2 };

        Tournament._matches.Add(new Match(1, new NullSelector(), new NullSelector(), GroupA)
        {
            Court = 0,
            TeamA = Teams[4],
            TeamB = Teams[5],
            ScoreA = 1,
            ScoreB = 0,
            OutcomeType = MatchOutcomeType.Standard
        });

        Tournament._matches.Add(new Match(2, new NullSelector(), new NullSelector(), GroupA)
        {
            Court = 0,
            TeamA = Teams[6],
            TeamB = Teams[7],
            ScoreA = 1,
            ScoreB = 3,
            OutcomeType = MatchOutcomeType.Standard
        });

        Tournament._matches.Add(new Match(3, new NullSelector(), new NullSelector(), GroupB)
        {
            Court = 0,
            TeamA = Teams[8],
            TeamB = Teams[9],
            ScoreA = 2,
            ScoreB = 2,
            OutcomeType = MatchOutcomeType.Standard
        });

        Tournament._matches.Add(new Match(4, new NullSelector(), new NullSelector(), GroupB)
        {
            Court = 0,
            TeamA = Teams[10],
            TeamB = Teams[11],
            ScoreA = null,
            ScoreB = null,
            OutcomeType = null
        });
    }
}
