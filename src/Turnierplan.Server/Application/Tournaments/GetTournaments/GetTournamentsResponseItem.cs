using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Tournaments;

namespace Turnierplan.Server.Application.Tournaments.GetTournaments;

internal sealed class GetTournamentsResponseItem
{
    public required PublicId Id { get; init; }

    public required string Scope { get; init; }

    public required string Name { get; init; }

    public required Visibility Visibility { get; init; }
}
