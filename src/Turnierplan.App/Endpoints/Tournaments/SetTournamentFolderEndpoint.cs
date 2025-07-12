using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Folder;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class SetTournamentFolderEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{id}/folder";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetTournamentFolderEndpointRequest request,
        ITournamentRepository tournamentRepository,
        IFolderRepository folderRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await tournamentRepository.GetByPublicIdAsync(id, ITournamentRepository.Include.FolderWithTournaments).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var previousFolder = tournament.Folder;

        if (request.FolderId is null)
        {
            if (request.FolderName is null)
            {
                // Reset tournament to "default" folder
                tournament.SetFolder(null);
            }
            else
            {
                // Assign tournament to a newly created folder

                var folder = new Folder(tournament.Organization, request.FolderName);

                tournament.SetFolder(folder);

                accessValidator.AddRolesToResponseHeader(folder);

                await folderRepository.CreateAsync(folder).ConfigureAwait(false);
            }
        }
        else
        {
            // Assign tournament to an existing folder

            var folder = await folderRepository.GetByPublicIdAsync(request.FolderId.Value).ConfigureAwait(false);

            if (folder is null)
            {
                return Results.NotFound();
            }

            if (!accessValidator.IsActionAllowed(folder, Actions.GenericWrite))
            {
                return Results.Forbid();
            }

            if (tournament.Organization != folder.Organization)
            {
                return Results.BadRequest("Existing folder must belong to the same organization as the tournament.");
            }

            tournament.SetFolder(folder);
        }

        if (previousFolder is not null && previousFolder.Tournaments.Count == 0)
        {
            folderRepository.Remove(previousFolder);
        }

        await tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    /// <remarks>
    /// <see cref="FolderId"/> and <see cref="FolderName"/> are both optional but mutually exclusive.
    /// </remarks>
    public sealed record SetTournamentFolderEndpointRequest
    {
        public PublicId? FolderId { get; init; }

        public string? FolderName { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetTournamentFolderEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x)
                .Must(x => x.FolderId is null ^ x.FolderName is null)
                .WithMessage($"Only one of {nameof(SetTournamentFolderEndpointRequest.FolderId)} and {nameof(SetTournamentFolderEndpointRequest.FolderName)} are allowed.")
                .When(x => x.FolderId is not null || x.FolderName is not null);

            RuleFor(x => x.FolderName)
                .NotEmpty()
                .When(x => x.FolderName is not null);
        }
    }
}
