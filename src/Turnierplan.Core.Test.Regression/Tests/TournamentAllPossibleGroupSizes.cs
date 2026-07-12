using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression.Tests;

/// <summary>
/// This test creates match plans for all supported tournament group sizes. This test
/// was written to ensure the correct migration from JSON-based group match definitions
/// to the C#-based definitions.
/// </summary>
internal sealed class TournamentAllPossibleGroupSizes : TournamentRegressionTestBase
{
    public TournamentAllPossibleGroupSizes()
    {
        var counter = 1;
        var group = Step(t => t.AddGroup('A', "Test"));

        // Initially for group with 2 teams
        Step(t =>
        {
            var t1 = t.AddTeam($"Test {counter++}");
            var t2 = t.AddTeam($"Test {counter++}");

            t.AddGroupParticipant(group, t1);
            t.AddGroupParticipant(group, t2);

            t.GenerateMatchPlan(new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                }
            });
        });

        Step(AddAnotherTeam); // group with 3 teams
        Step(AddAnotherTeam); // group with 4 teams
        Step(AddAnotherTeam); // group with 5 teams
        Step(AddAnotherTeam); // group with 6 teams
        Step(AddAnotherTeam); // group with 7 teams
        Step(AddAnotherTeam); // group with 8 teams
        Step(AddAnotherTeam); // group with 9 teams

        // Adding another team throws an exception because there exists no config for 10 teams
        ExceptionalStep<TurnierplanException>(AddAnotherTeam);

        return;

        void AddAnotherTeam(Tournament.Tournament t)
        {
            var tNext = t.AddTeam($"Test {counter++}");
            t.AddGroupParticipant(group, tNext);

            t.GenerateMatchPlan(new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                }
            }, clearMatches: true);
        }
    }
}
