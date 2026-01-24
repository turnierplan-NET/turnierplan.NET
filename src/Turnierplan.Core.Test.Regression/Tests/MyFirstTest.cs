using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression.Tests;

internal sealed class MyFirstTest : RegressionTestBase<Tournament.Tournament>
{
    protected override Tournament.Tournament CreateSubject()
    {
        return new Tournament.Tournament(new Organization.Organization(string.Empty), "reg_name", Visibility.Private);
    }

    protected override void RunSteps()
    {
        var team1 = Step(t => t.AddTeam("Test 1"));
        var team2 = Step(t => t.AddTeam("Test 2"));

        var group1 = Step(t => t.AddGroup('A'));

        Step(t => t.AddGroupParticipant(group1, team1));
        Step(t => t.AddGroupParticipant(group1, team2));

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
    }
}
