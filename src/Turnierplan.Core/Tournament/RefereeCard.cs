namespace Turnierplan.Core.Tournament;

public sealed record RefereeCard
{
    public required Match Match { get; init; }

    public required Team? RefereeTeam { get; init; }
}
