using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression.Tests;

internal sealed class TournamentUnequalGroupsCourtDistribution : TournamentRegressionTestBase
{
    public TournamentUnequalGroupsCourtDistribution()
    {
        var groupA = Step(t => t.AddGroup('A'));
        var groupB = Step(t => t.AddGroup('B'));

        Step(t =>
        {
            var team1 = t.AddTeam("Test 1");
            var team2 = t.AddTeam("Test 2");
            var team3 = t.AddTeam("Test 3");
            var team4 = t.AddTeam("Test 4");
            var team5 = t.AddTeam("Test 5");
            var team6 = t.AddTeam("Test 6");
            var team7 = t.AddTeam("Test 7");
            var team8 = t.AddTeam("Test 8");
            var team9 = t.AddTeam("Test 9");
            var team10 = t.AddTeam("Test 10");
            var team11 = t.AddTeam("Test 11");

            t.AddGroupParticipant(groupA, team1);
            t.AddGroupParticipant(groupA, team2);
            t.AddGroupParticipant(groupA, team3);
            t.AddGroupParticipant(groupA, team4);
            t.AddGroupParticipant(groupA, team5);
            t.AddGroupParticipant(groupA, team6);

            t.AddGroupParticipant(groupB, team7);
            t.AddGroupParticipant(groupB, team8);
            t.AddGroupParticipant(groupB, team9);
            t.AddGroupParticipant(groupB, team10);
            t.AddGroupParticipant(groupB, team11);
        });

        var cfg = new MatchPlanConfiguration
        {
            GroupRoundConfig = new GroupRoundConfig
            {
                GroupMatchOrder = GroupMatchOrder.Sequential,
                GroupPhaseRounds = 1
            },
            FinalsRoundConfig = null,
            ScheduleConfig = new ScheduleConfig
            {
                GroupPhaseNumberOfCourts = 1,
                FirstMatchKickoff = DateTime.UnixEpoch,
                GroupPhasePlayTime = TimeSpan.FromMinutes(10),
                GroupPhasePauseTime = TimeSpan.FromMinutes(1),
                PauseBetweenGroupAndFinalsPhase = TimeSpan.Zero,
                FinalsPhaseNumberOfCourts = 0,
                FinalsPhasePlayTime = TimeSpan.Zero,
                FinalsPhasePauseTime = TimeSpan.Zero
            }
        };

        Step(t => t.GenerateMatchPlan(cfg));

        cfg.ScheduleConfig.AllowMultipleGroupsParallel = true;
        cfg.ScheduleConfig.GroupPhaseNumberOfCourts = 2;
        Step(t => t.GenerateMatchPlan(cfg, clearMatches: true));

        cfg.ScheduleConfig.AllowMultipleGroupsParallel = false;
        cfg.ScheduleConfig.GroupPhaseNumberOfCourts = 2;
        Step(t => t.GenerateMatchPlan(cfg, clearMatches: true));

        cfg.ScheduleConfig.AllowMultipleGroupsParallel = true;
        cfg.ScheduleConfig.GroupPhaseNumberOfCourts = 3;
        Step(t => t.GenerateMatchPlan(cfg, clearMatches: true));

        cfg.ScheduleConfig.AllowMultipleGroupsParallel = false;
        cfg.ScheduleConfig.GroupPhaseNumberOfCourts = 3;
        Step(t => t.GenerateMatchPlan(cfg, clearMatches: true));
    }
}
