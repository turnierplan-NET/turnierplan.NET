using System.Collections.Immutable;
using System.Text.Json;

namespace Turnierplan.Core.Tournament.Definitions;

public static class MatchPlanDefinitions
{
    private const string GroupMatchDefinitionsResource = "Definitions.GroupMatchDefinitions.json";
    private const string FinalsMatchDefinitionsResource = "Definitions.FinalsMatchDefinitions.json";

    private static readonly Dictionary<int, GroupMatchDefinition> __groupMatchDefinitions = new();
    private static readonly Dictionary<(int GroupCount, int MatchCount), FinalsMatchDefinition> __finalsMatchDefinitions = new();

    static MatchPlanDefinitions()
    {
        LoadGroupMatchDefinitions();
        LoadFinalsMatchDefinitions();
    }

    public static GroupMatchDefinition? GetGroupMatchDefinition(int teamCount)
    {
        return __groupMatchDefinitions.GetValueOrDefault(teamCount);
    }

    public static FinalsMatchDefinition? GetFinalsMatchDefinition(int groupCount, int matchCount)
    {
        return __finalsMatchDefinitions.GetValueOrDefault((groupCount, matchCount));
    }

    public static IEnumerable<(int TeamCount, GroupMatchDefinition Definition)> GetAllGroupMatchDefinitions()
    {
        return __groupMatchDefinitions.Select(x => (TeamCount: x.Key, Definition: x.Value));
    }

    public static IEnumerable<(int GroupCount, int MatchCount, FinalsMatchDefinition Definition)> GetAllFinalsMatchDefinitions()
    {
        return __finalsMatchDefinitions.Select(x => (x.Key.GroupCount, x.Key.MatchCount, Definition: x.Value));
    }

    private static void LoadGroupMatchDefinitions()
    {
        var stream = typeof(MatchPlanDefinitions).Assembly.GetManifestResourceStream(GroupMatchDefinitionsResource);
        var result = JsonSerializer.Deserialize<List<GroupMatchDefinitionJsonModel>>(stream!);

        if (result is null)
        {
            throw new IOException($"Failed to parse '{GroupMatchDefinitionsResource}'.");
        }

        foreach (var model in result)
        {
            var matchBlocks = model.MatchBlocks.Select(x =>
            {
                var matches = x.Select(y =>
                {
                    // Subtract 1 from the indices because in definitions json the indices are provided on a 1.. range.
                    var teamIdA = y[0] - 1;
                    var teamIdB = y[1] - 1;
                    return new GroupMatchDefinition.MatchDefinition(teamIdA, teamIdB);
                });

                return new GroupMatchDefinition.MatchBlock(matches.ToImmutableArray());
            });
            __groupMatchDefinitions[model.TeamCount] = new GroupMatchDefinition(matchBlocks);
        }
    }

    private static void LoadFinalsMatchDefinitions()
    {
        var stream = typeof(MatchPlanDefinitions).Assembly.GetManifestResourceStream(FinalsMatchDefinitionsResource);
        var result = JsonSerializer.Deserialize<List<FinalsMatchDefinitionJsonModel>>(stream!);

        if (result is null)
        {
            throw new IOException($"Failed to parse '{FinalsMatchDefinitionsResource}'.");
        }

        foreach (var model in result)
        {
            var matches = model.MatchDefinitions
                .Select(x =>
                {
                    var teamA = AbstractTeamSelectorParser.ParseAbstractTeamSelectorFromDefinitionFormat(x[0]);
                    var teamB = AbstractTeamSelectorParser.ParseAbstractTeamSelectorFromDefinitionFormat(x[1]);

                    return new FinalsMatchDefinition.MatchDefinition(teamA, teamB);
                })
                .ToList();
            __finalsMatchDefinitions[(model.GroupCount, MatchCount: matches.Count)] = new FinalsMatchDefinition(matches);
        }
    }

    private sealed record GroupMatchDefinitionJsonModel(int TeamCount, int[][][] MatchBlocks);

    private sealed record FinalsMatchDefinitionJsonModel(int GroupCount, string[][] MatchDefinitions);
}
