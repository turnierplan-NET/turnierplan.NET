using Turnierplan.Core.Configuration;
using Turnierplan.Core.Definitions;
using Turnierplan.Core.TeamSelectors;

namespace Turnierplan.Core;

public sealed class Tournament
{
    internal readonly TeamComparer _teamComparer;

    internal readonly List<Team> _teams = [];
    internal readonly List<Group> _groups = [];
    internal readonly List<Match> _matches = [];
    internal readonly List<RankingOverwrite> _rankingOverwrites = [];
    internal readonly List<RankingPosition> _ranking = [];

    public Tournament(string name)
    {
        _teamComparer = new TeamComparer(this);

        Name = name;
        ComputationConfiguration = new ComputationConfiguration();
    }

    public string Name { get; set; }

    public MatchPlanConfiguration? MatchPlanConfiguration { get; internal set; }

    public ComputationConfiguration ComputationConfiguration { get; set; }

    public IReadOnlyList<Team> Teams => _teams.AsReadOnly();

    public IReadOnlyList<Group> Groups => _groups.AsReadOnly();

    public IReadOnlyList<Match> Matches => _matches.AsReadOnly();

    public IReadOnlyList<RankingOverwrite> RankingOverwrites => _rankingOverwrites.AsReadOnly();

    public IReadOnlyList<RankingPosition> Ranking => _ranking.AsReadOnly();

    public DateTime? StartTimestamp
    {
        get
        {
            DateTime? start = null;

            foreach (var kickoff in _matches.Select(x => x.Kickoff))
            {
                if (kickoff.HasValue && (!start.HasValue || kickoff.Value < start))
                {
                    start = kickoff;
                }
            }

            return start;
        }
    }

    public DateTime? EndTimestamp
    {
        get
        {
            Match? lastMatch = null;

            foreach (var match in _matches)
            {
                if (match.Kickoff.HasValue && (lastMatch is null || match.Kickoff.Value > lastMatch.Kickoff!.Value))
                {
                    lastMatch = match;
                }
            }

            if (lastMatch is null)
            {
                return null;
            }

            if (MatchPlanConfiguration?.ScheduleConfig is null)
            {
                return lastMatch.Kickoff;
            }

            var duration = lastMatch.IsGroupMatch
                ? MatchPlanConfiguration.ScheduleConfig.GroupPhasePlayTime
                : MatchPlanConfiguration.ScheduleConfig.FinalsPhasePlayTime;

            return lastMatch.Kickoff + duration;
        }
    }

    public Team AddTeam(string name)
    {
        var team = new Team(name, outOfCompetition: false);
        _teams.Add(team);

        return team;
    }

    public Group AddGroup(char alphabeticalId, string? displayName = null)
    {
        alphabeticalId = char.ToUpper(alphabeticalId);

        if (!char.IsBetween(alphabeticalId, 'A', 'Z'))
        {
            throw new TournamentException($"The specified alphabetical ID '{alphabeticalId}' must be between 'A' and 'Z'.");
        }

        if (_groups.Any(x => x.AlphabeticalId == alphabeticalId))
        {
            throw new TournamentException("The specified alphabetical id is already used by an existing group.");
        }

        var group = new Group(alphabeticalId, displayName);
        _groups.Add(group);

        return group;
    }

    public GroupParticipant AddGroupParticipant(Group group, Team team, int priority = 0)
    {
        if (!_groups.Contains(group) || !_teams.Contains(team))
        {
            throw new TournamentException("The specified group or team does not belong to this tournament");
        }

        if (_groups.Any(x => x._participants.Exists(y => y.Team == team)))
        {
            throw new TournamentException("The specified team is already a participant of another group.");
        }

        if (group._participants.Exists(x => x.Team == team))
        {
            throw new TournamentException("The specified team is already a participant of the group.");
        }

        var order = group._participants.Count == 0 ? 1 : group._participants.Max(x => x.Order) + 1;
        var participant = new GroupParticipant(group, team, order, priority);
        group._participants.Add(participant);

        return participant;
    }

    public RankingOverwrite AddRankingOverwrite(int placementRank, bool hideRanking)
    {
        if (_rankingOverwrites.Any(x => x.PlacementRank == placementRank))
        {
            throw new TournamentException($"Cannot create ranking overwrite to hide placement rank '{placementRank}' because there already exists an overwrite for this placement rank.");
        }

        var rankingOverwrite = new RankingOverwrite(placementRank, hideRanking);
        _rankingOverwrites.Add(rankingOverwrite);

        return rankingOverwrite;
    }

