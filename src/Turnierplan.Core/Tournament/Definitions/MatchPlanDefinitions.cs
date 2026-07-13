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

        builder.Finals(1, f =>
        {
            f.Match().Group(1, 'A').Against().Group(2, 'A');
        });

        builder.Finals(2, f =>
        {
            f.Match().Group(1, 'A').Against().Group(1, 'B');
        });

        builder.Finals(1, f =>
        {
            f.Match().Group(1, 'A').Against().Group(4, 'A');
            f.Match().Group(2, 'A').Against().Group(3, 'A');
        });

        builder.Finals(2, f =>
        {
            f.Match().Group(1, 'A').Against().Group(2, 'B');
            f.Match().Group(1, 'B').Against().Group(2, 'A');
        });

        builder.Finals(3, f =>
        {
            f.Match().Group(1, 'A').Against().Group(1, 'B');
            f.Match().Group(1, 'C').Against().NthRanked(1, 2);
        });

        builder.Finals(4, f =>
        {
            f.Match().Group(1, 'A').Against().Group(1, 'B');
            f.Match().Group(1, 'C').Against().Group(1, 'D');
        });

        builder.Finals(2, f =>
        {
            f.Match().Group(1, 'A').Against().Group(4, 'B');
            f.Match().Group(2, 'A').Against().Group(3, 'B');
            f.Match().Group(3, 'A').Against().Group(2, 'B');
            f.Match().Group(4, 'A').Against().Group(1, 'B');
        });

        builder.Finals(3, f =>
        {
            f.Match().Group(1, 'A').Against().NthRanked(1, 3);
            f.Match().Group(1, 'B').Against().NthRanked(2, 3);
            f.Match().Group(2, 'A').Against().Group(2, 'C');
            f.Match().Group(2, 'B').Against().Group(1, 'C');
        });

        builder.Finals(4, f =>
        {
            f.Match().Group(1, 'A').Against().Group(2, 'C');
            f.Match().Group(1, 'B').Against().Group(2, 'D');
            f.Match().Group(1, 'C').Against().Group(2, 'A');
            f.Match().Group(1, 'D').Against().Group(2, 'B');
        });

        builder.Finals(5, f =>
        {
            f.Match().Group(1, 'A').Against().Group(1, 'B');
            f.Match().Group(1, 'C').Against().NthRanked(1, 2);
            f.Match().Group(1, 'D').Against().NthRanked(2, 2);
            f.Match().Group(1, 'E').Against().NthRanked(3, 2);
        });

        builder.Finals(6, f =>
        {
            f.Match().Group(1, 'A').Against().Group(1, 'B');
            f.Match().Group(1, 'C').Against().Group(1, 'D');
            f.Match().Group(1, 'E').Against().NthRanked(1, 2);
            f.Match().Group(1, 'F').Against().NthRanked(2, 2);
        });

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

        public void Finals(int groupCount, Action<FinalsMatchDefinitionBuilder> configure)
        {
            var builder = new FinalsMatchDefinitionBuilder(groupCount);
            configure(builder);

            var matches = builder.BuildMatches();
            var key = (GroupCount: groupCount, MatchCount: matches.Length);

            if (_finalsMatchDefinitions.ContainsKey(key))
            {
                throw new InvalidOperationException($"A finals match definition for {groupCount} groups and {matches.Length} matches already exists.");
            }

            _finalsMatchDefinitions[key] = new FinalsMatchDefinition(matches);
        }

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

    private sealed record FinalsMatchDefinitionBuilder
    {
        private readonly int _groupCount;
        private readonly List<FinalsMatchDefinitionMatchBuilder> _matchBuilders = [];

        public FinalsMatchDefinitionBuilder(int groupCount)
        {
            _groupCount = groupCount;
        }

        public FinalsMatchDefinitionMatchBuilder Match()
        {
            var builder = new FinalsMatchDefinitionMatchBuilder(_groupCount);
            _matchBuilders.Add(builder);
            return builder;
        }

        public ImmutableArray<FinalsMatchDefinition.MatchDefinition> BuildMatches()
        {
            return [.._matchBuilders.Select(x => x.Build())];
        }
    }

    private sealed record FinalsMatchDefinitionMatchBuilder
    {
        private readonly int _groupCount;
        private AbstractTeamSelector? _teamA, _teamB;
        private bool _againstCalled;

        public FinalsMatchDefinitionMatchBuilder(int groupCount)
        {
            _groupCount = groupCount;
        }

        public FinalsMatchDefinitionMatchBuilder Group(int position, char group)
        {
            var groupIndex = group - 'A';

            if (groupIndex < 0 || groupIndex >= _groupCount)
            {
                throw new ArgumentException($"The group must be between 'A' and '{(char)('A' + _groupCount - 1)}'.");
            }

            SetOpponent(new AbstractTeamSelector(false, groupIndex, position, null));

            return this;
        }

        public FinalsMatchDefinitionMatchBuilder NthRanked(int ordinal, int position)
        {
            if (ordinal < 1 || ordinal > _groupCount)
            {
                throw new ArgumentException($"The ordinal must be between 1 and the group count {_groupCount}.");
            }

            // Subtract 1 from ordinal so the caller can specify 1..n which is more intuitive than 0..(n-1)
            SetOpponent(new AbstractTeamSelector(true, null, position, ordinal - 1));

            return this;
        }

        public FinalsMatchDefinitionMatchBuilder Against()
        {
            if (_againstCalled)
            {
                throw new InvalidOperationException($"The '{nameof(Against)}()' method may only be called once.");
            }

            _againstCalled = true;

            return this;
        }

        public FinalsMatchDefinition.MatchDefinition Build()
        {
            if (_teamA is null || _teamB is null)
            {
                throw new InvalidOperationException("One or both opponents are not defined.");
            }

            return new FinalsMatchDefinition.MatchDefinition(_teamA, _teamB);
        }

        private void SetOpponent(AbstractTeamSelector team)
        {
            if (_againstCalled)
            {
                if (_teamB is not null)
                {
                    throw new InvalidOperationException("Any of the two opponents may only be set once.");
                }

                _teamB = team;
            }
            else
            {
                if (_teamA is not null)
                {
                    throw new InvalidOperationException("Any of the two opponents may only be set once.");
                }

                _teamA = team;
            }
        }
    }
}
