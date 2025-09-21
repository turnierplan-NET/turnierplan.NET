using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record TournamentImagesDto
{
    public required PublicId TournamentId { get; init; }

    public required bool HasPrimaryLogo { get; init; }

    public required bool HasSecondaryLogo { get; init; }

    public required bool HasBannerImage { get; init; }

    public required ImageDto? PrimaryLogo { get; init; }

    public required ImageDto? SecondaryLogo { get; init; }

    public required ImageDto? BannerImage { get; init; }
}
