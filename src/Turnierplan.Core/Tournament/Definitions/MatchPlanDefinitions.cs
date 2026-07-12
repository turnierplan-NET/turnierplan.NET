using System.Collections.Immutable;

namespace Turnierplan.Core.Tournament.Definitions;

public static class MatchPlanDefinitions
{
    private static readonly IReadOnlyDictionary<int, GroupMatchDefinition> __groupMatchDefinitions;
    private static readonly IReadOnlyDictionary<(int GroupCount, int MatchCount), FinalsMatchDefinition> __finalsMatchDefinitions;

    static MatchPlanDefinitions()
    {
        var builder = new DefinitionsBuilder();

        builder.Group(2, g =>
        {
            g.Block(b => b.Match(1, 2));
        });

        builder.Group(3, g =>
        {
            g.Block(b => b.Match(1, 2));
            g.Block(b => b.Match(2, 3));
            g.Block(b => b.Match(3, 1));
        });

        builder.Group(4, g =>
        {
            g.Block(b => { b.Match(1, 2); b.Match(3, 4); });
            g.Block(b => { b.Match(1, 3); b.Match(2, 4); });
            g.Block(b => { b.Match(4, 1); b.Match(2, 3); });
        });

        builder.Group(5, g =>
        {
            g.Block(b => { b.Match(1, 2); b.Match(3, 4); });
            g.Block(b => { b.Match(5, 1); b.Match(2, 3); });
            g.Block(b => { b.Match(4, 5); b.Match(3, 1); });
            g.Block(b => { b.Match(2, 5); b.Match(1, 4); });
            g.Block(b => { b.Match(5, 3); b.Match(4, 2); });
        });

        builder.Group(6, g =>
        {
            g.Block(b => { b.Match(1, 2); b.Match(3, 4); b.Match(5, 6); });
            g.Block(b => { b.Match(3, 1); b.Match(6, 2); b.Match(4, 5); });
            g.Block(b => { b.Match(1, 6); b.Match(5, 3); b.Match(4, 2); });
            g.Block(b => { b.Match(5, 1); b.Match(6, 4); b.Match(2, 3); });
            g.Block(b => { b.Match(1, 4); b.Match(2, 5); b.Match(3, 6); });
        });

        builder.Group(7, g =>
        {
            g.Block(b => { b.Match(1, 2); b.Match(3, 4); b.Match(5, 6); });
            g.Block(b => { b.Match(4, 7); b.Match(1, 6); b.Match(5, 3); });
            g.Block(b => { b.Match(6, 2); b.Match(7, 5); b.Match(3, 1); });
            g.Block(b => { b.Match(4, 5); b.Match(2, 3); b.Match(7, 1); });
            g.Block(b => { b.Match(3, 6); b.Match(1, 4); b.Match(2, 7); });
            g.Block(b => { b.Match(5, 1); b.Match(6, 7); b.Match(4, 2); });
            g.Block(b => { b.Match(7, 3); b.Match(2, 5); b.Match(6, 4); });
        });

        builder.Group(8, g =>
        {
            g.Block(b => { b.Match(1, 2); b.Match(3, 4); b.Match(5, 6); b.Match(7, 8); });
            g.Block(b => { b.Match(1, 4); b.Match(6, 2); b.Match(3, 8); b.Match(7, 5); });
            g.Block(b => { b.Match(1, 6); b.Match(8, 4); b.Match(2, 7); b.Match(5, 3); });
            g.Block(b => { b.Match(1, 8); b.Match(6, 7); b.Match(4, 5); b.Match(2, 3); });
            g.Block(b => { b.Match(7, 1); b.Match(5, 8); b.Match(3, 6); b.Match(4, 2); });
            g.Block(b => { b.Match(5, 1); b.Match(7, 3); b.Match(8, 2); b.Match(6, 4); });
            g.Block(b => { b.Match(3, 1); b.Match(2, 5); b.Match(4, 7); b.Match(8, 6); });
        });

        builder.Group(9, g =>
        {
            g.Block(b => { b.Match(1, 2); b.Match(3, 4); b.Match(5, 6); b.Match(7, 8); });
            g.Block(b => { b.Match(9, 1); b.Match(2, 3); b.Match(4, 5); b.Match(6, 7); });
            g.Block(b => { b.Match(8, 9); b.Match(3, 1); b.Match(6, 4); b.Match(2, 5); });
            g.Block(b => { b.Match(9, 7); b.Match(1, 4); b.Match(5, 8); b.Match(2, 7); });
            g.Block(b => { b.Match(3, 6); b.Match(4, 9); b.Match(1, 8); b.Match(6, 2); });
            g.Block(b => { b.Match(5, 3); b.Match(8, 4); b.Match(7, 1); b.Match(9, 3); });
            g.Block(b => { b.Match(4, 2); b.Match(7, 5); b.Match(8, 6); b.Match(2, 9); });
            g.Block(b => { b.Match(5, 1); b.Match(7, 3); b.Match(6, 9); b.Match(8, 2); });
            g.Block(b => { b.Match(1, 6); b.Match(4, 7); b.Match(9, 5); b.Match(3, 8); });
        });

        // Add finals definitions...

        (__groupMatchDefinitions, __finalsMatchDefinitions) = builder.Build();
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

    private sealed class DefinitionsBuilder
    {
        private readonly Dictionary<int, GroupMatchDefinition> _groupMatchDefinitions = [];
        private readonly Dictionary<(int GroupCount, int MatchCount), FinalsMatchDefinition> _finalsMatchDefinitions = [];

        public void Group(int teamCount, Action<GroupMatchDefinitionBuilder> configure)
        {
            if (_groupMatchDefinitions.ContainsKey(teamCount))
            {
                throw new InvalidOperationException($"A group match definition for {teamCount} teams already exists.");
            }

            var builder = new GroupMatchDefinitionBuilder(teamCount);
            configure(builder);
            _groupMatchDefinitions[teamCount] = new GroupMatchDefinition(builder.Blocks);
        }

        // Add builder methods here...

        public (IReadOnlyDictionary<int, GroupMatchDefinition> __groupMatchDefinitions, IReadOnlyDictionary<(int GroupCount, int MatchCount), FinalsMatchDefinition> __finalsMatchDefinitions) Build()
        {
            return (_groupMatchDefinitions.AsReadOnly(), _finalsMatchDefinitions.AsReadOnly());
        }
    }

    private sealed class GroupMatchDefinitionBuilder
    {
        private readonly int _groupTeamCount;
        private readonly List<GroupMatchDefinition.MatchBlock> _blocks = [];

        public GroupMatchDefinitionBuilder(int groupTeamCount)
        {
            _groupTeamCount = groupTeamCount;
        }

        public ImmutableArray<GroupMatchDefinition.MatchBlock> Blocks => [.._blocks];

        public void Block(Action<GroupMatchDefinitionMatchBlockBuilder> configure)
        {
            var blockBuilder = new GroupMatchDefinitionMatchBlockBuilder(_groupTeamCount);
            configure(blockBuilder);
            _blocks.Add(new GroupMatchDefinition.MatchBlock(blockBuilder.Matches));
        }
    }

    private sealed class GroupMatchDefinitionMatchBlockBuilder
    {
        private readonly int _groupTeamCount;
        private readonly List<GroupMatchDefinition.MatchDefinition> _matches = [];

        public GroupMatchDefinitionMatchBlockBuilder(int groupTeamCount)
        {
            _groupTeamCount = groupTeamCount;
        }

        public ImmutableArray<GroupMatchDefinition.MatchDefinition> Matches => [.._matches];

        public void Match(int teamA, int teamB)
        {
            if (teamA < 1 || teamB < 1 || teamA > _groupTeamCount || teamB > _groupTeamCount || teamA == teamB)
            {
                throw new ArgumentException($"Teams A and B must both be between 1 and {_groupTeamCount} and they may not equal each other.");
            }

            // Subtract 1 so the caller can specify 1..n which is more intuitive than 0..(n-1)
            _matches.Add(new GroupMatchDefinition.MatchDefinition(teamA - 1, teamB - 1));
        }
    }
}
