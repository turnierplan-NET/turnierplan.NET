using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class GetTournamentsEndpoint : EndpointBase<IEnumerable<TournamentHeaderDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournaments";

    protected override Delegate Handler => Handle;

    protected override bool? AllowApiKeyAccess => true;

    private static async Task<IResult> Handle(
        [FromQuery] PublicId? organizationId,
        [FromQuery] PublicId? folderId,
        IOrganizationRepository organizationRepository,
        IFolderRepository folderRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        if (organizationId.HasValue == folderId.HasValue)
        {
            return Results.BadRequest("Specify either organizationId or folderId");
        }

        IReadOnlyCollection<Tournament>? tournaments;

        if (organizationId.HasValue)
        {
            var organization = await organizationRepository.GetByPublicIdAsync(organizationId.Value, IOrganizationRepository.Include.Tournaments).ConfigureAwait(false);

            if (organization is null)
            {
                return Results.NotFound();
            }

            if (!accessValidator.IsActionAllowed(organization, Actions.GenericRead))
            {
                return Results.Forbid();
            }

            tournaments = organization.Tournaments;
        }
        else
        {
            var folder = await folderRepository.GetByPublicIdAsync(folderId.Value, IFolderRepository.Include.Tournaments).ConfigureAwait(false);

            if (folder is null)
            {
                return Results.NotFound();
            }

            if (!accessValidator.IsActionAllowed(folder, Actions.GenericRead))
            {
                return Results.Forbid();
            }

            tournaments = folder.Tournaments;
        }

        foreach (var folder in tournaments.Where(x => x.Folder is not null).Select(x => x.Folder!).Distinct())
        {
            accessValidator.AddRolesToResponseHeader(folder);
        }

        return Results.Ok(mapper.MapCollection<TournamentHeaderDto>(tournaments));
    }
}