    public RankingOverwrite AddRankingOverwrite(int placementRank, Team? assignTeam)
    {
        if (assignTeam is not null && !_teams.Contains(assignTeam))
        {
            throw new TournamentException("The specified team does not belong to this tournament");
        }

        foreach (var existingOverwrite in _rankingOverwrites.Where(x => x.PlacementRank == placementRank))
        {
            if (existingOverwrite.HideRanking)
            {
                throw new TournamentException($"Cannot create ranking overwrite for placement rank '{placementRank}' with team assignment because there already exists an overwrite which hides this ranking.");
            }

            if (existingOverwrite.AssignTeam == assignTeam)
            {
                throw new TournamentException($"Cannot create ranking overwrite for placement rank '{placementRank}' with team assignment because there already exists an overwrite for this placement rank with the specified team.");
            }
        }

        var rankingOverwrite = new RankingOverwrite(placementRank, false)
        {
            AssignTeam = assignTeam
        };

        _rankingOverwrites.Add(rankingOverwrite);

        return rankingOverwrite;
    }

    public void RemoveTeam(Team team)
    {
        _teams.Remove(team);
    }

    public void RemoveGroup(Group group)
    {
        _groups.Remove(group);
    }

    public void RemoveRankingOverwrite(RankingOverwrite rankingOverwrite)
    {
        _rankingOverwrites.Remove(rankingOverwrite);
    }

    public void ShiftToTimezone(TimeZoneInfo timeZone)
    {
        foreach (var match in _matches)
        {
            if (match.Kickoff is not null)
            {
                match.Kickoff = TimeZoneInfo.ConvertTimeFromUtc(match.Kickoff.Value, timeZone);
            }
        }
    }

    public void SetMatchTeamSelectors(int matchIndex, TeamSelectorBase teamSelectorA, TeamSelectorBase teamSelectorB)
    {
        var match = _matches.SingleOrDefault(x => x.Index == matchIndex) ?? throw new TournamentException($"There exists no match with index {matchIndex}.");

        if (teamSelectorA is GroupDefinitionSelector groupDefinitionSelectorA)
        {
            if (teamSelectorB is GroupDefinitionSelector groupDefinitionSelectorB)
            {
                if (groupDefinitionSelectorA.TargetGroup != groupDefinitionSelectorB.TargetGroup)
                {
                    throw new TournamentException("When changing team selectors to type 'groupDefinition' both team selector A and B must reference the same group.");
                }

                match.Group = groupDefinitionSelectorA.TargetGroup;
            }
            else
            {
                throw new TournamentException("Team selector of type 'groupDefinition' cannot be combined with other team selector type.");
            }
        }
        else
        {
            if (teamSelectorB is not GroupDefinitionSelector)
            {
                match.Group = null;
            }
            else
            {
                throw new TournamentException("Team selector of type 'groupDefinition' cannot be combined with other team selector type.");
            }
        }

        match.TeamSelectorA = teamSelectorA;
        match.TeamSelectorB = teamSelectorB;

        match.TeamA = null;
        match.TeamB = null;
    }

    public void GenerateMatchPlan(MatchPlanConfiguration configuration, bool clearMatches = false)
    {
        if (configuration.GroupRoundConfig is null && configuration.FinalsRoundConfig is null)
        {
            throw new TournamentException("Either group round config or finals round config must be specified.");
        }

        if (_matches.Count > 0)
        {
            if (clearMatches)
            {
                _matches.Clear();
            }
            else
            {
                throw new TournamentException("Matches list must be empty before generating match plan.");
            }
        }

        try
        {
            GenerateGroupMatches(configuration.GroupRoundConfig, out var matchIndexOffset);
            GenerateFinalsMatches(configuration.FinalsRoundConfig, matchIndexOffset);
            GenerateSchedule(configuration.ScheduleConfig);

            // Only store the updated configuration after the generation succeeded
            MatchPlanConfiguration = configuration;
        }
        catch (Exception)
        {
            // The match plan generator modifies this tournament in-place. So when anything goes wrong, we must reset
            // to the original state which means clearing all the matches that the generator has possibly created.
            _matches.Clear();

            // Still re-throw the exception for handling by the caller.
            throw;
        }
    }

