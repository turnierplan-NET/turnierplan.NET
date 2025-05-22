using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record VenueDto
{
    public required PublicId Id { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string[] AddressDetails { get; init; }

    public required string[] ExternalLinks { get; init; }
}
