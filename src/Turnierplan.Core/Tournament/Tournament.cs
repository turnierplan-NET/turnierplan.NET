using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Image;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament.Comparers;
using Turnierplan.Core.Tournament.Definitions;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Tournament;

public sealed class Tournament : Entity<long>, IEntityWithRoleAssignments<Tournament>
{
    internal readonly GroupParticipantComparer _groupParticipantComparer;
    internal int? _nextEntityId;

    internal readonly List<RoleAssignment<Tournament>> _roleAssignments = new();
    internal readonly List<Team> _teams = new();
    internal readonly List<Group> _groups = new();
    internal readonly List<Match> _matches = new();
    internal readonly List<Document.Document> _documents = new();

    public Tournament(Organization.Organization organization, string name, Visibility visibility)
    {
        _groupParticipantComparer = new GroupParticipantComparer(this);

        organization._tournaments.Add(this);

        Id = 0;
        PublicId = new PublicId.PublicId();
        Organization = organization;
        IsMigrated = false;
        CreatedAt = DateTime.UtcNow;
        Name = name;
        Visibility = visibility;
        PublicPageViews = 0;
        ComputationConfiguration = new ComputationConfiguration();
        PresentationConfiguration = new PresentationConfiguration();
    }

    internal Tournament(long id, PublicId.PublicId publicId, bool isMigrated, DateTime createdAt, string name, Visibility visibility, int publicPageViews)
    {
        _groupParticipantComparer = new GroupParticipantComparer(this);

        Id = id;
        PublicId = publicId;
        IsMigrated = isMigrated;
        CreatedAt = createdAt;
        Name = name;
        Visibility = visibility;
        PublicPageViews = publicPageViews;
        ComputationConfiguration = new ComputationConfiguration();
        PresentationConfiguration = new PresentationConfiguration();
    }

    public override long Id { get; protected set; }

    public PublicId.PublicId PublicId { get; }

    public Organization.Organization Organization { get; internal set; } = null!;

    public IReadOnlyList<RoleAssignment<Tournament>> RoleAssignments => _roleAssignments.AsReadOnly();

    public bool IsMigrated { get; }

    public DateTime CreatedAt { get; }

    public string Name { get; set; }

    public Folder.Folder? Folder { get; internal set; }

    public Venue.Venue? Venue { get; internal set; }

    public Visibility Visibility { get; set; }

    public bool IsPublic => Visibility is Visibility.Public;

    public int PublicPageViews { get; private set; }

    public MatchPlanConfiguration? MatchPlanConfiguration { get; internal set; }

    public ComputationConfiguration ComputationConfiguration { get; set; }

    public PresentationConfiguration PresentationConfiguration { get; set; }

    public Image.Image? OrganizerLogo { get; internal set; }

    public Image.Image? SponsorLogo { get; internal set; }

    public Image.Image? SponsorBanner { get; internal set; }

    public IReadOnlyList<Team> Teams => _teams.AsReadOnly();

    public IReadOnlyList<Group> Groups => _groups.AsReadOnly();

    public IReadOnlyList<Match> Matches => _matches.AsReadOnly();

    public IReadOnlyList<Document.Document> Documents => _documents.AsReadOnly();

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

    public RoleAssignment<Tournament> AddRoleAssignment(Role role, Principal principal, string? description = null)
    {
        var roleAssignment = new RoleAssignment<Tournament>(this, role, principal, description);
        _roleAssignments.Add(roleAssignment);

        return roleAssignment;
    }

    public Team AddTeam(string name)
    {
        var team = new Team(GetNextId(), name);
        _teams.Add(team);

        return team;
    }

    public Group AddGroup(char alphabeticalId, string? displayName = null)
    {
        alphabeticalId = char.ToUpper(alphabeticalId);

        if (!char.IsBetween(alphabeticalId, 'A', 'Z'))
        {
            throw new TurnierplanException($"The specified alphabetical ID '{alphabeticalId}' must be between 'A' and 'Z'.");
        }

        if (_groups.Any(x => x.AlphabeticalId == alphabeticalId))
        {
            throw new TurnierplanException("The specified alphabetical id is already used by an existing group.");
        }

        var group = new Group(GetNextId(), alphabeticalId, displayName);
        _groups.Add(group);

        return group;
    }