    public IEnumerable<TeamSelectorBase> GenerateAllTeamSelectors()
    {
        foreach (var team in _teams)
        {
            yield return new StaticTeamSelector(team);
        }

        foreach (var group in _groups)
        {
            for (var i = 0; i < group._participants.Count; i++)
            {
                yield return new GroupDefinitionSelector(group, i);
                yield return new GroupResultsSelector(group, i + 1);
            }
        }

        if (_groups.Count > 1)
        {
            var maxTeamsPerGroup = _groups.Max(x => x.Participants.Count);

            for (var position = 1; position <= maxTeamsPerGroup; position++)
            {
                // Determine the number of groups which have at least the required number of teams
                var numberOfSuitableGroups = _groups.Count(x => x.Participants.Count >= position);

                for (var i = 0; i < numberOfSuitableGroups; i++)
                {
                    yield return new GroupResultsNthRankedSelector([.._groups], i, position);
                }
            }
        }

        foreach (var match in _matches.Where(match => !match.IsGroupMatch))
        {
            yield return new MatchSelector(match, MatchSelector.Mode.Winner);
            yield return new MatchSelector(match, MatchSelector.Mode.Loser);
        }
    }

    public List<RefereeCard> GenerateRefereeCards()
    {
        var cards = new List<RefereeCard>();
        var groupRefereeMap = new Dictionary<Group, Group>();

        switch (_groups.Count)
        {
            case 1:
                {
                    groupRefereeMap[_groups[0]] = _groups[0];
                    break;
                }
            case 4:
                {
                    groupRefereeMap[_groups[0]] = _groups[2];
                    groupRefereeMap[_groups[1]] = _groups[3];
                    groupRefereeMap[_groups[2]] = _groups[0];
                    groupRefereeMap[_groups[3]] = _groups[1];
                    break;
                }
            default:
                {
                    for (var i = 0; i < _groups.Count; i++)
                    {
                        groupRefereeMap[_groups[i]] = _groups[(i + 1) % _groups.Count];
                    }

                    break;
                }
        }

        var refereeMatchesPerTeam = _teams.ToDictionary(x => x, _ => 0);

        foreach (var match in _matches.Where(x => x.IsGroupMatch).OrderBy(x => x.Index))
        {
            Team? refereeTeam = null;

            if (match.TeamA is not null && match.TeamB is not null && match.Group is not null)
            {
                var refereeGroup = groupRefereeMap[match.Group];

                var teams = refereeGroup.Participants.Select(x => x.Team);

                // Under the assumption, that all matches are equally long, a concurrency check can be done by comparing kickoff time only.
                var concurrentMatches = _matches.Where(other => other != match && other.Kickoff == match.Kickoff);

                var filteredTeams = teams
                    .Where(x => !match.IsTeamParticipant(x) && !concurrentMatches.Any(concurrent => concurrent.IsTeamParticipant(x)))
                    .ToList();

                if (filteredTeams.Count > 0)
                {
                    refereeTeam = filteredTeams.Count > 1
                        ? filteredTeams.MinBy(team => refereeMatchesPerTeam.GetValueOrDefault(team, 0))!
                        : filteredTeams[0];

                    refereeMatchesPerTeam[refereeTeam] = refereeMatchesPerTeam.GetValueOrDefault(refereeTeam, 0) + 1;
                }
            }

            cards.Add(new RefereeCard
            {
                Match = match,
                RefereeTeam = refereeTeam
            });
        }

        cards.AddRange(_matches
            .Where(x => !x.IsGroupMatch)
            .OrderBy(x => x.Index)
            .Select(x => new RefereeCard
            {
                Match = x,
                RefereeTeam = null
            }));

        return cards;
    }

    public void Compute()
    {
        ComputeMatches(match => match.IsGroupMatch);
        ComputeGroupPhaseResults();
        ComputeMatches(match => !match.IsGroupMatch);
        ComputeRanking();
        ComputeTeamStatistics();
    }

    private void ComputeMatches(Func<Match, bool> filter)
    {
        foreach (var match in _matches.Where(filter).OrderBy(x => x.Index))
        {
            match.TeamA = match.TeamSelectorA.GetTargetTeam(this);
            match.TeamB = match.TeamSelectorB.GetTargetTeam(this);
        }
    }

