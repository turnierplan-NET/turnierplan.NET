using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Models;

public sealed record TournamentDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public PublicId? FolderId { get; init; }

    public PublicId? VenueId { get; init; }

    public required string Name { get; init; }

    public required string OrganizationName { get; init; }

    public string? FolderName { get; init; }

    public string? VenueName { get; init; }

    public required Visibility Visibility { get; init; }

    public required int PublicPageViews { get; init; }

    public required TeamDto[] Teams { get; init; }

    public required GroupDto[] Groups { get; init; }

    public required MatchDto[] Matches { get; init; }

    public required RankingDto[] Rankings { get; init; }

    public required MatchPlanConfigurationDto MatchPlanConfiguration { get; init; }

    public required ComputationConfigurationDto ComputationConfiguration { get; init; }

    public required PresentationConfigurationDto PresentationConfiguration { get; init; }
}
