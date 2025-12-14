using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class DeleteTournamentEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/tournaments/{id}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        ITournamentRepository tournamentRepository,
        IFolderRepository folderRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var tournament = await tournamentRepository.GetByPublicIdAsync(id, ITournamentRepository.Includes.FolderWithTournaments);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        if (tournament.Folder is not null
            && tournament.Folder.Tournaments.Count == 1
            && tournament.Folder.Tournaments.Single() == tournament)
        {
            folderRepository.Remove(tournament.Folder);
        }

        tournamentRepository.Remove(tournament);

        await tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
