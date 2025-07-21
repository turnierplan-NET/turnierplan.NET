using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record InvitationLinkDto
{
    public required long Id { get; init; }

    public required PublicId PublicId { get; init; }

    public required string Name { get; init; }

    public required string? Title { get; init; }

    public required string? Description { get; init; }

    public required string ColorCode { get; init; }

    public required DateTime? ValidUntil { get; init; }

    public required string? ContactPerson { get; init; }

    public required string? ContactEmail { get; init; }

    public required string? ContactTelephone { get; init; }

    public required ImageDto? PrimaryLogo { get; init; }

    public required ImageDto? SecondaryLogo { get; init; }

    public required InvitationLinkEntryDto[] Entries { get; init; }

    public required int NumberOfApplications { get; init; }
}