    public GroupParticipant AddGroupParticipant(Group group, Team team, int priority = 0)
    {
        if (!_groups.Contains(group) || !_teams.Contains(team))
        {
            throw new TurnierplanException("The specified group or team does not belong to this tournament");
        }

        if (_groups.Any(x => x._participants.Exists(y => y.Team == team)))
        {
            throw new TurnierplanException("The specified team is already a participant of another group.");
        }

        if (group._participants.Exists(x => x.Team == team))
        {
            throw new TurnierplanException("The specified team is already a participant of the group.");
        }

        var order = group._participants.Count == 0 ? 1 : group._participants.Max(x => x.Order) + 1;
        var participant = new GroupParticipant(group, team, order, priority);
        group._participants.Add(participant);

        return participant;
    }

    public void RemoveTeam(Team team)
    {
        _teams.Remove(team);
    }

    public void RemoveGroup(Group group)
    {
        _groups.Remove(group);
    }

    public void SetFolder(Folder.Folder? folder)
    {
        if (folder is not null && folder.Organization != Organization)
        {
            throw new TurnierplanException("Cannot assign a folder from another organization.");
        }

        Folder?._tournaments.Remove(this);

        Folder = folder;

        Folder?._tournaments.Add(this);
    }

    public void SetVenue(Venue.Venue? venue)
    {
        if (venue is not null && venue.Organization != Organization)
        {
            throw new TurnierplanException("Cannot assign a venue from another organization.");
        }

        Venue?._tournaments.Remove(this);

        Venue = venue;

        Venue?._tournaments.Add(this);
    }

    public void SetOrganizerLogo(Image.Image? organizerLogo)
    {
        CheckImageTypeAndSetImage(organizerLogo, ImageType.SquareLargeLogo, () => OrganizerLogo = organizerLogo);
    }

    public void SetSponsorLogo(Image.Image? sponsorLogo)
    {
        CheckImageTypeAndSetImage(sponsorLogo, ImageType.SquareLargeLogo, () => SponsorLogo = sponsorLogo);
    }

