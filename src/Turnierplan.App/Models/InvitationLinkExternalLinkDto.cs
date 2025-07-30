namespace Turnierplan.App.Models;

public sealed record InvitationLinkExternalLinkDto
{
    public required string Name { get; init; }

    public required string Url { get; init; }
}
