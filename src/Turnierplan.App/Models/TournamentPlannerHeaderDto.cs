using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record TournamentPlannerHeaderDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string Name { get; init; }
}