    public void SetSponsorBanner(Image.Image? sponsorBanner)
    {
        CheckImageTypeAndSetImage(sponsorBanner, ImageType.SponsorBanner, () => SponsorBanner = sponsorBanner);
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

    public void IncrementPublicPageViews()
    {
        if (Visibility != Visibility.Public)
        {
            throw new TurnierplanException($"Cannot increment page view counter when visibility is '{Visibility.Public}'.");
        }

        if (PublicPageViews == int.MaxValue)
        {
            return;
        }

        PublicPageViews++;
    }

    public void SetMatchOrder(Dictionary<int, int> matchIndices)
    {
        if (!matchIndices.Keys.ToHashSet().SetEquals(Matches.Select(x => x.Id)))
        {
            throw new TurnierplanException("The given match IDs must equal the existing match IDs.");
        }

        if (matchIndices.Values.Distinct().Count() != matchIndices.Count)
        {
            throw new TurnierplanException("The match indices must be unique.");
        }

        if (!matchIndices.Values.IsSequential(1))
        {
            throw new TurnierplanException("The match indices must be sequential, starting from 1.");
        }

        foreach (var match in Matches)
        {
            match.Index = matchIndices[match.Id];
        }
    }

    public void SetMatchTeamSelectors(int matchId, TeamSelectorBase teamSelectorA, TeamSelectorBase teamSelectorB)
    {
        var match = _matches.SingleOrDefault(x => x.Id == matchId) ?? throw new TurnierplanException($"There exists no match with id {matchId}.");

        if (teamSelectorA is GroupDefinitionSelector groupDefinitionSelectorA)
        {
            if (teamSelectorB is GroupDefinitionSelector groupDefinitionSelectorB)
            {
                if (groupDefinitionSelectorA.TargetGroupId != groupDefinitionSelectorB.TargetGroupId)
                {
                    throw new TurnierplanException("When changing team selectors to type 'groupDefinition' both team selector A and B must reference the same group.");
                }

                var groupId = groupDefinitionSelectorA.TargetGroupId;
                match.Group = _groups.SingleOrDefault(x => x.Id == groupId) ?? throw new TurnierplanException($"There is no group with id {groupId}");
            }
            else
            {
                throw new TurnierplanException("Team selector of type 'groupDefinition' cannot be combined with other team selector type.");
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
                throw new TurnierplanException("Team selector of type 'groupDefinition' cannot be combined with other team selector type.");
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
            throw new TurnierplanException("Either group round config or finals round config must be specified.");
        }

        if (_matches.Count > 0)
        {
            if (clearMatches)
            {
                _matches.Clear();
            }
            else
            {
                throw new TurnierplanException("Matches list must be empty before generating match plan.");
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
            yield return new StaticTeamSelector(team.Id);
        }

        foreach (var group in _groups)
        {
            for (var i = 0; i < group._participants.Count; i++)
            {
                yield return new GroupDefinitionSelector(group.Id, i);
                yield return new GroupResultsSelector(group.Id, i + 1);
            }
        }

        if (_groups.Count > 1)
        {
            var groupIds = _groups.Select(x => x.Id).ToArray();
            var maxTeamsPerGroup = _groups.Max(x => x.Participants.Count);

            for (var position = 1; position <= maxTeamsPerGroup; position++)
            {
                // Determine the number of groups which have at least the required number of teams
                var numberOfSuitableGroups = _groups.Count(x => x.Participants.Count >= position);

                for (var i = 0; i < numberOfSuitableGroups; i++)
                {
                    yield return new GroupResultsNthRankedSelector(groupIds, i, position);
                }
            }
        }

        foreach (var index in _matches.Where(match => !match.IsGroupMatch).Select(match => match.Index))
        {
            yield return new MatchSelector(index, MatchSelector.Mode.Winner);
            yield return new MatchSelector(index, MatchSelector.Mode.Loser);
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
                    var scoreFor = (match.TeamA == participant.Team ? match.ScoreA : match.ScoreB) ?? 0;
                    var scoreAgainst = (match.TeamA == participant.Team ? match.ScoreB : match.ScoreA) ?? 0;

                    participant.Statistics.AddMatchOutcome(scoreFor, scoreAgainst, ComputationConfiguration);
                }
            }

            var position = 1;

            foreach (var team in group._participants.Order(_groupParticipantComparer))
            {
                team.Statistics.Position = position++;
            }
        }
    }

    private void ComputeRanking()
    {
        var matchesWithRankingInfluence = new HashSet<Match>();
        var undefinedRankings = Enumerable.Range(1, _teams.Count).ToList();

        foreach (var team in _teams)
        {
            team.Ranking = null;
        }

        void AttemptToAssignRankingToTeam(Team? team, int ranking)
        {
            if (team is not null)
            {
                team.Ranking = ranking;
            }
        }

        foreach (var match in _matches.Where(x => x.PlayoffPosition is not null))
        {
            matchesWithRankingInfluence.Add(match);

            var winnerRanking = match.PlayoffPosition!.Value;
            var loserRanking = match.PlayoffPosition!.Value + 1;

            if (!undefinedRankings.Contains(winnerRanking))
            {
                throw new TurnierplanException($"Match {match.Id} attempts to redefine ranking {winnerRanking}.");
            }

            if (!undefinedRankings.Contains(loserRanking))
            {
                throw new TurnierplanException($"Match {match.Id} attempts to redefine ranking {loserRanking}.");
            }

            // If the match is not finished, the winning/losing team is null and no ranking will be assigned
            AttemptToAssignRankingToTeam(match.GetWinningTeam(), winnerRanking);
            AttemptToAssignRankingToTeam(match.GetLosingTeam(), loserRanking);

            undefinedRankings.Remove(winnerRanking);
            undefinedRankings.Remove(loserRanking);
        }

        if (undefinedRankings.Count == 0
            || matchesWithRankingInfluence.Any(x => x.TeamA is null || x.TeamB is null)
            || _matches.Where(x => x.IsGroupMatch).Any(x => !x.IsFinished))
        {
            return;
        }

        var eligibleTeams = _teams
            .Where(team => !matchesWithRankingInfluence.Any(match => match.IsTeamParticipant(team)))
            .Select(team =>
            {
                var participations = _groups
                    .Select(group => group._participants.SingleOrDefault(participant => participant.Team == team))
                    .WhereNotNull()
                    .ToList();

                if (participations.Count == 0)
                {
                    return new RankingEligibleTeam(null, int.MaxValue);
                }

                var source = participations[0];
                var priority = participations.Max(x => x.Priority);
                var combinedGroupStats = new GroupParticipant(source.Group, source.Team, 0, priority)
                {
                    Statistics = participations.Select(x => x.Statistics).Combine()
                };

                var bestFinalsRoundPlayed = _matches
                    .Where(match => match is { FinalsRound: not null, PlayoffPosition: null } && match.IsTeamParticipant(team))
                    .Select(x => x.FinalsRound)
                    .Order() // Lower 'FinalsRound' is "better"
                    .FirstOrDefault() ?? int.MaxValue;

                return new RankingEligibleTeam(combinedGroupStats, bestFinalsRoundPlayed);
            })
            .Where(x => x.CombinedGroupStats is not null)
            .GroupBy(x => x.BestFinalsRoundPlayed)
            .OrderBy(x => x.Key);

        var undefinedRankingsIndex = 0;

        foreach (var grouping in eligibleTeams)
        {
            // NOTE: The instances of GroupParticipant are not the original ones as they appear in the tournament's groups. Rather,
            // they are constructed using a certain team's id and the combined statistics of all the team's participations.
            var sortedTeams = grouping.Select(x => x.CombinedGroupStats!).Order(_groupParticipantComparer);

            foreach (var participant in sortedTeams)
            {
                participant.Team.Ranking = undefinedRankings[undefinedRankingsIndex++];
            }
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
                    throw new TurnierplanException($"Invalid value for 'GroupMatchOrder': {config.GroupMatchOrder}");
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

                _matches.Add(new Match(GetNextId(), ++globalMatchIndex, teamSelectorA, teamSelectorB, matchDefinition.Group));
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
                throw new TurnierplanException($"No pre-defined first finals round configuration exists for {_groups.Count} groups and {firstFinalsRoundMatchCount} matches.");
            }

            for (var i = 0; i < definition.Matches.Count; i++)
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
            throw new TurnierplanException($"{config.TeamSelectors.Count} team selectors are specified, but {teamSelectors.Length * 2} team selectors are required.");
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
        var groupIds = GetGroupIdsForConvertingAbstractTeamSelector();
        for (var i = 0; i < currentFinalsRoundMatchCount; i++)
        {
            var teamSelectorA = ConvertToSpecificInstance(teamSelectors[i].TeamA, groupIds);
            var teamSelectorB = ConvertToSpecificInstance(teamSelectors[i].TeamB, groupIds);

            int? playoffPosition = firstFinalsRoundMatchCount == 1 ? 1 : null;

            _matches.Add(new Match(GetNextId(), globalMatchIndex++, teamSelectorA, teamSelectorB, firstFinalsRoundOrder, playoffPosition));
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
                var teamSelectorA = new MatchSelector(selectWinningTeamFromMatch++, MatchSelector.Mode.Winner);
                var teamSelectorB = new MatchSelector(selectWinningTeamFromMatch++, MatchSelector.Mode.Winner);

                _matches.Add(new Match(GetNextId(), globalMatchIndex++, teamSelectorA, teamSelectorB, currentFinalsRound, null));
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
                throw new TurnierplanException("Can not auto-generate a third place playoff when it is already specified in additional playoffs.");
            }

            var teamSelectorA = new MatchSelector(selectWinningTeamFromMatch, MatchSelector.Mode.Loser);
            var teamSelectorB = new MatchSelector(selectWinningTeamFromMatch + 1, MatchSelector.Mode.Loser);

            _matches.Add(new Match(GetNextId(), globalMatchIndex++, teamSelectorA, teamSelectorB, null, 3));
        }

        // Generate final match.
        var finalTeamA = new MatchSelector(selectWinningTeamFromMatch, MatchSelector.Mode.Winner);
        var finalTeamB = new MatchSelector(selectWinningTeamFromMatch + 1, MatchSelector.Mode.Winner);

        _matches.Add(new Match(GetNextId(), globalMatchIndex, finalTeamA, finalTeamB, 0, 1));
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
            throw new TurnierplanException("Playoff configuration must only contain unique playoff positions.");
        }

