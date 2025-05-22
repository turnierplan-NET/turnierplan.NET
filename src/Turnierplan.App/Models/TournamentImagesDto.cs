using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record TournamentImagesDto
{
    public required PublicId TournamentId { get; init; }

    public required bool HasOrganizerLogo { get; init; }

    public required bool HasSponsorLogo { get; init; }

    public required bool HasSponsorBanner { get; init; }

    public required ImageDto? OrganizerLogo { get; init; }

    public required ImageDto? SponsorLogo { get; init; }

    public required ImageDto? SponsorBanner { get; init; }
}
