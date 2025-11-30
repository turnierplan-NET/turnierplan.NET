using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Folders;

internal sealed class GetFolderTimetableEndpoint : EndpointBase<FolderTimetableDto>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/folders/{id}/timetable";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        IFolderRepository folderRepository,
        IAccessValidator accessValidator)
    {
        var folder = await folderRepository.GetByPublicIdAsync(id, IFolderRepository.Includes.TournamentsWithMatchesAndGroups);

        if (folder is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(folder, Actions.GenericRead))
        {
            return Results.Forbid();
        }

        var timetableDto = new FolderTimetableDto
        {
            FolderId = folder.PublicId,
            OrganizationId = folder.Organization.PublicId,
            FolderName = folder.Name,
            Tournaments = folder.Tournaments.Select(tournament => new FolderTimetableDto.FolderTimetableTournamentEntry
            {
                Id = tournament.PublicId,
                Name = tournament.Name,
                StartDate = tournament.StartTimestamp,
                EndDate = tournament.EndTimestamp
            }).ToArray()
        };

        return Results.Ok(timetableDto);
    }
}
