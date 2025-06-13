using Turnierplan.App.Models;
using Turnierplan.App.Models.Enums;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Converters;
using MatchType = Turnierplan.App.Models.Enums.MatchType;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class TournamentMappingRule : MappingRuleBase<Tournament, TournamentDto>
{
    protected override TournamentDto Map(IMapper mapper, MappingContext context, Tournament source)
    {
        return new TournamentDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            FolderId = source.Folder?.PublicId,
            VenueId = source.Venue?.PublicId,
            Name = source.Name,
            OrganizationName = source.Organization.Name,
            FolderName = source.Folder?.Name,
            VenueName = source.Venue?.Name,
            Visibility = source.Visibility,
            PublicPageViews = source.PublicPageViews,
            Teams = source.Teams.Select(team =>
            {
                return new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    OutOfCompetition = team.OutOfCompetition,
                    HasPaidEntryFee = team.EntryFeePaidAt is not null,
                    EntryFeePaidAt = team.EntryFeePaidAt
                };
            }).OrderBy(x => x.Name).ToArray(),
            Groups = source.Groups.Select(group =>
            {
                return new GroupDto
                {
                    Id = group.Id,
                    AlphabeticalId = group.AlphabeticalId,
                    DisplayName = context.Localization.LocalizeGroupName(group),
                    HasCustomDisplayName = group.DisplayName is not null,
                    Participants = group.Participants.Select(participant =>
                    {
                        return new GroupParticipantDto
                        {
                            TeamId = participant.Team.Id,
                            Priority = participant.Priority,
                            Statistics = new TeamGroupStatisticsDto
                            {
                                Position = participant.Statistics.Position,
                                ScoreFor = participant.Statistics.ScoreFor,
                                ScoreAgainst = participant.Statistics.ScoreAgainst,
                                ScoreDifference = participant.Statistics.ScoreDifference,
                                MatchesPlayed = participant.Statistics.MatchesPlayed,
                                MatchesWon = participant.Statistics.MatchesWon,
                                MatchesDrawn = participant.Statistics.MatchesDrawn,
                                MatchesLost = participant.Statistics.MatchesLost,
                                Points = participant.Statistics.Points
                            }
                        };
                    }).ToArray()
                };
            }).OrderBy(x => x.AlphabeticalId).ToArray(),
            Matches = source.Matches.Select(match =>
            {
                return new MatchDto
                {
                    Id = match.Id,
                    Index = match.Index,
                    Court = match.Court,
                    Kickoff = match.Kickoff,
                    Type = DetermineMatchType(match),
                    FormattedType = context.Localization.LocalizeMatchDisplayName(match),
                    GroupId = match.Group?.Id,
                    FinalsRound = match.FinalsRound,
                    PlayoffPosition = match.PlayoffPosition,
                    TeamA = new MatchTeamInfoDto
                    {
                        TeamSelector = new TeamSelectorDto
                        {
                            Key = TeamSelectorConverter.ConvertTeamSelectorToString(match.TeamSelectorA),
                            Localized = context.Localization.LocalizeTeamSelector(match.TeamSelectorA, source)
                        },
                        TeamId = match.TeamA?.Id,
                        Score = match.ScoreA
                    },
                    TeamB = new MatchTeamInfoDto
                    {
                        TeamSelector = new TeamSelectorDto
                        {
                            Key = TeamSelectorConverter.ConvertTeamSelectorToString(match.TeamSelectorB),
                            Localized = context.Localization.LocalizeTeamSelector(match.TeamSelectorB, source)
                        },
                        TeamId = match.TeamB?.Id,
                        Score = match.ScoreB
                    },
                    State = DetermineMatchState(match),
                    OutcomeType = match.OutcomeType
                };
            }).OrderBy(x => x.Index).ToArray(),
            Rankings = Enumerable.Range(1, source.Teams.Count).Select(ranking =>
            {
                var teamId = source.Teams.FirstOrDefault(x => x.Ranking == ranking)?.Id;

                return new RankingDto
                {
                    PlacementRank = ranking,
                    IsDefined = teamId is not null,
                    TeamId = teamId
                };
            }).ToArray(),
            MatchPlanConfiguration = new MatchPlanConfigurationDto
            {
                FirstMatchKickoff = source.MatchPlanConfiguration?.ScheduleConfig?.FirstMatchKickoff,
                GroupPhaseConfig = MapGroupRoundConfiguration(source.MatchPlanConfiguration?.GroupRoundConfig, source.MatchPlanConfiguration?.ScheduleConfig),
                PauseBetweenGroupAndFinalsPhase =
                    source.MatchPlanConfiguration?.GroupRoundConfig is not null && source.MatchPlanConfiguration?.FinalsRoundConfig is not null
                        ? source.MatchPlanConfiguration.ScheduleConfig?.PauseBetweenGroupAndFinalsPhase
                        : null,
                FinalsPhaseConfig = MapFinalsRoundConfiguration(source.MatchPlanConfiguration?.FinalsRoundConfig, source.MatchPlanConfiguration?.ScheduleConfig)
            },
            ComputationConfiguration = new ComputationConfigurationDto
            {
                MatchWonPoints = source.ComputationConfiguration.MatchWonPoints,
                MatchDrawnPoints = source.ComputationConfiguration.MatchDrawnPoints,
                MatchLostPoints = source.ComputationConfiguration.MatchLostPoints,
                HigherScoreLoses = source.ComputationConfiguration.HigherScoreLoses,
                ComparisonModes = source.ComputationConfiguration.ComparisonModes.ToArray()
            },
            PresentationConfiguration = mapper.Map<PresentationConfigurationDto>(source.PresentationConfiguration)
        };
    }

    private static GroupPhaseConfigurationDto? MapGroupRoundConfiguration(GroupRoundConfig? groupRoundConfig, ScheduleConfig? scheduleConfig)
    {
        if (groupRoundConfig is null)
        {
            return null;
        }

        return new GroupPhaseConfigurationDto
        {
            Schedule = scheduleConfig is null ? null : new ScheduleConfigurationDto
            {
                PlayTime = scheduleConfig.GroupPhasePlayTime,
                PauseTime = scheduleConfig.GroupPhasePauseTime
            },
            NumberOfCourts = scheduleConfig?.GroupPhaseNumberOfCourts ?? 1,
            UseAlternatingOrder = groupRoundConfig.GroupMatchOrder == GroupMatchOrder.Alternating,
            NumberOfGroupRounds = groupRoundConfig.GroupPhaseRounds
        };
    }

    private static FinalsPhaseConfigurationDto? MapFinalsRoundConfiguration(FinalsRoundConfig? finalsRoundConfig, ScheduleConfig? scheduleConfig)
    {
        if (finalsRoundConfig is null)
        {
            return null;
        }

        return new FinalsPhaseConfigurationDto
        {
            Schedule = scheduleConfig is null ? null : new ScheduleConfigurationDto
            {
                PlayTime = scheduleConfig.FinalsPhasePlayTime,
                PauseTime = scheduleConfig.FinalsPhasePauseTime
            },
            NumberOfCourts = scheduleConfig?.FinalsPhaseNumberOfCourts ?? 1,
            FirstFinalsRound = (int)finalsRoundConfig.FirstFinalsRoundOrder,
            ThirdPlacePlayoff = finalsRoundConfig.EnableThirdPlacePlayoff,
            AdditionalPlayoffs = (finalsRoundConfig.AdditionalPlayoffs ?? []).Select(x => new AdditionalPlayoffDto
            {
                PlayoffPosition = x.PlayoffPosition,
                TeamSelectorA = x.TeamSelectorA,
                TeamSelectorB = x.TeamSelectorB
            }).ToArray()
        };
    }

    private static MatchState DetermineMatchState(Match match)
    {
        if (match.IsFinished)
        {
            return MatchState.Finished;
        }

        return match.IsCurrentlyPlaying == true ? MatchState.CurrentlyPlaying : MatchState.NotStarted;
    }

    private static MatchType DetermineMatchType(Match source)
    {
        if (source.IsGroupMatch)
        {
            return MatchType.GroupMatch;
        }

        switch (source.PlayoffPosition)
        {
            case 1:
                return MatchType.Final;
            case 3:
                return MatchType.ThirdPlacePlayoff;
            case not null:
                return MatchType.AdditionalPlayoff;
        }

        if (source.FinalsRound is not null)
        {
            return MatchType.NonFinalKnockout;
        }

        throw new InvalidOperationException($"Could not determine match type of match with id {source.Id}.");
    }
}
