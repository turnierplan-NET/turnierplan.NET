using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Projects;
using Turnierplan.Server.Infrastructure.Authorization;
using Turnierplan.Server.Infrastructure.Database;
using Turnierplan.Server.Infrastructure.Endpoints;

namespace Turnierplan.Server.Application.Tournaments.GetTournaments;

internal sealed class GetTournamentsEndpoint : EndpointBase<GetTournamentsResponse>
{
    public GetTournamentsEndpoint()
        : base(HttpMethod.Get, "/tournaments", PolicyNames.Default)
    {
    }

    public static async Task<IResult> HandleAsync(
        [FromQuery, Permission(typeof(Project), Permission.GenericRead)] PublicId projectId,
        TurnierplanContext context,
        ResponseRolesStore responseRolesStore,
        CancellationToken cancellationToken)
    {
        var tournaments = await context.Tournaments
            .Where(x => x.Project.PublicId == projectId)
            .ToArrayAsync(cancellationToken);

        // TODO: Implement pagination (at some point later)
        // TODO: Check if we need the roles in the response because evaluation can be quite expensive

        foreach (var tournament in tournaments)
        {
            await responseRolesStore.AddRolesToResponseHeaderAsync(tournament, cancellationToken);
        }

        return Results.Ok(new GetTournamentsResponse
        {
            Tournaments =
            [
                .. tournaments.Select(tournament => new GetTournamentsResponseItem
                {
                    Id = tournament.PublicId,
                    Scope = ScopeIdentifier.Create(tournament),
                    Name = tournament.Name,
                    Visibility = tournament.Visibility
                })
            ]
        });
    }
}