    private void ComputeGroupPhaseResults()
    {
        foreach (var group in _groups)
        {
            var groupMatches = _matches.Where(x => x.Group == group).ToList();

            foreach (var participant in group._participants)
            {
                participant.Statistics.Reset();

                var filteredMatches = groupMatches.Where(x => x.IsFinished && x.IsTeamParticipant(participant.Team));

                foreach (var match in filteredMatches)
                {
                    // We know ScoreA and ScoreB cannot be null because filteredMatches only contains matches with IsFinished = true
                    var scoreFor = (match.TeamA == participant.Team ? match.ScoreA : match.ScoreB)!.Value;
                    var scoreAgainst = (match.TeamA == participant.Team ? match.ScoreB : match.ScoreA)!.Value;

                    participant.Statistics.AddMatchOutcome(scoreFor, scoreAgainst, ComputationConfiguration);
                }
            }

            var position = 1;

            foreach (var team in group._participants.Order(_teamComparer))
            {
                team.Statistics.Position = position++;
            }
        }
    }

    private void ComputeRanking()
    {
        var positionsTemporary = new List<RankingPosition>();

        // Matches with a defined playoff position (final, 3rd playoff, etc.) are always evaluated immediately
        foreach (var match in _matches)
        {
            if (!match.PlayoffPosition.HasValue)
            {
                continue;
            }

            var reasons = match.PlayoffPosition switch
            {
                1 => (Winner: RankingReason.WinnerOfFinal, Loser: RankingReason.LoserOfFinal),
                3 => (Winner: RankingReason.WinnerOfThirdPlacePlayoff, Loser: RankingReason.LoserOfThirdPlacePlayoff),
                _ => (Winner: RankingReason.WinnerOfAdditionalPlayoff, Loser: RankingReason.LoserOfAdditionalPlayoff)
            };

            positionsTemporary.Add(new RankingPosition(match.PlayoffPosition.Value, reasons.Winner, match.GetWinningTeam()));
            positionsTemporary.Add(new RankingPosition(match.PlayoffPosition.Value + 1, reasons.Loser, match.GetLosingTeam()));
        }

        // All other rankings can are evaluated via "sections"
        var sections = new List<RankingSection>();

        // Add a ranking section for each finals round in the tournament
        sections.AddRange(_matches
            .Where(x => x.PlayoffPosition is null && x.FinalsRound is not null)
            .Select(x => x.FinalsRound)
            .Distinct()
            .Select(fr => new RankingSection(this, fr!.Value)));

        // If all group matches are finished, we can also add the section for all nonqualified teams. If any
        // group match is not finished yet, we create blank rankings below for the "missing" number of teams.
        var areAllGroupMatchesFinished = _matches.Where(x => x.IsGroupMatch).All(x => x.IsFinished);
        if (areAllGroupMatchesFinished)
        {
            sections.Add(new RankingSection(this));
        }

        // The next ranking position, given all previously defined rankings
        var nextPosition = positionsTemporary.Count > 0
            ? positionsTemporary.Max(x => x.Position) + 1
            : 1;

        // Iterate over all sections, starting from the highest tier, and copy the rankings
        foreach (var section in sections.OrderByDescending(x => x.Tier))
        {
            if (section.Size == 0)
            {
                continue;
            }

            if (section.IsDefined)
            {
                var teamsAdded = 0;

                foreach (var team in section.Teams)
                {
                    positionsTemporary.Add(new RankingPosition(nextPosition++, section.RankingReason, team));
                    teamsAdded++;
                }

                if (teamsAdded != section.Size)
                {
                    throw new TournamentException("Section contains a number of teams which is different from the specified size!");
                }
            }
            else
            {
                for (var i = 0; i < section.Size; i++)
                {
                    positionsTemporary.Add(new RankingPosition(nextPosition++, section.RankingReason, null));
                }
            }
        }

        // If any group match is not finished, create blank rankings until the number of rankings equals number of teams.
        // For explanation, see command above where the 'areAllGroupMatchesFinished' is defined.
        if (!areAllGroupMatchesFinished)
        {
            var reason = _matches.Any(x => x.FinalsRound.HasValue || x.PlayoffPosition.HasValue)
                ? RankingReason.NotQualifiedForFinals
                : RankingReason.RankingViaGroupResults;

            for (; nextPosition <= _teams.Count; nextPosition++)
            {
                positionsTemporary.Add(new RankingPosition(nextPosition, reason, null));
            }
        }

        foreach (var placementRank in _rankingOverwrites.Select(x => x.PlacementRank).Distinct())
        {
            positionsTemporary.RemoveAll(x => x.Position == placementRank);
        }

        foreach (var overwrite in _rankingOverwrites)
        {
            if (!overwrite.HideRanking)
            {
                positionsTemporary.Add(new RankingPosition(overwrite.PlacementRank, RankingReason.ManuallyChanged, overwrite.AssignTeam));
            }
        }

        _ranking.Clear();
        _ranking.AddRange(positionsTemporary
            .OrderBy(x => x.Position)
            .ThenBy(x => x.Team?.Name));
    }

