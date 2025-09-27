using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record PlanningRealmDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string RbacScopeId { get; init; }

    public required string Name { get; init; }

    public required TournamentClassDto[] TournamentClasses { get; init; }

    public required InvitationLinkDto[] InvitationLinks { get; init; }

    public required LabelDto[] Labels { get; init; }
}
