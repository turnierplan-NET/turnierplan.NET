using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Entity;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.RoleAssignments;

internal sealed class GetRoleAssignmentsEndpoint : EndpointBase<IEnumerable<RoleAssignmentDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/role-assignments/{scopeId}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] string scopeId,
        IServiceProvider serviceProvider,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        if (!RbacScopeHelper.TryParseScopeId(scopeId, out var typeName, out var targetId))
        {
            return Results.BadRequest("Invalid scope identifier provided.");
        }

        var task = typeName switch
        {
            "ApiKey" => GetRoleAssignmentsAsync<ApiKey>(serviceProvider, accessValidator, mapper, targetId),
            "Folder" => GetRoleAssignmentsAsync<Folder>(serviceProvider, accessValidator, mapper, targetId),
            "Image" => GetRoleAssignmentsAsync<Image>(serviceProvider, accessValidator, mapper, targetId),
            "Organization" => GetRoleAssignmentsAsync<Organization>(serviceProvider, accessValidator, mapper, targetId),
            "PlanningRealm" => GetRoleAssignmentsAsync<PlanningRealm>(serviceProvider, accessValidator, mapper, targetId),
            "Tournament" => GetRoleAssignmentsAsync<Tournament>(serviceProvider, accessValidator, mapper, targetId),
            "Venue" => GetRoleAssignmentsAsync<Venue>(serviceProvider, accessValidator, mapper, targetId),
            _ => null
        };

        return task is null
            ? Results.BadRequest("Invalid scope identifier provided.")
            : await task;
    }

    private static async Task<IResult> GetRoleAssignmentsAsync<T>(IServiceProvider serviceProvider, IAccessValidator accessValidator, IMapper mapper, PublicId targetId)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        var repository = serviceProvider.GetRequiredService<IRepositoryWithPublicId<T, long>>();
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