    private void ComputeTeamStatistics()
    {
        foreach (var team in _teams)
        {
            team.Statistics.Reset();
        }

        foreach (var match in _matches)
        {
            if (!match.IsFinished)
            {
                continue;
            }

            // If the match outcome is 'SpecialScoring', the score of the match should not be counted towards the score of the teams.
            var ignoreScoreCounter = match.OutcomeType is MatchOutcomeType.SpecialScoring;

            match.TeamA?.Statistics.AddMatchOutcome(match.ScoreA.Value, match.ScoreB.Value, ignoreScoreCounter, ComputationConfiguration);
            match.TeamB?.Statistics.AddMatchOutcome(match.ScoreB.Value, match.ScoreA.Value, ignoreScoreCounter, ComputationConfiguration);
        }
    }

    private void GenerateGroupMatches(GroupRoundConfig? config, out int matchIndexOffset)
    {
        if (config is null)
        {
            matchIndexOffset = 0;
            return;
        }

        var groupMatchData = _groups.Select(group => new GroupMatchData(group)).ToList();

        if (groupMatchData.Count == 0)
        {
            matchIndexOffset = 0;
            return;
        }

        var singleRoundMatches = new List<GroupMatchData.MatchDefinition>();

        if (groupMatchData.Count == 1)
        {
            // In the case of one group, the group's matches can be used directly regardless of the specified group match order
            singleRoundMatches.AddRange(groupMatchData[0].MatchDefinitions);
        }
        else
        {
            switch (config.GroupMatchOrder)
            {
                case GroupMatchOrder.Alternating:
                    GenerateAlternatingGroupMatches(groupMatchData, singleRoundMatches);
                    break;
                case GroupMatchOrder.Sequential:
                    GenerateSequentialGroupMatches(groupMatchData, singleRoundMatches);
                    break;
                default:
                    throw new TournamentException($"Invalid value for 'GroupMatchOrder': {config.GroupMatchOrder}");
            }
        }

        var globalMatchIndex = 0;

        for (var i = 0; i < config.GroupPhaseRounds; i++)
        {
            // Swap the teams in every second group phase round in order to even out "home/away" matches
            var swapTeams = i % 2 == 1;

            foreach (var matchDefinition in singleRoundMatches)
            {
                var teamSelectorA = swapTeams ? matchDefinition.TeamB : matchDefinition.TeamA;
                var teamSelectorB = swapTeams ? matchDefinition.TeamA : matchDefinition.TeamB;

                _matches.Add(new Match(++globalMatchIndex, teamSelectorA, teamSelectorB, matchDefinition.Group));
            }
        }

        matchIndexOffset = globalMatchIndex;
    }

    private static void GenerateAlternatingGroupMatches(List<GroupMatchData> groupMatchData, List<GroupMatchData.MatchDefinition> destination)
    {
        var highestMatchCount = groupMatchData.Max(x => x.TotalMatchCount);

        for (var matchIndexInGroup = 0; matchIndexInGroup < highestMatchCount; matchIndexInGroup++)
        {
            foreach (var groupMatches in groupMatchData)
            {
                if (matchIndexInGroup >= groupMatches.TotalMatchCount)
                {
                    continue;
                }

                destination.Add(groupMatches.MatchDefinitions.ElementAt(matchIndexInGroup));
            }
        }
    }

    private static void GenerateSequentialGroupMatches(List<GroupMatchData> groupMatchData, List<GroupMatchData.MatchDefinition> destination)
    {
        var highestBlockCount = groupMatchData.Max(x => x.BlockCount);

        for (var blockIndex = 0; blockIndex < highestBlockCount; blockIndex++)
        {
            foreach (var groupMatches in groupMatchData)
            {
                if (blockIndex >= groupMatches.BlockCount)
                {
                    continue;
                }

                destination.AddRange(groupMatches.MatchDefinitions.Where(x => x.BlockIndex == blockIndex));
            }
        }
    }

