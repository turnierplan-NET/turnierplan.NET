using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Entity;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.RoleAssignments;

internal sealed class GetRoleAssignmentsEndpoint : EndpointBase<IEnumerable<RoleAssignmentDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/role-assignments/{scopeId}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] string scopeId,
        IApiKeyRepository apiKeyRepository,
        IFolderRepository folderRepository,
        IImageRepository imageRepository,
        IOrganizationRepository organizationRepository,
        IPlanningRealmRepository planningRealmRepository,
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        if (!RbacScopeHelper.TryParseScopeId(scopeId, out var typeName, out var targetId))
        {
            return Results.BadRequest("Invalid scope identifier provided.");
        }

        var task = typeName switch
        {
            "ApiKey" => GetRoleAssignmentsAsync(apiKeyRepository, targetId, accessValidator, mapper),
            "Folder" => GetRoleAssignmentsAsync(folderRepository, targetId, accessValidator, mapper),
            "Image" => GetRoleAssignmentsAsync(imageRepository, targetId, accessValidator, mapper),
            "Organization" => GetRoleAssignmentsAsync(organizationRepository, targetId, accessValidator, mapper),
            "PlanningRealm" => GetRoleAssignmentsAsync(planningRealmRepository, targetId, accessValidator, mapper),
            "Tournament" => GetRoleAssignmentsAsync(tournamentRepository, targetId, accessValidator, mapper),
            "Venue" => GetRoleAssignmentsAsync(venueRepository, targetId, accessValidator, mapper),
            _ => null
        };

        return task is null
            ? Results.BadRequest("Invalid scope identifier provided.")
            : await task;
    }

    private static async Task<IResult> GetRoleAssignmentsAsync<T>(IRepositoryWithPublicId<T, long> repository, PublicId targetId, IAccessValidator accessValidator, IMapper mapper)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        var entity = await repository.GetByPublicIdAsync(targetId);

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

        if (entity is Tournament { Folder: not null } tournament)
        {
            result.AddRange(mapper.MapCollection<RoleAssignmentDto>(tournament.Folder.RoleAssignments)
                .Select(r => r with { IsInherited = true }));
        }

        return Results.Ok(result);
    }
}
