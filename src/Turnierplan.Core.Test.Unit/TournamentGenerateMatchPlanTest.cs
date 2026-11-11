using FluentAssertions;
using FluentAssertions.Extensions;
using Turnierplan.Core.Configuration;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit;

public sealed class TournamentGenerateMatchPlanTest
{
    private static readonly Group[] __groups = [..Enumerable.Range(0, 5).Select(x => new Group((char)('A' + x)))];

    private static readonly ScheduleConfig __scheduleConfig = new()
    {
        FirstMatchKickoff = KickoffAt(12, 0),
        GroupPhaseNumberOfCourts = 1,
        GroupPhasePlayTime = 10.Minutes(),
        GroupPhasePauseTime = 1.Minutes(),
        PauseBetweenGroupAndFinalsPhase = 0.Minutes(),
        FinalsPhaseNumberOfCourts = 1,
        FinalsPhasePlayTime = 15.Minutes(),
        FinalsPhasePauseTime = 2.Minutes()
    };

    public static readonly TheoryData<int[], MatchPlanConfiguration, MatchExpectation[]> ValidTestData = new()
    {
        {
            // Empty tournament
            [],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            []
        },
        {
            // Single group with 3 teams, no finals round
            [3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0))
            ]
        },
        {
            // Single group with 3 teams and two group phase rounds, no finals round
            [3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 2
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2))
            ]
        },
        {
            // Single group with 3 teams and three group phase rounds, no finals round
            [3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 3
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 17), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0))
            ]
        },
        {
            // Two group with 4 teams each, sequential match ordering and no finals round
            [4, 4],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 6), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 17), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 39), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 50), 0, new GroupDefinitionTeamSelector(__groups[1], 3), new GroupDefinitionTeamSelector(__groups[1], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(14, 1), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2))
            ]
        },
        {
            // Two group with 4 teams each, alternating match ordering and no finals round
            // Also: Played on multiple courts, so kickoff times are parallel
            [4, 4],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig with { GroupPhaseNumberOfCourts = 2 },
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 0), 1, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 11), 1, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 22), 1, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 1, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 44), 1, new GroupDefinitionTeamSelector(__groups[1], 3), new GroupDefinitionTeamSelector(__groups[1], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 55), 1, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2))
            ]
        },
        {
            // One group with 4 teams, alternating match ordering and no finals round
            // Also: Played on "too many" courts, so kickoff times are somewhat parallel
            [4],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig with { GroupPhaseNumberOfCourts = 3 },
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 1, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 2, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 1, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 2, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2))
            ]
        },
        {
            // Two group with 2 and 3 teams respectively, sequential match ordering and no finals round
            // Also: No schedule specified, therefore matches are generated w/o kickoff
            [2, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = null,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, null, 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[1], null, null, null, 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),
                new MatchExpectation(__groups[1], null, null, null, 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2)),
                new MatchExpectation(__groups[1], null, null, null, 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 0))
            ]
        },
        {
            // Two group with 4 and 3 teams respectively, sequential match ordering and no finals round
            [4, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 17), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 28), 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 0))
            ]
        },
        {
            // Two group with 4 and 3 teams respectively, alternating match ordering and no finals round
            [4, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 0)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 17), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2))
            ]
        },
        {
            // Single group with 2 teams, finals round
            [2],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.FinalOnly,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs = null
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(null, 0, 1, KickoffAt(12, 11), 0, new GroupResultsTeamSelector(__groups[0], 1), new GroupResultsTeamSelector(__groups[0], 2))
            ]
        },
        {
            // Single group with 2 teams, finals round and custom pause between group and finals round
            [2],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig with
                {
                    PauseBetweenGroupAndFinalsPhase = 10.Minutes()
                },
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.FinalOnly,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs = null
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(null, 0, 1, KickoffAt(12, 20), 0, new GroupResultsTeamSelector(__groups[0], 1), new GroupResultsTeamSelector(__groups[0], 2))
            ]
        },
        {
            // Single group with 4 teams, finals round and custom third place playoff
            [4],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.FinalOnly,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(3, "3.0", "4.0")
                    ]
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(null, null, 3, KickoffAt(13, 6), 0, new GroupResultsTeamSelector(__groups[0], 3), new GroupResultsTeamSelector(__groups[0], 4)),
                new MatchExpectation(null, 0, 1, KickoffAt(13, 23), 0, new GroupResultsTeamSelector(__groups[0], 1), new GroupResultsTeamSelector(__groups[0], 2))
            ]
        },
        {
            // Single group with 4 teams, semi-finals, third place playoff and final match
            [4],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs = null
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(null, 1, null, KickoffAt(13, 6), 0, new GroupResultsTeamSelector(__groups[0], 1), new GroupResultsTeamSelector(__groups[0], 4)),
                new MatchExpectation(null, 1, null, KickoffAt(13, 23), 0, new GroupResultsTeamSelector(__groups[0], 2), new GroupResultsTeamSelector(__groups[0], 3)),
                new MatchExpectation(null, 3, KickoffAt(13, 40), 0, 7, MatchTeamSelector.Mode.Loser, 8, MatchTeamSelector.Mode.Loser),
                new MatchExpectation(0, 1, KickoffAt(13, 57), 0, 7, MatchTeamSelector.Mode.Winner, 8, MatchTeamSelector.Mode.Winner)
            ]
        },
        {
            // Two groups with 5 teams each, quarter-finals, semi-finals, fifth place playoff, third place playoff and final match
            // Played on a single court (as opposed to the test case below)
            [5, 5],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.QuarterFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(5, "5.0", "5.1")
                    ]
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 4), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 6), 0, new GroupDefinitionTeamSelector(__groups[1], 4), new GroupDefinitionTeamSelector(__groups[1], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 17), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 4)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 39), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 50), 0, new GroupDefinitionTeamSelector(__groups[1], 3), new GroupDefinitionTeamSelector(__groups[1], 4)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(14, 1), 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 0)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(14, 12), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 4)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(14, 23), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(14, 34), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 4)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(14, 45), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(14, 56), 0, new GroupDefinitionTeamSelector(__groups[0], 4), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(15, 7), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(15, 18), 0, new GroupDefinitionTeamSelector(__groups[1], 4), new GroupDefinitionTeamSelector(__groups[1], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(15, 29), 0, new GroupDefinitionTeamSelector(__groups[1], 3), new GroupDefinitionTeamSelector(__groups[1], 1)),

                // 21-24
                new MatchExpectation(null, 2, null, KickoffAt(15, 40), 0, new GroupResultsTeamSelector(__groups[0], 1), new GroupResultsTeamSelector(__groups[1], 4)),
                new MatchExpectation(null, 2, null, KickoffAt(15, 57), 0, new GroupResultsTeamSelector(__groups[0], 2), new GroupResultsTeamSelector(__groups[1], 3)),
                new MatchExpectation(null, 2, null, KickoffAt(16, 14), 0, new GroupResultsTeamSelector(__groups[0], 3), new GroupResultsTeamSelector(__groups[1], 2)),
                new MatchExpectation(null, 2, null, KickoffAt(16, 31), 0, new GroupResultsTeamSelector(__groups[0], 4), new GroupResultsTeamSelector(__groups[1], 1)),

                // 25-26
                new MatchExpectation(1, null, KickoffAt(16, 48), 0, 21, MatchTeamSelector.Mode.Winner, 22, MatchTeamSelector.Mode.Winner),
                new MatchExpectation(1, null, KickoffAt(17, 5), 0, 23, MatchTeamSelector.Mode.Winner, 24, MatchTeamSelector.Mode.Winner),

                // 27-29
                new MatchExpectation(null, null, 5, KickoffAt(17, 22), 0, new GroupResultsTeamSelector(__groups[0], 5), new GroupResultsTeamSelector(__groups[1], 5)),
                new MatchExpectation(null, 3, KickoffAt(17, 39), 0, 25, MatchTeamSelector.Mode.Loser, 26, MatchTeamSelector.Mode.Loser),
                new MatchExpectation(0, 1, KickoffAt(17, 56), 0, 25, MatchTeamSelector.Mode.Winner, 26, MatchTeamSelector.Mode.Winner)
            ]
        },
        {
            // Two groups with 5 teams each, quarter-finals, semi-finals, fifth place playoff, third place playoff and final match
            // Played on a two courts in group phase and three courts in finals phase (as opposed to the test case below)
            [5, 5],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig with { GroupPhaseNumberOfCourts = 2, FinalsPhaseNumberOfCourts = 3 },
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.QuarterFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(5, "5.0", "5.1")
                    ]
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 1, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 11), 1, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 4), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 1, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[1], 4), new GroupDefinitionTeamSelector(__groups[1], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 1, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 4)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 1, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[1], 3), new GroupDefinitionTeamSelector(__groups[1], 4)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 55), 1, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 0)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 4)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 6), 1, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 3)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 17), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 4)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 17), 1, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 3)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionTeamSelector(__groups[0], 4), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[0], null, null, KickoffAt(13, 28), 1, new GroupDefinitionTeamSelector(__groups[0], 3), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 39), 0, new GroupDefinitionTeamSelector(__groups[1], 4), new GroupDefinitionTeamSelector(__groups[1], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(13, 39), 1, new GroupDefinitionTeamSelector(__groups[1], 3), new GroupDefinitionTeamSelector(__groups[1], 1)),

                // 21-24
                new MatchExpectation(null, 2, null, KickoffAt(13, 50), 0, new GroupResultsTeamSelector(__groups[0], 1), new GroupResultsTeamSelector(__groups[1], 4)),
                new MatchExpectation(null, 2, null, KickoffAt(13, 50), 1, new GroupResultsTeamSelector(__groups[0], 2), new GroupResultsTeamSelector(__groups[1], 3)),
                new MatchExpectation(null, 2, null, KickoffAt(13, 50), 2, new GroupResultsTeamSelector(__groups[0], 3), new GroupResultsTeamSelector(__groups[1], 2)),
                new MatchExpectation(null, 2, null, KickoffAt(14, 07), 0, new GroupResultsTeamSelector(__groups[0], 4), new GroupResultsTeamSelector(__groups[1], 1)),

                // 25-26
                new MatchExpectation(1, null, KickoffAt(14, 24), 0, 21, MatchTeamSelector.Mode.Winner, 22, MatchTeamSelector.Mode.Winner),
                new MatchExpectation(1, null, KickoffAt(14, 24), 1, 23, MatchTeamSelector.Mode.Winner, 24, MatchTeamSelector.Mode.Winner),

                // 27-29
                new MatchExpectation(null, null, 5, KickoffAt(14, 41), 0, new GroupResultsTeamSelector(__groups[0], 5), new GroupResultsTeamSelector(__groups[1], 5)),
                new MatchExpectation(null, 3, KickoffAt(14, 41), 1, 25, MatchTeamSelector.Mode.Loser, 26, MatchTeamSelector.Mode.Loser),
                new MatchExpectation(0, 1, KickoffAt(14, 58), 0, 25, MatchTeamSelector.Mode.Winner, 26, MatchTeamSelector.Mode.Winner)
            ]
        },
        {
            // Two groups with 3 teams each and finals round with custom team selectors
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    TeamSelectors =
                    [
                        "0B1",
                        "1B2",
                        "1B1",
                        "0B2"
                    ],
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs = null
                }
            },
            [
                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionTeamSelector(__groups[0], 0), new GroupDefinitionTeamSelector(__groups[0], 1)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 11), 0, new GroupDefinitionTeamSelector(__groups[1], 0), new GroupDefinitionTeamSelector(__groups[1], 1)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionTeamSelector(__groups[0], 1), new GroupDefinitionTeamSelector(__groups[0], 2)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionTeamSelector(__groups[1], 1), new GroupDefinitionTeamSelector(__groups[1], 2)),

                new MatchExpectation(__groups[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionTeamSelector(__groups[0], 2), new GroupDefinitionTeamSelector(__groups[0], 0)),
                new MatchExpectation(__groups[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionTeamSelector(__groups[1], 2), new GroupDefinitionTeamSelector(__groups[1], 0)),

                new MatchExpectation(null, 1, null, KickoffAt(13, 6), 0, new GroupResultsNthRankedTeamSelector([__groups[0], __groups[1]], 0, 1), new GroupResultsNthRankedTeamSelector([__groups[0], __groups[1]], 1, 2)),
                new MatchExpectation(null, 1, null, KickoffAt(13, 23), 0, new GroupResultsNthRankedTeamSelector([__groups[0], __groups[1]], 1, 1), new GroupResultsNthRankedTeamSelector([__groups[0], __groups[1]], 0, 2)),

                new MatchExpectation(0, 1, KickoffAt(13, 40), 0, 7, MatchTeamSelector.Mode.Winner, 8, MatchTeamSelector.Mode.Winner)
            ]
        }

        // The following test cases were removed when switching from standard team selectors to abstract team selectors in the FinalsRoundConfig:
        //    > One group with 8 teams with only a finals round (basically, a K/O tournament)
        //    > No groups and 8 non-assigned teams and only a finals round (basically, a K/O tournament but without groups)
        // IDEA: At some point, abstract team selectors should allow selecting a team by index and then a corresponding test case should be re-added
    };

    public static readonly TheoryData<string, int[], MatchPlanConfiguration> InvalidTestData = new()
    {
        {
            // Single group with 10 teams, no finals round
            "No group match definitions exist for a group with 10 teams.",
            [10],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = null
            }
        },
        {
            // Two groups with 3 teams each, finals round with redundant third place playoff
            "Can not auto-generate a third place playoff when it is already specified in additional playoffs.",
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(5, "3.0", "3.1"),
                        new AdditionalPlayoffConfig(3, "2.0", "2.1")
                    ]
                }
            }
        },
        {
            // Two groups with 3 teams each, finals round with semi-finals and wrong number of custom team selectors
            "3 team selectors are specified, but 4 team selectors are required.",
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    TeamSelectors =
                    [
                        "1.0",
                        "2.0",
                        "1.1"
                    ],
                    EnableThirdPlacePlayoff = true,
                    AdditionalPlayoffs = null
                }
            }
        },
        {
            // Two groups with 3 teams each, finals round with duplicate additional playoffs
            "Playoff configuration must only contain unique playoff positions.",
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.FinalOnly,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(5, "3.0", "3.1"),
                        new AdditionalPlayoffConfig(5, "2.0", "2.1")
                    ]
                }
            }
        },
        {
            // Two groups with 3 teams each, finals round with invalid additional playoff position (6, which is even)
            "Playoff position must be an odd integer >= 3, but specified value 6 is not.",
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.FinalOnly,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(6, "3.0", "3.1"),
                        new AdditionalPlayoffConfig(5, "2.0", "2.1")
                    ]
                }
            }
        },
        {
            // Two groups with 3 teams each, finals round with invalid additional playoff position (1, which is to small)
            "Playoff position must be an odd integer >= 3, but specified value 1 is not.",
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.FinalOnly,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = false,
                    AdditionalPlayoffs =
                    [
                        new AdditionalPlayoffConfig(1, "3.0", "3.1"),
                        new AdditionalPlayoffConfig(5, "2.0", "2.1")
                    ]
                }
            }
        },
        {
            // Five groups with 3 teams each with semi-finals (there exist no pre-defined team selectors for first round)
            "No pre-defined first finals round configuration exists for 5 groups and 2 matches.",
            [3, 3, 3, 3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = __scheduleConfig,
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Sequential,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    TeamSelectors = null,
                    EnableThirdPlacePlayoff = true
                }
            }
        },
        {
            // No group nor finals config specified
            "Either group round config or finals round config must be specified.",
            [3, 3],
            new MatchPlanConfiguration
            {
                ScheduleConfig = null,
                GroupRoundConfig = null,
                FinalsRoundConfig = null
            }
        },
        {
            "At least one group does not contain enough participants (present: 3, required: 4) for the pre-defined first finals round.",
            [3],
            new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.SemiFinals,
                    EnableThirdPlacePlayoff = false,
                    TeamSelectors = null,
                    AdditionalPlayoffs = null
                },
                ScheduleConfig = null
            }
        },
        {
            "At least one group does not contain enough participants (present: 2, required: 3) for the pre-defined first finals round.",
            [3, 3, 2],
            new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.QuarterFinals,
                    EnableThirdPlacePlayoff = false,
                    TeamSelectors = null,
                    AdditionalPlayoffs = null
                },
                ScheduleConfig = null
            }
        },
        {
            "At least one group does not contain enough participants (present: 3, required: 4) for the pre-defined first finals round.",
            [4, 3],
            new MatchPlanConfiguration
            {
                GroupRoundConfig = new GroupRoundConfig
                {
                    GroupMatchOrder = GroupMatchOrder.Alternating,
                    GroupPhaseRounds = 1
                },
                FinalsRoundConfig = new FinalsRoundConfig
                {
                    FirstFinalsRoundOrder = FinalsRoundOrder.QuarterFinals,
                    EnableThirdPlacePlayoff = false,
                    TeamSelectors = null,
                    AdditionalPlayoffs = null
                },
                ScheduleConfig = null
            }
        }
    };

    /// <remarks>The parameter <paramref name="teamsPerGroup"/> is allowed to only contain a single negative integer. In that case,
    /// the specified integer will be negated and that many teams will be created but without assigning them to a group.</remarks>
    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Tournament___GenerateMatchPlan_With_Valid_Config___Works_As_Expected(int[] teamsPerGroup, MatchPlanConfiguration configuration, MatchExpectation[] expectedMatchInfo)
    {
        // Arrange
        var tournament = CreateTestTournament(teamsPerGroup);

        // Act
        tournament.GenerateMatchPlan(configuration);

        // Assert
        var matches = tournament._matches.OrderBy(x => x.Index).ToArray();
        matches.Should().HaveCount(expectedMatchInfo.Length);
        for (var i = 0; i < matches.Length; i++)
        {
            var match = matches[i];
            var expected = expectedMatchInfo[i];

            match.Index.Should().Be(i + 1, $"match with id {i + 1} should have correct 'Index'");
            match.TeamA.Should().BeNull($"match with id {i + 1} should have correct 'TeamA'");
            match.TeamB.Should().BeNull($"match with id {i + 1} should have correct 'TeamB'");
            match.ScoreA.Should().Be(null, $"match with id {i + 1} should have correct 'ScoreA'");
            match.ScoreB.Should().Be(null, $"match with id {i + 1} should have correct 'ScoreB'");
            match.OutcomeType.Should().Be(null, $"match with id {i + 1} should have correct 'OutcomeType'");

            expected.Validate(match);
        }
    }

    [Fact]
    public void Tournament___GenerateMatchPlan_When_Matches_Is_Not_Empty___Works_As_Expected()
    {
        // Arrange
        var config = new MatchPlanConfiguration
        {
            GroupRoundConfig = new GroupRoundConfig
            {
                GroupMatchOrder = GroupMatchOrder.Alternating,
                GroupPhaseRounds = 1
            }
        };

        var tournament = new Tournament(string.Empty);
        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), null!));

        var action = () => tournament.GenerateMatchPlan(config);
        action.Should().ThrowExactly<TournamentException>().WithMessage("Matches list must be empty before generating match plan.");
        tournament._matches.Should().HaveCount(1);

        tournament.GenerateMatchPlan(config, clearMatches: true);
        tournament._matches.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void Tournament___GenerateMatchPlan_With_Invalid_Config___Throws_Exception(string exceptionMessage, int[] teamsPerGroup, MatchPlanConfiguration configuration)
    {
        // Arrange
        var tournament = CreateTestTournament(teamsPerGroup);

        // Act
        var func = () =>
        {
            tournament.GenerateMatchPlan(configuration);
        };

        // Assert
        func.Should().ThrowExactly<TournamentException>().WithMessage(exceptionMessage);
        tournament._matches.Should().BeEmpty();
    }

    /// <remarks>Helper method to simplify the creation of <see cref="DateTime"/> objects.</remarks>
    private static DateTime KickoffAt(int hours, int minutes) => new(2024, 1, 1, hours, minutes, 0);

    private static Tournament CreateTestTournament(int[] teamsPerGroup)
    {
        var tournament = new Tournament(string.Empty);

        if (teamsPerGroup.Length == 1 && teamsPerGroup[0] < 0)
        {
            // This mode is used to create a specified number of teams but without assigning them to a group
            var teamIndex = 0;
            var numberOfTeams = -teamsPerGroup[0];
            for (var i = 0; i < numberOfTeams; i++)
            {
                tournament._teams.Add(new Team($"Team {++teamIndex}", false));
            }
        }
        else
        {
            var teamIndex = 0;
            for (var i = 0; i < teamsPerGroup.Length; i++)
            {
                var teamCount = teamsPerGroup[i];

                var group = __groups[i];
                group._participants.Clear();
                group._tournament = tournament;
                tournament._groups.Add(group);

                for (var j = 0; j < teamCount; j++)
                {
                    var team = new Team($"Team {++teamIndex}", false);
                    tournament._teams.Add(team);

                    group._participants.Add(new GroupParticipant(group, team, 0, 0));
                }
            }
        }

        return tournament;
    }

    public readonly record struct MatchExpectation
    {
        private readonly Group? _group;
        private readonly int? _finalsRound;
        private readonly int? _playoffPosition;
        private readonly DateTime? _kickoff;
        private readonly short _court;
        private readonly TeamSelectorBase? _teamSelectorA;
        private readonly TeamSelectorBase? _teamSelectorB;
        private readonly int _matchIndexA;
        private readonly MatchTeamSelector.Mode _matchModeA;
        private readonly int _matchIndexB;
        private readonly MatchTeamSelector.Mode _matchModeB;

        public MatchExpectation(Group? group, int? finalsRound, int? playoffPosition, DateTime? kickoff, short court, TeamSelectorBase teamSelectorA, TeamSelectorBase teamSelectorB)
        {
            _group = group;
            _finalsRound = finalsRound;
            _playoffPosition = playoffPosition;
            _kickoff = kickoff;
            _court = court;
            _teamSelectorA = teamSelectorA;
            _teamSelectorB = teamSelectorB;
        }

        public MatchExpectation(int? finalsRound, int? playoffPosition, DateTime? kickoff, short court, int matchIndexA, MatchTeamSelector.Mode matchModeA, int matchIndexB, MatchTeamSelector.Mode matchModeB)
        {
            _finalsRound = finalsRound;
            _playoffPosition = playoffPosition;
            _kickoff = kickoff;
            _court = court;
            _matchIndexA = matchIndexA;
            _matchModeA = matchModeA;
            _matchIndexB = matchIndexB;
            _matchModeB = matchModeB;
        }

        public void Validate(Match match)
        {
            match.Court.Should().Be(_court, $"match with index {match.Index} should have correct 'Court'");
            match.Kickoff.Should().Be(_kickoff, $"match with index {match.Index} should have correct 'Kickoff'");
            match.Group?.Should().Be(_group, $"match with index {match.Index} should have correct 'GroupId'");
            match.FinalsRound.Should().Be(_finalsRound, $"match with index {match.Index} should have correct 'FinalsRound'");
            match.PlayoffPosition.Should().Be(_playoffPosition, $"match with index {match.Index} should have correct 'PlayoffPosition'");

            if (_teamSelectorA is not null)
            {
                match.TeamSelectorA.Should().Be(_teamSelectorA, $"match with index {match.Index} should have correct 'TeamSelectorA'");
                match.TeamSelectorB.Should().Be(_teamSelectorB, $"match with index {match.Index} should have correct 'TeamSelectorB'");
            }
            else
            {
                var teamSelectorA = match.TeamSelectorA.Should().BeOfType<MatchTeamSelector>().Subject;
                var teamSelectorB = match.TeamSelectorB.Should().BeOfType<MatchTeamSelector>().Subject;

                teamSelectorA.TargetMatch.Index.Should().Be(_matchIndexA);
                teamSelectorA.SelectionMode.Should().Be(_matchModeA);
                teamSelectorB.TargetMatch.Index.Should().Be(_matchIndexB);
                teamSelectorB.SelectionMode.Should().Be(_matchModeB);
            }
        }
    }
}
