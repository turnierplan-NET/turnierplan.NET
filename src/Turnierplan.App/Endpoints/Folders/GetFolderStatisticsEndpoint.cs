using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Folder;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Folders;

internal sealed class GetFolderStatisticsEndpoint : EndpointBase<FolderStatisticsDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/folders/{id}/statistics";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IFolderRepository folderRepository,
        IAccessValidator accessValidator)
    {
        var folder = await folderRepository.GetByPublicIdAsync(id, IFolderRepository.Includes.TournamentsWithGameRelevant);

        if (folder is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(folder, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        var numberOfTournaments = folder.Tournaments.Count;
        var numberOfMatches = folder.Tournaments.Sum(x => x.Matches.Count);
        var numberOfFinishedMatches = folder.Tournaments.Sum(x => x.Matches.Count(y => y.IsFinished));
        var numberOfTeams = folder.Tournaments.Sum(x => x.Teams.Count);
        var numberOfGroups = folder.Tournaments.Sum(x => x.Groups.Count);

        // If all matches in a tournament are reported as "0:0", this tournament should not count towards the "average goals per match"
        // statistic and the outcome distribution. Because of this, we create a list of tournaments which are excluded from these statistics.
        var goalDistributionExcludedTournaments = folder.Tournaments.Where(x => x.Matches.Any(y => y.IsFinished) && x.Matches.Where(y => y.IsFinished).All(y => y is { ScoreA: 0, ScoreB: 0 })).ToHashSet();
        var numberOfFinishedMatchesFromTournamentsWithAtLeastOneGoal = folder.Tournaments.Except(goalDistributionExcludedTournaments).Sum(x => x.Matches.Count(y => y.IsFinished));

        var numberOfGoals = 0;
        var maximumGoalsPerMatch = 0;
        var outcomes = new List<FolderStatisticsOutcomeDto>();

        foreach (var tournament in folder.Tournaments)
        {
            foreach (var match in tournament.Matches.Where(x => x is { IsFinished: true, OutcomeType: not MatchOutcomeType.SpecialScoring }))
            {
                var goalsA = match.ScoreA ?? 0;
                var goalsB = match.ScoreB ?? 0;

                var goals = goalsA + goalsB;
                numberOfGoals += goals;

                if (goals > maximumGoalsPerMatch)
                {
                    maximumGoalsPerMatch = goals;
                }

                if (goalDistributionExcludedTournaments.Contains(tournament))
                {
                    continue;
                }

                if (goalsB > goalsA)
                {
                    // Swap such that goalsA is always the larger number
                    (goalsA, goalsB) = (goalsB, goalsA);
                }

                outcomes.Add(new FolderStatisticsOutcomeDto(goalsA, goalsB));
            }
        }

        var mostSignificantOutcome = outcomes
            .OrderByDescending(x => x.Difference)
            .ThenByDescending(x => x.ScoreA)
            .FirstOrDefault();

        var outcomeDistribution = outcomes
            .GroupBy(x => x)
            .Select(x => new FolderStatisticsOutcomeDistributionDto(x.Key, x.Count()))
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.Outcome.Difference)
            .ThenByDescending(x => x.Outcome.ScoreA)
            .ToList();

        var decidingMatches = folder.Tournaments.SelectMany(x => x.Matches).Where(x => x.IsDecidingMatch).ToList();

        var pageViewEntries = folder.Tournaments
            .Select(x => new FolderStatisticsPageViewsDto(x))
            .OrderByDescending(x => x.PublicPageViews)
            .ToList();
        var numberOfPageViews = pageViewEntries.Sum(x => x.PublicPageViews);

        var statisticsDto = new FolderStatisticsDto
        {
            FolderId = folder.PublicId,
            OrganizationId = folder.Organization.PublicId,
            FolderName = folder.Name,
            NumberOfTournaments = numberOfTournaments,
            NumberOfMatches = numberOfMatches,
            NumberOfFinishedMatches = numberOfFinishedMatches,
            PercentageOfFinishedMatches = DivideSafe(numberOfFinishedMatches, numberOfMatches) * 100,
            NumberOfTeams = numberOfTeams,
            NumberOfGroups = numberOfGroups,
            NumberOfGoals = numberOfGoals,
            AverageGoalsPerMatch = DivideSafe(numberOfGoals, numberOfFinishedMatchesFromTournamentsWithAtLeastOneGoal),
            MostGoalsPerMatch = maximumGoalsPerMatch,
            MostSignificantOutcome = mostSignificantOutcome ?? new FolderStatisticsOutcomeDto(0, 0),
            NumberOfDecidingMatches = decidingMatches.Count,
            NumberOfPenaltyShootouts = decidingMatches.Count(x => x.OutcomeType is MatchOutcomeType.AfterPenalties),
            NumberOfOverTimes = decidingMatches.Count(x => x.OutcomeType is MatchOutcomeType.AfterOvertime),
            TotalPublicPageViews = numberOfPageViews,
            AveragePublicPageViewsPerTournament = DivideSafe(numberOfPageViews, numberOfTournaments),
            TournamentPageViews = pageViewEntries,
            OutcomeDistribution = outcomeDistribution,
            GoalDistributionExcludedTournaments = goalDistributionExcludedTournaments.Select(x => new FolderStatisticsExcludedTournamentDto(x)).ToList()
        };

        return Results.Ok(statisticsDto);
    }

    private static float DivideSafe(int a, int b)
    {
        return b == 0 ? 0 : (float)a / b;
    }
}