    private void GenerateFinalsMatches(FinalsRoundConfig? config, int matchIndexOffset)
    {
        if (config is null)
        {
            return;
        }

        var globalMatchIndex = matchIndexOffset + 1;

        var firstFinalsRoundOrder = (int)config.FirstFinalsRoundOrder;
        var firstFinalsRoundMatchCount = 1 << firstFinalsRoundOrder;

        var teamSelectors = new (AbstractTeamSelector TeamA, AbstractTeamSelector TeamB)[firstFinalsRoundMatchCount];

        if (config.TeamSelectors is null)
        {
            var definition = MatchPlanDefinitions.GetFinalsMatchDefinition(_groups.Count, firstFinalsRoundMatchCount);

            if (definition is null)
            {
                throw new TournamentException($"No pre-defined first finals round configuration exists for {_groups.Count} groups and {firstFinalsRoundMatchCount} matches.");
            }

            if (_groups.Any(group => group._participants.Count < definition.RequiredTeamsPerGroup))
            {
                var lowestCount = _groups.Min(group => group._participants.Count);
                throw new TournamentException($"At least one group does not contain enough participants (present: {lowestCount}, required: {definition.RequiredTeamsPerGroup}) for the pre-defined first finals round.");
            }

            for (var i = 0; i < definition.Matches.Length; i++)
            {
                var teamSelectorA = definition.Matches[i].TeamA;
                var teamSelectorB = definition.Matches[i].TeamB;

                teamSelectors[i] = (teamSelectorA, teamSelectorB);
            }
        }
        else if (config.TeamSelectors.Count == teamSelectors.Length * 2)
        {
            for (var i = 0; i < teamSelectors.Length; i++)
            {
                var teamSelectorA = AbstractTeamSelectorParser.ParseAbstractTeamSelector(config.TeamSelectors[i * 2]);
                var teamSelectorB = AbstractTeamSelectorParser.ParseAbstractTeamSelector(config.TeamSelectors[i * 2 + 1]);

                teamSelectors[i] = (teamSelectorA, teamSelectorB);
            }
        }
        else
        {
            throw new TournamentException($"{config.TeamSelectors.Count} team selectors are specified, but {teamSelectors.Length * 2} team selectors are required.");
        }

        // If there is only one round (no. of team selectors == 2), the additional ranking matches must
        // be generated before the first round of finals matches. Otherwise, the additional ranking
        // matches are generated directly before the final match, see below.
        if (config.FirstFinalsRoundOrder == FinalsRoundOrder.FinalOnly)
        {
            GenerateAdditionalRankingMatches(config.AdditionalPlayoffs, ref globalMatchIndex);
        }

        // Generate first round using the team selectors list.
        // If there is only one round (no. of team selectors == 2), then return afterwards.
        var currentFinalsRoundMatchCount = firstFinalsRoundMatchCount;
        var selectWinningTeamFromMatch = globalMatchIndex;
        var groupsForTeamSelectors = GetGroupsForConvertingAbstractTeamSelector();
        for (var i = 0; i < currentFinalsRoundMatchCount; i++)
        {
            var teamSelectorA = ConvertToSpecificInstance(teamSelectors[i].TeamA, groupsForTeamSelectors);
            var teamSelectorB = ConvertToSpecificInstance(teamSelectors[i].TeamB, groupsForTeamSelectors);

            int? playoffPosition = firstFinalsRoundMatchCount == 1 ? 1 : null;

            _matches.Add(new Match(globalMatchIndex++, teamSelectorA, teamSelectorB, firstFinalsRoundOrder, playoffPosition));
        }

        if (firstFinalsRoundMatchCount == 1)
        {
            return;
        }

        // Generate all rounds except the final match.
        for (var currentFinalsRound = firstFinalsRoundOrder - 1; currentFinalsRound > 0; currentFinalsRound--)
        {
            currentFinalsRoundMatchCount >>= 1;

            for (var j = 0; j < currentFinalsRoundMatchCount; j++)
            {
                var teamSelectorA = CreateMatchSelector(selectWinningTeamFromMatch++, MatchSelector.Mode.Winner);
                var teamSelectorB = CreateMatchSelector(selectWinningTeamFromMatch++, MatchSelector.Mode.Winner);

                _matches.Add(new Match(globalMatchIndex++, teamSelectorA, teamSelectorB, currentFinalsRound, null));
            }
        }

        // Generate additional ranking matches. This handles the generation of these matches if there are at least
        // two finals rounds. If there is only one, the current method will have already returned above.
        GenerateAdditionalRankingMatches(config.AdditionalPlayoffs, ref globalMatchIndex);

        // Generate ranking match for 3rd place.
        if (config.EnableThirdPlacePlayoff)
        {
            if (_matches.Any(x => x.PlayoffPosition == 3))
            {
                throw new TournamentException("Can not auto-generate a third place playoff when it is already specified in additional playoffs.");
            }

            var teamSelectorA = CreateMatchSelector(selectWinningTeamFromMatch, MatchSelector.Mode.Loser);
            var teamSelectorB = CreateMatchSelector(selectWinningTeamFromMatch + 1, MatchSelector.Mode.Loser);

            _matches.Add(new Match(globalMatchIndex++, teamSelectorA, teamSelectorB, null, 3));
        }

        // Generate final match.
        var finalTeamA = CreateMatchSelector(selectWinningTeamFromMatch, MatchSelector.Mode.Winner);
        var finalTeamB = CreateMatchSelector(selectWinningTeamFromMatch + 1, MatchSelector.Mode.Winner);

        _matches.Add(new Match(globalMatchIndex, finalTeamA, finalTeamB, 0, 1));

        return;

        MatchSelector CreateMatchSelector(int index, MatchSelector.Mode mode)
        {
            var match = _matches.Single(x => x.Index == index);
            return new MatchSelector(match, mode);
        }
    }

