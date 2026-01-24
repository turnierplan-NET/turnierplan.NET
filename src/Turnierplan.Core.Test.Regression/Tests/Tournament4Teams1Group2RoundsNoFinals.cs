using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression.Tests;

internal sealed class Tournament4Teams1Group2RoundsNoFinals : TournamentRegressionTestBase
{
    public Tournament4Teams1Group2RoundsNoFinals()
    {
        Subject(new Tournament.Tournament(new Organization.Organization(string.Empty), "reg_name", Visibility.Private));

        var team1 = Step(t => t.AddTeam("Test 1"));
        var team2 = Step(t => t.AddTeam("Test 2"));
        var team3 = Step(t => t.AddTeam("Test 3"));
        var team4 = Step(t => t.AddTeam("Test 4"));

        var group1 = Step(t => t.AddGroup('A'));

        Step(t => t.AddGroupParticipant(group1, team1));
        Step(t => t.AddGroupParticipant(group1, team2));
        Step(t => t.AddGroupParticipant(group1, team3));
        Step(t => t.AddGroupParticipant(group1, team4));

        Step(t => t.GenerateMatchPlan(new MatchPlanConfiguration
        {
            GroupRoundConfig = new GroupRoundConfig
            {
                GroupMatchOrder = GroupMatchOrder.Alternating,
                GroupPhaseRounds = 2
            },
            FinalsRoundConfig = null,
            ScheduleConfig = null
        }));

        Step(t => t.Compute());
        Step(t => t.Matches[0].SetOutcome(false, 1, 0, MatchOutcomeType.Standard));
        Step(t => t.Compute());
        Step(t => t.Matches[1].SetOutcome(false, 2, 2, MatchOutcomeType.Standard));
        Step(t => t.Matches[2].SetOutcome(false, 3, 1, MatchOutcomeType.Standard));
        Step(t => t.Compute());
        Step(t => t.Matches[3].SetOutcome(false, 4, 6, MatchOutcomeType.Standard));
        Step(t => t.Matches[4].SetOutcome(false, 0, 0, MatchOutcomeType.Standard));
        Step(t => t.Matches[5].SetOutcome(false, 2, 2, MatchOutcomeType.Standard));
        Step(t => t.Matches[6].SetOutcome(false, 3, 2, MatchOutcomeType.Standard));
        Step(t => t.Compute());
        Step(t => t.Matches[7].SetOutcome(false, 1, 1, MatchOutcomeType.Standard));
        Step(t => t.Matches[8].SetOutcome(false, 0, 3, MatchOutcomeType.Standard));
        Step(t => t.Matches[9].SetOutcome(false, 2, 0, MatchOutcomeType.Standard));
        Step(t => t.Matches[10].SetOutcome(false, 0, 2, MatchOutcomeType.Standard));
        Step(t => t.Matches[11].SetOutcome(false, 2, 1, MatchOutcomeType.Standard));
        Step(t => t.Compute());
    }
}
