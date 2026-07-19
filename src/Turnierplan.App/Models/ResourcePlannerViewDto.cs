using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record ResourcePlannerViewDto
{
    public required long Id { get; init; }

    public required PublicId PublicId { get; init; }

    public required bool IsActive { get; init; }

    public required bool DisplayAllGroups { get; init; }

    public required long[] ResourceGroupIds { get; init; }
}