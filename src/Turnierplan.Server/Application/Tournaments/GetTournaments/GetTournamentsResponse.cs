namespace Turnierplan.Server.Application.Tournaments.GetTournaments;

internal sealed class GetTournamentsResponse
{
    public required GetTournamentsResponseItem[] Tournaments { get; init; }
}