using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.SeedWork;
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
            return Results.BadRequest("Invalid scope identifier provided.");
        }

        var task = typeName switch
        {
            "ApiKey" => GetRoleAssignmentsResultAsync(apiKeyRepository, targetId, accessValidator, mapper),
            "Folder" => GetRoleAssignmentsResultAsync(folderRepository, targetId, accessValidator, mapper),
            "Image" => GetRoleAssignmentsResultAsync(imageRepository, targetId, accessValidator, mapper),
            "Organization" => GetRoleAssignmentsResultAsync(organizationRepository, targetId, accessValidator, mapper),
            "Tournament" => GetRoleAssignmentsResultAsync(tournamentRepository, targetId, accessValidator, mapper),
            "Venue" => GetRoleAssignmentsResultAsync(venueRepository, targetId, accessValidator, mapper),
            _ => null
        };

        return task is null 
            ? Results.BadRequest("Invalid scope identifier provided.")
            : await task.ConfigureAwait(false);
    }

    private static async Task<IResult> GetRoleAssignmentsResultAsync<T>(IRepositoryWithPublicId<T, long> repository, PublicId targetId, IAccessValidator accessValidator, IMapper mapper)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        var entity = await repository.GetByPublicIdAsync(targetId).ConfigureAwait(false);

        if (entity is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(entity, Actions.ReadOrWriteRoleAssignments))
        {
            return Results.Forbid();
        }

        var result = new List<RoleAssignmentDto>();

        result.AddRange(mapper.MapCollection<RoleAssignmentDto>(entity.RoleAssignments));

        if (entity is IEntityWithOrganization entityWithOrganization)
        {
            result.AddRange(mapper.MapCollection<RoleAssignmentDto>(entityWithOrganization.Organization.RoleAssignments)
                .Select(r => r with { IsInherited = true }));
        }

        // Special case in generic method is not the cleanest...
        if (entity is Tournament { Folder: not null } tournament)
        {
            result.AddRange(mapper.MapCollection<RoleAssignmentDto>(tournament.Folder.RoleAssignments)
                .Select(r => r with { IsInherited = true }));
        }

        return Results.Ok(result);
    }
}
