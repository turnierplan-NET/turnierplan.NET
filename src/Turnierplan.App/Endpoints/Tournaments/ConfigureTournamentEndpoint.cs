using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.Definitions;
using Turnierplan.Dal;
using Turnierplan.Dal.Extensions;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class ConfigureTournamentEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/tournaments/{id}/configuration";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] ConfigureTournamentEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(id, ITournamentRepository.Include.GameRelevant).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(tournament.Organization))
        {
            return Results.Forbid();
        }

        DeleteNoLongerNeededTeams(tournament, request);
        DeleteNoLongerNeededGroups(tournament, request);

        var groupMappings = new Dictionary<int, Group>();
        var teamMappings = new Dictionary<(int GroupIndex, int TeamIndex), Team>();
        var existingTeamPriorities = new Dictionary<(int GroupId, int TeamId), int>();

        for (var i = 0; i < request.Groups.Length; i++)
        {
            var requestGroup = request.Groups[i];

            var groupDisplayName = requestGroup.DisplayName?.Trim() ?? string.Empty;
            groupDisplayName = groupDisplayName.Length == 0 ? null : groupDisplayName;

            var group = requestGroup.Id is null
                ? tournament.AddGroup(requestGroup.AlphabeticalId, groupDisplayName)
                : tournament.Groups.FirstOrDefault(x => x.Id == requestGroup.Id);

            if (group is null)
            {
                return Results.BadRequest($"Missing group with id {requestGroup.Id}");
            }

            if (requestGroup.Id is not null)
            {
                if (group.AlphabeticalId != requestGroup.AlphabeticalId)
                {
                    return Results.BadRequest("Group alphabetical ID may not change.");
                }

                group.DisplayName = groupDisplayName;
            }

            // Always re-generate all participations but store all existing priorities to re-use later
            foreach (var participant in group.Participants)
            {
                existingTeamPriorities[(GroupId: group.Id, TeamId: participant.Team.Id)] = participant.Priority;
            }

            group.ClearParticipants();

            groupMappings[i] = group;
        }

        for (var i = 0; i < request.Groups.Length; i++)
        {
            var requestGroup = request.Groups[i];

            for (var j = 0; j < requestGroup.Teams.Length; j++)
            {
                var requestTeam = requestGroup.Teams[j];

                var team = requestTeam.Id is null
                    ? tournament.AddTeam(requestTeam.Name)
                    : tournament.Teams.FirstOrDefault(x => x.Id == requestTeam.Id);

                if (team is null)
                {
                    return Results.BadRequest($"Missing team with id {requestTeam.Id}");
                }

                if (requestTeam.Id is not null)
                {
                    team.Name = requestTeam.Name;
                }

                teamMappings[(GroupIndex: i, TeamIndex: j)] = team;
            }
        }

        // Wrap the code below in a transaction such that the modifications to the tournament are only
        // stored to the database if no exception occurs between the two SaveChangesAsync() calls.

        await using (var transaction = await repository.UnitOfWork.WrapTransactionAsync().ConfigureAwait(false))
        {
            // Save changes to generate IDs for all new groups and teams
            await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            for (var i = 0; i < request.Groups.Length; i++)
            {
                var requestGroup = request.Groups[i];
                var group = groupMappings[i];

                for (var j = 0; j < requestGroup.Teams.Length; j++)
                {
                    var team = teamMappings[(GroupIndex: i, TeamIndex: j)];
                    var priority = existingTeamPriorities.GetValueOrDefault((GroupId: group.Id, TeamId: team.Id), 1);

                    tournament.AddGroupParticipant(group, team, priority);
                }
            }

            var configuration = ConvertMatchPlanConfiguration(request);
            tournament.GenerateMatchPlan(configuration, clearMatches: true);

            await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            transaction.ShouldCommit = true;
        }

        return Results.NoContent();
    }

    private static void DeleteNoLongerNeededTeams(Tournament tournament, ConfigureTournamentEndpointRequest request)
    {
        var requestTeamIds = request.Groups
            .SelectMany(x => x.Teams)
            .Select(x => x.Id)
            .Where(x => x is not null)
            .Distinct()
            .ToList();

        tournament.Teams
            .Where(x => !requestTeamIds.Contains(x.Id))
            .ToList()
            .ForEach(tournament.RemoveTeam);
    }

    private static void DeleteNoLongerNeededGroups(Tournament tournament, ConfigureTournamentEndpointRequest request)
    {
        var requestGroupIds = request.Groups
            .Select(x => x.Id)
            .Where(x => x is not null)
            .Distinct()
            .ToList();

        tournament.Groups
            .Where(x => !requestGroupIds.Contains(x.Id))
            .ToList()
            .ForEach(tournament.RemoveGroup);
    }

    private static MatchPlanConfiguration ConvertMatchPlanConfiguration(ConfigureTournamentEndpointRequest request)
    {
        return new MatchPlanConfiguration
        {
            GroupRoundConfig = request.GroupPhase is null ? null : new GroupRoundConfig
            {
                GroupMatchOrder = request.GroupPhase.UseAlternatingOrder ? GroupMatchOrder.Alternating : GroupMatchOrder.Sequential,
                GroupPhaseRounds = request.GroupPhase.NumberOfGroupRounds
            },
            FinalsRoundConfig = request.FinalsPhase is null ? null : new FinalsRoundConfig
            {
                FirstFinalsRoundOrder = (FinalsRoundOrder)request.FinalsPhase.FirstFinalsRound,
                TeamSelectors = null,
                EnableThirdPlacePlayoff = request.FinalsPhase.ThirdPlacePlayoff,
                AdditionalPlayoffs = request.FinalsPhase.AdditionalPlayoffs.Length > 0
                    ? request.FinalsPhase.AdditionalPlayoffs.Select(x => new AdditionalPlayoffConfig(x.PlayoffPosition, x.TeamSelectorA, x.TeamSelectorB)).ToList()
                    : null
            },
            ScheduleConfig = ConvertScheduleConfig(request)
        };
    }

    private static ScheduleConfig? ConvertScheduleConfig(ConfigureTournamentEndpointRequest request)
    {
        if (request.FirstMatchKickoff is null)
        {
            return null;
        }

        return new ScheduleConfig
        {
            FirstMatchKickoff = request.FirstMatchKickoff.Value,
            GroupPhaseNumberOfCourts = request.GroupPhase?.NumberOfCourts ?? 1,
            GroupPhasePlayTime = request.GroupPhase?.Schedule?.PlayTime ?? TimeSpan.FromMinutes(10),
            GroupPhasePauseTime = request.GroupPhase?.Schedule?.PauseTime ?? TimeSpan.FromMinutes(1),
            PauseBetweenGroupAndFinalsPhase = request.PauseBetweenGroupAndFinalsPhase ?? TimeSpan.Zero,
            FinalsPhaseNumberOfCourts = request.FinalsPhase?.NumberOfCourts ?? 1,
            FinalsPhasePlayTime = request.FinalsPhase?.Schedule?.PlayTime ?? TimeSpan.FromMinutes(10),
            FinalsPhasePauseTime = request.FinalsPhase?.Schedule?.PauseTime ?? TimeSpan.FromMinutes(1)
        };
    }

    public sealed record ConfigureTournamentEndpointRequest
    {
        public required ConfigureTournamentEndpointRequestGroupEntry[] Groups { get; init; }

        public DateTime? FirstMatchKickoff { get; init; }

        public GroupPhaseConfigurationDto? GroupPhase { get; init; }

        public TimeSpan? PauseBetweenGroupAndFinalsPhase { get; init; }

        public FinalsPhaseConfigurationDto? FinalsPhase { get; init; }

        public sealed record ConfigureTournamentEndpointRequestGroupEntry(int? Id, char AlphabeticalId, string? DisplayName, ConfigureTournamentEndpointRequestTeamEntry[] Teams);

        public sealed record ConfigureTournamentEndpointRequestTeamEntry(int? Id, string Name);
    }

    internal sealed class Validator : AbstractValidator<ConfigureTournamentEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Groups)
                .NotEmpty()
                .WithMessage("Configuration must include at least one group.");

            RuleFor(x => x.Groups)
                .Must(x => x.Sum(y => y.Teams.Length) <= ValidationConstants.Tournament.MaxNumberOfTeams)
                .WithMessage($"Configuration may include at most {ValidationConstants.Tournament.MaxNumberOfTeams} teams.");

            RuleFor(x => x.Groups.Select(y => y.AlphabeticalId).ToList())
                .Must(x => x.Count == x.Distinct().Count())
                .WithMessage("Group alphabetical IDs must be unique.")
                .OverridePropertyName(nameof(ConfigureTournamentEndpointRequest.Groups));

            RuleFor(x => x.Groups.Select(y => y.Id).Where(y => y.HasValue).ToList())
                .Must(x => x.Count == x.Distinct().Count())
                .WithMessage("Configuration may not contain more than 1 entry per existing group ID.")
                .OverridePropertyName(nameof(ConfigureTournamentEndpointRequest.Groups));

            RuleFor(x => x.Groups.SelectMany(y => y.Teams).Select(y => y.Id).Where(y => y.HasValue).ToList())
                .Must(x => x.Count == x.Distinct().Count())
                .WithMessage("Configuration may not contain more than 1 entry per existing team ID.")
                .OverridePropertyName(nameof(ConfigureTournamentEndpointRequest.Groups));

            RuleForEach(x => x.Groups).ChildRules(group =>
            {
                group.RuleFor(x => x.AlphabeticalId)
                    .GreaterThanOrEqualTo('A')
                    .WithMessage("Group alphabetical ID must be between 'A' and 'Z'.")
                    .LessThanOrEqualTo('Z')
                    .WithMessage("Group alphabetical ID must be between 'A' and 'Z'.");

                group.RuleFor(x => x.DisplayName)
                    .NotEmpty()
                    .WithMessage("Group display name must be null or a non-empty string with at most 25 characters.")
                    .MaximumLength(ValidationConstants.Group.MaxDisplayNameLength)
                    .WithMessage("Group display name must be null or a non-empty string with at most 25 characters.")
                    .When(y => y.DisplayName is not null);

                group.RuleFor(x => x.Teams)
                    .Must(x => x.Length >= 2)
                    .WithMessage("Each group must contain at least two teams.")
                    .Must(x => x.Length <= ValidationConstants.Group.MaxNumberOfTeams)
                    .WithMessage($"Each group must contain at most {ValidationConstants.Group.MaxNumberOfTeams} teams.");

                group.RuleForEach(x => x.Teams).ChildRules(team =>
                {
                    team.RuleFor(x => x.Name)
                        .NotEmpty()
                        .WithMessage($"Team name must be a non-empty string with at most {ValidationConstants.Team.MaxNameLength} characters.")
                        .MaximumLength(ValidationConstants.Team.MaxNameLength)
                        .WithMessage($"Team name must be a non-empty string with at most {ValidationConstants.Team.MaxNameLength} characters.");
                });
            });

            RuleFor(x => x)
                .Must(x => x.GroupPhase is not null || x.FinalsPhase is not null)
                .WithMessage("Either group phase, finals phase, or both must be specified.");

            RuleFor(x => x.GroupPhase!)
                .ChildRules(groupPhase =>
                {
                    groupPhase.RuleFor(x => x.NumberOfCourts)
                        .GreaterThanOrEqualTo((short)1)
                        .WithMessage("Number of courts for group phase must be at least 1.");

                    groupPhase.RuleFor(x => x.NumberOfGroupRounds)
                        .GreaterThanOrEqualTo(1)
                        .WithMessage($"Number of group phase rounds must be between 1 and {ValidationConstants.Tournament.MaxNumberOfGroupPhaseRounds}.")
                        .LessThanOrEqualTo(ValidationConstants.Tournament.MaxNumberOfGroupPhaseRounds)
                        .WithMessage($"Number of group phase rounds must be between 1 and {ValidationConstants.Tournament.MaxNumberOfGroupPhaseRounds}.");
                })
                .When(x => x.GroupPhase is not null);

            RuleFor(x => x.GroupPhase!.Schedule)
                .Null()
                .WithMessage($"Schedule for group phase not allowed when {nameof(ConfigureTournamentEndpointRequest.FirstMatchKickoff)} is null.")
                .When(x => x.GroupPhase is not null && x.FirstMatchKickoff is null);

            RuleFor(x => x.GroupPhase!.Schedule)
                .NotNull()
                .WithMessage($"Schedule for group phase must be specified when {nameof(ConfigureTournamentEndpointRequest.FirstMatchKickoff)} is specified.")
                .ChildRules(schedule =>
                {
                    schedule.RuleFor(x => x!.PlayTime)
                        .GreaterThan(TimeSpan.Zero)
                        .WithMessage("Group phase play time must be greater than zero.");

                    schedule.RuleFor(x => x!.PauseTime)
                        .GreaterThanOrEqualTo(TimeSpan.Zero)
                        .WithMessage("Group phase pause time must be greater than or equal to zero.");
                })
                .When(x => x.GroupPhase is not null && x.FirstMatchKickoff is not null);

            RuleFor(x => x.FinalsPhase!)
                .ChildRules(finalsPhase =>
                {
                    finalsPhase.RuleFor(x => x.NumberOfCourts)
                        .GreaterThanOrEqualTo((short)1)
                        .WithMessage("Number of courts for finals phase must be at least 1.");

                    finalsPhase.RuleFor(x => (FinalsRoundOrder)x.FirstFinalsRound)
                        .IsInEnum()
                        .WithMessage($"First finals round must be one of the following: {string.Join(", ", Enum.GetValues<FinalsRoundOrder>().Select(x => (int)x))}");

                    finalsPhase.RuleFor(x => x.ThirdPlacePlayoff)
                        .Must(x => !x)
                        .When(x => x.FirstFinalsRound == (int)FinalsRoundOrder.FinalOnly)
                        .WithMessage("Third place playoff must be disabled if first finals round is 'final only'.");

                    finalsPhase.RuleForEach(x => x.AdditionalPlayoffs)
                        .Must(x => x.PlayoffPosition >= 3 && x.PlayoffPosition % 2 == 1)
                        .When(x => x.FirstFinalsRound == (int)FinalsRoundOrder.FinalOnly || !x.ThirdPlacePlayoff)
                        .WithMessage("Additional playoff positions must be >= 3 and odd if first finals round is 'final only'.");

                    finalsPhase.RuleForEach(x => x.AdditionalPlayoffs)
                        .Must(x => x.PlayoffPosition >= 5 && x.PlayoffPosition % 2 == 1)
                        .When(x => x.ThirdPlacePlayoff)
                        .WithMessage("Additional playoff positions must be >= 5 and odd if the default third place playoff is enabled.");

                    finalsPhase.RuleForEach(x => x.AdditionalPlayoffs)
                        .ChildRules(playoff =>
                        {
                            playoff.RuleFor(x => x.TeamSelectorA)
                                .NotEmpty()
                                .WithMessage("Additional playoff definition must contain only valid team selectors.")
                                .MaximumLength(ValidationConstants.Match.MaxTeamSelectorLength)
                                .WithMessage("Additional playoff definition must contain only valid team selectors.")
                                .Must(x => AbstractTeamSelectorParser.TryParseAbstractTeamSelector(x, out _))
                                .WithMessage("Additional playoff definition must contain only valid team selectors.");

                            playoff.RuleFor(x => x.TeamSelectorB)
                                .NotEmpty()
                                .WithMessage("Additional playoff definition must contain only valid team selectors.")
                                .MaximumLength(ValidationConstants.Match.MaxTeamSelectorLength)
                                .WithMessage("Additional playoff definition must contain only valid team selectors.")
                                .Must(x => AbstractTeamSelectorParser.TryParseAbstractTeamSelector(x, out _))
                                .WithMessage("Additional playoff definition must contain only valid team selectors.");
                        });
                })
                .When(x => x.FinalsPhase is not null);

            RuleFor(x => x)
                .Must(x =>
                {
                    var highestPlayoffPosition = x.FinalsPhase!.AdditionalPlayoffs.Max(y => y.PlayoffPosition);
                    var requiredTeamCount = highestPlayoffPosition + 1;
                    var totalTeamCount = x.Groups.Sum(y => y.Teams.Length);

                    return totalTeamCount >= requiredTeamCount;
                })
                .When(x => x.FinalsPhase is not null && x.FinalsPhase.AdditionalPlayoffs.Length > 0)
                .WithMessage("The maximum additional playoff position must be 1 less than the number of teams.");

            RuleFor(x => x.FinalsPhase!.Schedule)
                .Null()
                .WithMessage($"Schedule for finals phase not allowed when {nameof(ConfigureTournamentEndpointRequest.FirstMatchKickoff)} is null.")
                .When(x => x.FinalsPhase is not null && x.FirstMatchKickoff is null);

            RuleFor(x => x.FinalsPhase!.Schedule)
                .NotNull()
                .WithMessage($"Schedule for finals phase must be specified when {nameof(ConfigureTournamentEndpointRequest.FirstMatchKickoff)} is specified.")
                .ChildRules(schedule =>
                {
                    schedule.RuleFor(x => x!.PlayTime)
                        .GreaterThan(TimeSpan.Zero)
                        .WithMessage("Finals phase play time must be greater than zero.");

                    schedule.RuleFor(x => x!.PauseTime)
                        .GreaterThanOrEqualTo(TimeSpan.Zero)
                        .WithMessage("Finals phase pause time must be greater than or equal to zero.");
                })
                .When(x => x.FinalsPhase is not null && x.FirstMatchKickoff is not null);

            RuleFor(x => x.PauseBetweenGroupAndFinalsPhase)
                .Null()
                .WithMessage("Pause between group and finals phase must be null if group and finals phase are not both specified.")
                .When(x => x.FinalsPhase is null || x.GroupPhase is null);

            RuleFor(x => x.PauseBetweenGroupAndFinalsPhase)
                .GreaterThanOrEqualTo(TimeSpan.Zero)
                .WithMessage("Pause between group and finals phase must be greater than or equal to zero if specified.")
                .When(x => x.PauseBetweenGroupAndFinalsPhase is not null);
        }
    }
}