    private void GenerateAdditionalRankingMatches(List<AdditionalPlayoffConfig>? config, ref int globalMatchIndex)
    {
        if (config is null || config.Count == 0)
        {
            return;
        }

        var distinctPlayoffPositions = config.Select(x => x.PlayoffPosition).Distinct();

        if (distinctPlayoffPositions.Count() != config.Count)
        {
            throw new TournamentException("Playoff configuration must only contain unique playoff positions.");
        }

        var groupsForTeamSelectors = GetGroupsForConvertingAbstractTeamSelector();

        foreach (var additionalPlayoff in config.OrderByDescending(x => x.PlayoffPosition))
        {
            if (additionalPlayoff.PlayoffPosition < 3 || additionalPlayoff.PlayoffPosition % 2 == 0)
            {
                throw new TournamentException($"Playoff position must be an odd integer >= 3, but specified value {additionalPlayoff.PlayoffPosition} is not.");
            }

            var teamSelectorA = ConvertToSpecificInstance(AbstractTeamSelectorParser.ParseAbstractTeamSelector(additionalPlayoff.TeamSelectorA), groupsForTeamSelectors);
            var teamSelectorB = ConvertToSpecificInstance(AbstractTeamSelectorParser.ParseAbstractTeamSelector(additionalPlayoff.TeamSelectorB), groupsForTeamSelectors);

            _matches.Add(new Match(globalMatchIndex++, teamSelectorA, teamSelectorB, null, additionalPlayoff.PlayoffPosition));
        }
    }

