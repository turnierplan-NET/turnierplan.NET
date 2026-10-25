using FluentAssertions.Extensions;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression.Tests;

internal sealed class Tournament8Teams2Groups1RoundSemiFinals : TournamentRegressionTestBase
{
    public Tournament8Teams2Groups1RoundSemiFinals()
    {
        var team1 = Step(t => t.AddTeam("Test 1"));
        var team2 = Step(t => t.AddTeam("Test 2"));
        var team3 = Step(t => t.AddTeam("Test 3"));
        var team4 = Step(t => t.AddTeam("Test 4"));
        var team5 = Step(t => t.AddTeam("Test 5"));
        var team6 = Step(t => t.AddTeam("Test 6"));
        var team7 = Step(t => t.AddTeam("Test 7"));
        var team8 = Step(t => t.AddTeam("Test 8"));

        var group1 = Step(t => t.AddGroup('A', "Apples"));
        var group2 = Step(t => t.AddGroup('B', "Bananas"));

        Step(t =>
        {
            t.AddGroupParticipant(group1, team1);
            t.AddGroupParticipant(group1, team2);
            t.AddGroupParticipant(group1, team3);
            t.AddGroupParticipant(group1, team4);

            t.AddGroupParticipant(group2, team5);
            t.AddGroupParticipant(group2, team6);
            t.AddGroupParticipant(group2, team7);
            t.AddGroupParticipant(group2, team8);
        });

        Step(t => t.GenerateMatchPlan(new MatchPlanConfiguration
        {
            GroupRoundConfig = new GroupRoundConfig
            {
                GroupMatchOrder = GroupMatchOrder.Sequential,
                GroupPhaseRounds = 1
            },
            FinalsRoundConfig = new FinalsRoundConfig
            {
                FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                EnableThirdPlacePlayoff = true,
                TeamSelectors = null,
                AdditionalPlayoffs = null
            },
            ScheduleConfig = new ScheduleConfig
            {
                FirstMatchKickoff = new DateTime(2026, 1, 1, 16, 0, 0),
                GroupPhaseNumberOfCourts = 1,
                GroupPhasePlayTime = 10.Minutes(),
                GroupPhasePauseTime = 2.Minutes(),
                PauseBetweenGroupAndFinalsPhase = 5.Minutes(),
                FinalsPhaseNumberOfCourts = 1,
                FinalsPhasePlayTime = 12.Minutes(),
                FinalsPhasePauseTime = 2.Minutes()
            }
        }));

        Step(t =>
        {
            t.Matches[0].SetOutcome(false, 1, 0, MatchOutcomeType.Standard);
            t.Matches[1].SetOutcome(false, 2, 1, MatchOutcomeType.Standard);
            t.Matches[2].SetOutcome(false, 1, 3, MatchOutcomeType.Standard);
            t.Matches[3].SetOutcome(false, 2, 2, MatchOutcomeType.Standard);
            t.Matches[4].SetOutcome(false, 0, 0, MatchOutcomeType.Standard);
            t.Matches[5].SetOutcome(false, 1, 1, MatchOutcomeType.Standard);
            t.Compute();
        });

        Step(t =>
        {
            t.Matches[6].SetOutcome(false, 1, 2, MatchOutcomeType.Standard);
            t.Matches[7].SetOutcome(false, 4, 2, MatchOutcomeType.Standard);
            t.Matches[8].SetOutcome(false, 0, 2, MatchOutcomeType.Standard);
            t.Matches[9].SetOutcome(false, 5, 1, MatchOutcomeType.Standard);
            t.Matches[10].SetOutcome(false, 2, 2, MatchOutcomeType.Standard);
            t.Matches[11].SetOutcome(false, 3, 2, MatchOutcomeType.Standard);
            t.Compute();
        });

        Step(t => t.Matches[12].SetOutcome(false, 3, 1, MatchOutcomeType.AfterOvertime));
        Step(t => t.Matches[13].SetOutcome(false, 6, 7, MatchOutcomeType.AfterPenalties));
        Step(t => t.Compute());

        Step(t =>
        {
            t.Matches[14].SetOutcome(false, 3, 0, MatchOutcomeType.Standard);
            t.Matches[15].SetOutcome(true, 1, 2, MatchOutcomeType.Standard);
            t.Compute();
        });

        Step(t =>
        {
            t.Matches[15].SetOutcome(false, 1, 4, MatchOutcomeType.Standard);
            t.Compute();
        });
    }
}
