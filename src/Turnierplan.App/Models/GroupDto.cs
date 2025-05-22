namespace Turnierplan.App.Models;

public sealed record GroupDto
{
    public required int Id { get; init; }

    public required char AlphabeticalId { get; init; }

    public required string DisplayName { get; init; }

    public required bool HasCustomDisplayName { get; init; }

    public required GroupParticipantDto[] Participants { get; init; }
}
