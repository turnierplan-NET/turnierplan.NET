using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament.TeamSelectors;

public abstract class TeamSelectorTestBase
{
    private Core.Tournament.Tournament? _testTournament;

    protected Core.Tournament.Tournament TestTournament => _testTournament ??= MakeTestTournament();

    private static Core.Tournament.Tournament MakeTestTournament()
    {
        var tournament = Tournament.TestTournament.Default;

        var teams = new Dictionary<int, Team>();
        foreach (var id in Enumerable.Range(4, 12))
        {
            teams[id] = new Team(id, null!, string.Empty);
        }

        var groupA = new Group(100, 'A');
        tournament._groups.Add(groupA);

        var groupB = new Group(101, 'B');
        tournament._groups.Add(groupB);

        var groupC = new Group(102, 'C');
        tournament._groups.Add(groupC);

        groupA._participants.Add(new GroupParticipant(groupA, teams[4], 0, 0) { Statistics = { Position = 4, Points = 0 }});
        groupA._participants.Add(new GroupParticipant(groupA, teams[5], 0, 0) { Statistics = { Position = 2, Points = 6 }});
        groupA._participants.Add(new GroupParticipant(groupA, teams[6], 0, 0) { Statistics = { Position = 3, Points = 3 }});
        groupA._participants.Add(new GroupParticipant(groupA, teams[7], 0, 0) { Statistics = { Position = 1, Points = 9 }});

        groupC._participants.Add(new GroupParticipant(groupC, teams[12], 0, 0) { Statistics = { Position = 1, Points = 5 }});
        groupC._participants.Add(new GroupParticipant(groupC, teams[13], 0, 0) { Statistics = { Position = 2, Points = 4 }});
        groupC._participants.Add(new GroupParticipant(groupC, teams[14], 0, 0) { Statistics = { Position = 3, Points = 3 }});
        groupC._participants.Add(new GroupParticipant(groupC, teams[15], 0, 0) { Statistics = { Position = 4, Points = 2 }});

        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), groupA)
        {
            Court = 0,
            TeamA = teams[4],
            TeamB = teams[5],
            ScoreA = 1,
            ScoreB = 0,
            OutcomeType = MatchOutcomeType.Standard
        });

        tournament._matches.Add(new Match(2, 2, new NullSelector(), new NullSelector(), groupA)
        {
            Court = 0,
            TeamA = teams[6],
            TeamB = teams[7],
            ScoreA = 1,
            ScoreB = 3,
            OutcomeType = MatchOutcomeType.Standard
        });

        tournament._matches.Add(new Match(3, 3, new NullSelector(), new NullSelector(), groupB)
        {
            Court = 0,
            TeamA = teams[8],
            TeamB = teams[9],
            ScoreA = 2,
            ScoreB = 2,
            OutcomeType = MatchOutcomeType.Standard
        });

        tournament._matches.Add(new Match(4, 4, new NullSelector(), new NullSelector(), groupB)
        {
            Court = 0,
            TeamA = teams[10],
            TeamB = teams[11],
            ScoreA = null,
            ScoreB = null,
            OutcomeType = null
        });

        return tournament;
    }
}
