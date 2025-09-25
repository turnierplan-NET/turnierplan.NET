using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class TournamentGenerateMatchPlanTest
{
    private static readonly int[] __teamIds = Enumerable.Range(0, 1000).Select(x => x + 10000).ToArray();
    private static readonly int[] __groupIds = Enumerable.Range(0, 1000).Select(x => x + 12000).ToArray();

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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 17), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 6), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 17), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 39), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 50), 0, new GroupDefinitionSelector(__groupIds[1], 3), new GroupDefinitionSelector(__groupIds[1], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(14, 1), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 0), 1, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 11), 1, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 22), 1, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 1, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 44), 1, new GroupDefinitionSelector(__groupIds[1], 3), new GroupDefinitionSelector(__groupIds[1], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 55), 1, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 1, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 2, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 1, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 2, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2))
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
                new MatchExpectation(__groupIds[0], null, null, null, 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[1], null, null, null, 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),
                new MatchExpectation(__groupIds[1], null, null, null, 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2)),
                new MatchExpectation(__groupIds[1], null, null, null, 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 0))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 17), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 28), 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 0))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 0)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 17), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(null, 0, 1, KickoffAt(12, 11), 0, new GroupResultsSelector(__groupIds[0], 1), new GroupResultsSelector(__groupIds[0], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(null, 0, 1, KickoffAt(12, 20), 0, new GroupResultsSelector(__groupIds[0], 1), new GroupResultsSelector(__groupIds[0], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(null, null, 3, KickoffAt(13, 6), 0, new GroupResultsSelector(__groupIds[0], 3), new GroupResultsSelector(__groupIds[0], 4)),
                new MatchExpectation(null, 0, 1, KickoffAt(13, 23), 0, new GroupResultsSelector(__groupIds[0], 1), new GroupResultsSelector(__groupIds[0], 2))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(null, 1, null, KickoffAt(13, 6), 0, new GroupResultsSelector(__groupIds[0], 1), new GroupResultsSelector(__groupIds[0], 4)),
                new MatchExpectation(null, 1, null, KickoffAt(13, 23), 0, new GroupResultsSelector(__groupIds[0], 2), new GroupResultsSelector(__groupIds[0], 3)),
                new MatchExpectation(null, null, 3, KickoffAt(13, 40), 0, new MatchSelector(7, MatchSelector.Mode.Loser), new MatchSelector(8, MatchSelector.Mode.Loser)),
                new MatchExpectation(null, 0, 1, KickoffAt(13, 57), 0, new MatchSelector(7, MatchSelector.Mode.Winner), new MatchSelector(8, MatchSelector.Mode.Winner))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 4), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 6), 0, new GroupDefinitionSelector(__groupIds[1], 4), new GroupDefinitionSelector(__groupIds[1], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 17), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 4)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 39), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 50), 0, new GroupDefinitionSelector(__groupIds[1], 3), new GroupDefinitionSelector(__groupIds[1], 4)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(14, 1), 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 0)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(14, 12), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 4)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(14, 23), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(14, 34), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 4)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(14, 45), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(14, 56), 0, new GroupDefinitionSelector(__groupIds[0], 4), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(15, 7), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(15, 18), 0, new GroupDefinitionSelector(__groupIds[1], 4), new GroupDefinitionSelector(__groupIds[1], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(15, 29), 0, new GroupDefinitionSelector(__groupIds[1], 3), new GroupDefinitionSelector(__groupIds[1], 1)),

                // 21-24
                new MatchExpectation(null, 2, null, KickoffAt(15, 40), 0, new GroupResultsSelector(__groupIds[0], 1), new GroupResultsSelector(__groupIds[1], 4)),
                new MatchExpectation(null, 2, null, KickoffAt(15, 57), 0, new GroupResultsSelector(__groupIds[0], 2), new GroupResultsSelector(__groupIds[1], 3)),
                new MatchExpectation(null, 2, null, KickoffAt(16, 14), 0, new GroupResultsSelector(__groupIds[0], 3), new GroupResultsSelector(__groupIds[1], 2)),
                new MatchExpectation(null, 2, null, KickoffAt(16, 31), 0, new GroupResultsSelector(__groupIds[0], 4), new GroupResultsSelector(__groupIds[1], 1)),

                // 25-26
                new MatchExpectation(null, 1, null, KickoffAt(16, 48), 0, new MatchSelector(21, MatchSelector.Mode.Winner), new MatchSelector(22, MatchSelector.Mode.Winner)),
                new MatchExpectation(null, 1, null, KickoffAt(17, 5), 0, new MatchSelector(23, MatchSelector.Mode.Winner), new MatchSelector(24, MatchSelector.Mode.Winner)),

                // 27-29
                new MatchExpectation(null, null, 5, KickoffAt(17, 22), 0, new GroupResultsSelector(__groupIds[0], 5), new GroupResultsSelector(__groupIds[1], 5)),
                new MatchExpectation(null, null, 3, KickoffAt(17, 39), 0, new MatchSelector(25, MatchSelector.Mode.Loser), new MatchSelector(26, MatchSelector.Mode.Loser)),
                new MatchExpectation(null, 0, 1, KickoffAt(17, 56), 0, new MatchSelector(25, MatchSelector.Mode.Winner), new MatchSelector(26, MatchSelector.Mode.Winner))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 1, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 11), 1, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 4), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 1, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[1], 4), new GroupDefinitionSelector(__groupIds[1], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 1, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 4)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 1, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[1], 3), new GroupDefinitionSelector(__groupIds[1], 4)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 55), 1, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 0)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 6), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 4)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 6), 1, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 3)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 17), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 4)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 17), 1, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 3)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 28), 0, new GroupDefinitionSelector(__groupIds[0], 4), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(13, 28), 1, new GroupDefinitionSelector(__groupIds[0], 3), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 39), 0, new GroupDefinitionSelector(__groupIds[1], 4), new GroupDefinitionSelector(__groupIds[1], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(13, 39), 1, new GroupDefinitionSelector(__groupIds[1], 3), new GroupDefinitionSelector(__groupIds[1], 1)),

                // 21-24
                new MatchExpectation(null, 2, null, KickoffAt(13, 50), 0, new GroupResultsSelector(__groupIds[0], 1), new GroupResultsSelector(__groupIds[1], 4)),
                new MatchExpectation(null, 2, null, KickoffAt(13, 50), 1, new GroupResultsSelector(__groupIds[0], 2), new GroupResultsSelector(__groupIds[1], 3)),
                new MatchExpectation(null, 2, null, KickoffAt(13, 50), 2, new GroupResultsSelector(__groupIds[0], 3), new GroupResultsSelector(__groupIds[1], 2)),
                new MatchExpectation(null, 2, null, KickoffAt(14, 07), 0, new GroupResultsSelector(__groupIds[0], 4), new GroupResultsSelector(__groupIds[1], 1)),

                // 25-26
                new MatchExpectation(null, 1, null, KickoffAt(14, 24), 0, new MatchSelector(21, MatchSelector.Mode.Winner), new MatchSelector(22, MatchSelector.Mode.Winner)),
                new MatchExpectation(null, 1, null, KickoffAt(14, 24), 1, new MatchSelector(23, MatchSelector.Mode.Winner), new MatchSelector(24, MatchSelector.Mode.Winner)),

                // 27-29
                new MatchExpectation(null, null, 5, KickoffAt(14, 41), 0, new GroupResultsSelector(__groupIds[0], 5), new GroupResultsSelector(__groupIds[1], 5)),
                new MatchExpectation(null, null, 3, KickoffAt(14, 41), 1, new MatchSelector(25, MatchSelector.Mode.Loser), new MatchSelector(26, MatchSelector.Mode.Loser)),
                new MatchExpectation(null, 0, 1, KickoffAt(14, 58), 0, new MatchSelector(25, MatchSelector.Mode.Winner), new MatchSelector(26, MatchSelector.Mode.Winner))
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
                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 0), 0, new GroupDefinitionSelector(__groupIds[0], 0), new GroupDefinitionSelector(__groupIds[0], 1)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 11), 0, new GroupDefinitionSelector(__groupIds[1], 0), new GroupDefinitionSelector(__groupIds[1], 1)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 22), 0, new GroupDefinitionSelector(__groupIds[0], 1), new GroupDefinitionSelector(__groupIds[0], 2)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 33), 0, new GroupDefinitionSelector(__groupIds[1], 1), new GroupDefinitionSelector(__groupIds[1], 2)),

                new MatchExpectation(__groupIds[0], null, null, KickoffAt(12, 44), 0, new GroupDefinitionSelector(__groupIds[0], 2), new GroupDefinitionSelector(__groupIds[0], 0)),
                new MatchExpectation(__groupIds[1], null, null, KickoffAt(12, 55), 0, new GroupDefinitionSelector(__groupIds[1], 2), new GroupDefinitionSelector(__groupIds[1], 0)),

                new MatchExpectation(null, 1, null, KickoffAt(13, 6), 0, new GroupResultsNthRankedSelector([__groupIds[0], __groupIds[1]], 0, 1), new GroupResultsNthRankedSelector([__groupIds[0], __groupIds[1]], 1, 2)),
                new MatchExpectation(null, 1, null, KickoffAt(13, 23), 0, new GroupResultsNthRankedSelector([__groupIds[0], __groupIds[1]], 1, 1), new GroupResultsNthRankedSelector([__groupIds[0], __groupIds[1]], 0, 2)),

                new MatchExpectation(null, 0, 1, KickoffAt(13, 40), 0, new MatchSelector(7, MatchSelector.Mode.Winner), new MatchSelector(8, MatchSelector.Mode.Winner))
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
        }
    };

    /// <remarks>The parameter <paramref name="teamsPerGroup"/> is allowed to only contain a single negative integer. In that case,
    /// the specified integer will be negated and that many teams will be created but without assigning them to a group.</remarks>
    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Tournament___GenerateMatchPlan_With_Invalid_Config___Works_As_Expected(int[] teamsPerGroup, MatchPlanConfiguration configuration, MatchExpectation[] expectedMatchInfo)
    {
        // Arrange
        var tournament = CreateTestTournament(teamsPerGroup);

        // Act
        tournament.GenerateMatchPlan(configuration);

        // Assert
        var matches = tournament._matches.OrderBy(x => x.Id).ToList();
        matches.Should().HaveCount(expectedMatchInfo.Length);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var expected = expectedMatchInfo[i];

            match.Index.Should().Be(i + 1, $"match with id {i + 1} should have correct 'Index'");
            match.Court.Should().Be(expected.Court, $"match with id {i + 1} should have correct 'Court'");
            match.Kickoff.Should().Be(expected.Kickoff, $"match with id {i + 1} should have correct 'Kickoff'");
            match.TeamSelectorA.Should().Be(expected.A, $"match with id {i + 1} should have correct 'TeamSelectorsA'");
            match.TeamSelectorB.Should().Be(expected.B, $"match with id {i + 1} should have correct 'TeamSelectorsB'");
            match.TeamA.Should().BeNull($"match with id {i + 1} should have correct 'TeamA'");
            match.TeamB.Should().BeNull($"match with id {i + 1} should have correct 'TeamB'");
            match.Group?.Id.Should().Be(expected.GroupId, $"match with id {i + 1} should have correct 'GroupId'");
            match.FinalsRound.Should().Be(expected.FinalsRound, $"match with id {i + 1} should have correct 'FinalsRound'");
            match.PlayoffPosition.Should().Be(expected.PlayoffPosition, $"match with id {i + 1} should have correct 'PlayoffPosition'");
            match.ScoreA.Should().Be(null, $"match with id {i + 1} should have correct 'ScoreA'");
            match.ScoreB.Should().Be(null, $"match with id {i + 1} should have correct 'ScoreB'");
            match.OutcomeType.Should().Be(null, $"match with id {i + 1} should have correct 'OutcomeType'");
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

        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(0, 1, new NullSelector(), new NullSelector(), null!));

        var action = () => tournament.GenerateMatchPlan(config);
        action.Should().ThrowExactly<TurnierplanException>().WithMessage("Matches list must be empty before generating match plan.");
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
        func.Should().ThrowExactly<TurnierplanException>().WithMessage(exceptionMessage);
        tournament._matches.Should().BeEmpty();
    }

    /// <remarks>Helper method to simplify the creation of <see cref="DateTime"/> objects.</remarks>
    private static DateTime KickoffAt(int hours, int minutes) => new(2024, 1, 1, hours, minutes, 0);

    private static Core.Tournament.Tournament CreateTestTournament(int[] teamsPerGroup)
    {
        var tournament = TestTournament.Default;

        if (teamsPerGroup.Length == 1 && teamsPerGroup[0] < 0)
        {
            // This mode is used to create a specified number of teams but without assigning them to a group
            var teamIndex = 0;
            var numberOfTeams = -teamsPerGroup[0];
            for (var i = 0; i < numberOfTeams; i++)
            {
                tournament._teams.Add(new Team(__teamIds[teamIndex], null!, $"Team {++teamIndex}"));
            }
        }
        else
        {
            var teamIndex = 0;
            for (var i = 0; i < teamsPerGroup.Length; i++)
            {
                var teamCount = teamsPerGroup[i];

                var group = new Group(__groupIds[i], (char)('A' + i));
                tournament._groups.Add(group);

                for (var j = 0; j < teamCount; j++)
                {
                    var team = new Team(__teamIds[teamIndex], null!, $"Team {++teamIndex}");
                    tournament._teams.Add(team);

                    group._participants.Add(new GroupParticipant(group, team, 0, 0));
                }
            }
        }

        return tournament;
    }

    public record struct MatchExpectation(int? GroupId, int? FinalsRound, int? PlayoffPosition, DateTime? Kickoff, short Court, TeamSelectorBase A, TeamSelectorBase B);
}