        var groupIds = GetGroupIdsForConvertingAbstractTeamSelector();

        foreach (var additionalPlayoff in config.OrderByDescending(x => x.PlayoffPosition))
        {
            if (additionalPlayoff.PlayoffPosition < 3 || additionalPlayoff.PlayoffPosition % 2 == 0)
            {
                throw new TurnierplanException($"Playoff position must be an odd integer >= 3, but specified value {additionalPlayoff.PlayoffPosition} is not.");
            }

            var teamSelectorA = ConvertToSpecificInstance(AbstractTeamSelectorParser.ParseAbstractTeamSelector(additionalPlayoff.TeamSelectorA), groupIds);
            var teamSelectorB = ConvertToSpecificInstance(AbstractTeamSelectorParser.ParseAbstractTeamSelector(additionalPlayoff.TeamSelectorB), groupIds);

            _matches.Add(new Match(GetNextId(), globalMatchIndex++, teamSelectorA, teamSelectorB, null, additionalPlayoff.PlayoffPosition));
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

    private int[] GetGroupIdsForConvertingAbstractTeamSelector()
    {
        return _groups
            .OrderBy(x => x.AlphabeticalId)
            .ThenBy(x => x.Id)
            .Select(x => x.Id)
            .ToArray();
    }

    private void CheckImageTypeAndSetImage(Image.Image? provided, ImageType expectedType, Action apply)
    {
        if (provided is null)
        {
            apply();
            return;
        }

        if (provided.Organization != Organization)
        {
            throw new TurnierplanException("Cannot assign an image from another organization.");
        }

        if (provided.Type != expectedType)
        {
            throw new TurnierplanException($"Cannot assign image because the image's type is not the expected type '{expectedType}'.");
        }

        apply();
    }

    private int GetNextId()
    {
        if (_nextEntityId is null)
        {
            var allIds = Teams.Select(x => x.Id).Union(Groups.Select(x => x.Id)).Union(Matches.Select(x => x.Id)).ToList();
            _nextEntityId = allIds.Count == 0 ? 0 : allIds.Max();
        }

        _nextEntityId++;

        return _nextEntityId.Value;
    }

    private static TeamSelectorBase ConvertToSpecificInstance(AbstractTeamSelector abstractSelector, int[] groupIds)
    {
        return abstractSelector.IsNthRanked
            ? new GroupResultsNthRankedSelector(groupIds, abstractSelector.OrdinalNumber!.Value, abstractSelector.PlacementRank)
            : new GroupResultsSelector(groupIds[abstractSelector.GroupIndex!.Value], abstractSelector.PlacementRank);
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
                throw new TurnierplanException($"No group match definitions exist for a group with {teamCount} teams.");
            }

            BlockCount = definition.BlockCount;

            for (var blockIndex = 0; blockIndex < BlockCount; blockIndex++)
            {
                foreach (var matchDefinition in definition.MatchBlocks[blockIndex].Matches)
                {
                    var teamSelectorA = new GroupDefinitionSelector(group.Id, matchDefinition.TeamIndexA);
                    var teamSelectorB = new GroupDefinitionSelector(group.Id, matchDefinition.TeamIndexB);
                    _matchDefinitions.Add(new MatchDefinition(group, blockIndex, teamSelectorA, teamSelectorB));
                }
            }
        }

        internal sealed record MatchDefinition(Group Group, int BlockIndex, TeamSelectorBase TeamA, TeamSelectorBase TeamB);
    }

    private sealed record RankingEligibleTeam(GroupParticipant? CombinedGroupStats, int BestFinalsRoundPlayed);
}
