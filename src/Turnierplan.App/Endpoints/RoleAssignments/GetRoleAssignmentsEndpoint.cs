using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Endpoints.RoleAssignments;

internal sealed class GetRoleAssignmentsEndpoint : EndpointBase<IEnumerable<RoleAssignmentDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/role-assignments";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromQuery] string scope,
        IApiKeyRepository apiKeyRepository,
        IFolderRepository folderRepository,
        IImageRepository imageRepository,
        IOrganizationRepository organizationRepository,
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        if (!RbacScopeHelper.TryParseScopeId(scope, out var typeName, out var targetId))
        {
            return Results.BadRequest("Invalid scope ID provided.");
        }

        if (typeName.Equals("Tournament"))
        {
            var tournament = await tournamentRepository.GetByPublicIdAsync(targetId).ConfigureAwait(false);

            if (tournament is null)
            {
                return Results.NotFound();
            }

            if (!accessValidator.IsActionAllowed(tournament, Actions.ReadOrWriteRoleAssignments))
            {
                return Results.Forbid();
            }

            var result = new List<RoleAssignmentDto>();

            result.AddRange(mapper.MapCollection<RoleAssignmentDto>(tournament.RoleAssignments));

            result.AddRange(mapper.MapCollection<RoleAssignmentDto>(tournament.Organization.RoleAssignments)
                .Select(r => r with { IsInherited = true }));

            if (tournament.Folder is not null)
            {
                result.AddRange(mapper.MapCollection<RoleAssignmentDto>(tournament.Folder.RoleAssignments)
                    .Select(r => r with { IsInherited = true }));
            }

            return Results.Ok(result);
        }

        return Results.StatusCode(501);
    }
}
