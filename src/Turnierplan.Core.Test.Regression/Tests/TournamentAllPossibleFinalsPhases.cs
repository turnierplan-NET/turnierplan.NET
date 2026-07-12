using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression.Tests;

/// <summary>
/// This test creates match plans for all supported finals phases. This test was written to ensure
/// the correct migration from JSON-based group match definitions to the C#-based definitions.
/// </summary>
internal sealed class TournamentAllPossibleFinalsPhases : TournamentRegressionTestBase
{
    public TournamentAllPossibleFinalsPhases()
    {
        // Set as low as possible to reduce size of expected test output
        const int teamsCountPerGroup = 4;

        var groupCounter = 'A';
        var teamCounter = 1;

        // -- 1 group --
        Step(AddAnotherGroup);
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));

        // -- 2 groups --
        Step(AddAnotherGroup);
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.EighthFinals));

        // -- 3 groups --
        Step(AddAnotherGroup);
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.EighthFinals));

        // -- 4 groups --
        Step(AddAnotherGroup);
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.EighthFinals));

        // -- 5 groups --
        Step(AddAnotherGroup);
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.EighthFinals));

        // -- 6 groups --
        Step(AddAnotherGroup);
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        Step(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.EighthFinals));

        // -- 7 groups --
        Step(AddAnotherGroup);
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.FinalOnly));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.SemiFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.QuarterFinals));
        StepWithException<TurnierplanException>(t => GenerateMatchPlan(t, FinalsRoundOrder.EighthFinals));

        return;

        void AddAnotherGroup(Tournament.Tournament t)
        {
            var teams = Enumerable.Range(0, teamsCountPerGroup).Select(_ => t.AddTeam($"Team {teamCounter++}")).ToList();
            var group = t.AddGroup(groupCounter, $"Test {groupCounter++}");
            teams.ForEach(team => t.AddGroupParticipant(group, team));
        }

        void GenerateMatchPlan(Tournament.Tournament t, FinalsRoundOrder firstRound)
        {
            t.GenerateMatchPlan(new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = firstRound,
                    EnableThirdPlacePlayoff = false,
                    TeamSelectors = null,
                    AdditionalPlayoffs = null
                }
            }, clearMatches: true);
        }
    }
}