    private void GenerateSchedule(ScheduleConfig? config)
    {
        if (config is null)
        {
            return;
        }

        // GenerateSchedule is only called immediately after generating the group and/or finals phase.
        // Therefore, we can make the assumption here that the tournament contains a none or a certain
        // number of group phase matches followed by none or a certain amount of finals matches.

        var groupMatches = _matches.Where(x => x.IsGroupMatch).OrderBy(x => x.Index).ToList();
        var finalsMatches = _matches.Where(x => x.IsDecidingMatch).OrderBy(x => x.Index).ToList();

        var nextCourt = (short)0;
        var currentlyPlayingTeams = new HashSet<TeamSelectorBase>();
        var nextKickoffTime = config.FirstMatchKickoff;

        foreach (var groupMatch in groupMatches)
        {
            if (nextCourt >= config.GroupPhaseNumberOfCourts)
            {
                nextCourt = 0;
                nextKickoffTime += config.GroupPhasePlayTime;
                nextKickoffTime += config.GroupPhasePauseTime;
                currentlyPlayingTeams.Clear();
            }
            else if (currentlyPlayingTeams.Contains(groupMatch.TeamSelectorA) || currentlyPlayingTeams.Contains(groupMatch.TeamSelectorB))
            {
                // If either of the group match's teams is "currently" playing another match, increase the kickoff time.
                nextKickoffTime += config.GroupPhasePlayTime;
                nextKickoffTime += config.GroupPhasePauseTime;
                currentlyPlayingTeams.Clear();
            }

            groupMatch.Court = nextCourt++;
            groupMatch.Kickoff = nextKickoffTime;

            currentlyPlayingTeams.Add(groupMatch.TeamSelectorA);
            currentlyPlayingTeams.Add(groupMatch.TeamSelectorB);
        }

        if (groupMatches.Count > 0)
        {
            nextKickoffTime += config.GroupPhasePlayTime;

            if (config.PauseBetweenGroupAndFinalsPhase == TimeSpan.Zero)
            {
                // Only add another pause duration if the config does not explicitly
                // specify a pause duration between group and finals matches.
                nextKickoffTime += config.GroupPhasePauseTime;
            }
        }

        if (finalsMatches.Count > 0)
        {
            nextKickoffTime += config.PauseBetweenGroupAndFinalsPhase;

            var firstFinalsMatch = finalsMatches[0];
            var previousFinalsMatchRound = firstFinalsMatch.FinalsRound;

            firstFinalsMatch.Kickoff = nextKickoffTime;
            firstFinalsMatch.Court = 0;
            nextCourt = 1;

            foreach (var finalsMatch in finalsMatches.Skip(1))
            {
                if (finalsMatch.FinalsRound != previousFinalsMatchRound || nextCourt >= config.FinalsPhaseNumberOfCourts)
                {
                    // If a new finals round is entered, it is switched between ranking and non-ranking deciding matches
                    // or the court limit is reached, reset the court index to zero and increase the kickoff time.
                    nextCourt = 0;
                    nextKickoffTime += config.FinalsPhasePlayTime;
                    nextKickoffTime += config.FinalsPhasePauseTime;
                }

                finalsMatch.Court = nextCourt++;
                finalsMatch.Kickoff = nextKickoffTime;

                previousFinalsMatchRound = finalsMatch.FinalsRound;
            }
        }
    }

    private Group[] GetGroupsForConvertingAbstractTeamSelector()
    {
        return [.._groups.OrderBy(x => x.AlphabeticalId)];
    }

    private static TeamSelectorBase ConvertToSpecificInstance(AbstractTeamSelector abstractSelector, Group[] groups)
    {
        return abstractSelector.IsNthRanked
            ? new GroupResultsNthRankedSelector(groups, abstractSelector.OrdinalNumber.Value, abstractSelector.PlacementRank)
            : new GroupResultsSelector(groups[abstractSelector.GroupIndex.Value], abstractSelector.PlacementRank);
    }

    private sealed class GroupMatchData
    {
        private readonly List<MatchDefinition> _matchDefinitions = [];

        public GroupMatchData(Group group)
        {
            Initialize(group);
        }

        public IEnumerable<MatchDefinition> MatchDefinitions => _matchDefinitions;

        public int TotalMatchCount => _matchDefinitions.Count;

        public int BlockCount { get; private set; }

        private void Initialize(Group group)
        {
            var teamCount = group._participants.Count;
            var definition = MatchPlanDefinitions.GetGroupMatchDefinition(teamCount);

            if (definition is null)
            {
                throw new TournamentException($"No group match definitions exist for a group with {teamCount} teams.");
            }

            BlockCount = definition.BlockCount;

            for (var blockIndex = 0; blockIndex < BlockCount; blockIndex++)
            {
                foreach (var matchDefinition in definition.MatchBlocks[blockIndex].Matches)
                {
                    var teamSelectorA = new GroupDefinitionSelector(group, matchDefinition.TeamIndexA);
                    var teamSelectorB = new GroupDefinitionSelector(group, matchDefinition.TeamIndexB);
                    _matchDefinitions.Add(new MatchDefinition(group, blockIndex, teamSelectorA, teamSelectorB));
                }
            }
        }

        internal sealed record MatchDefinition(Group Group, int BlockIndex, TeamSelectorBase TeamA, TeamSelectorBase TeamB);
    }
}
