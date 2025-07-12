using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Endpoints.RoleAssignments;

internal sealed class DeleteRoleAssignmentEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/role-assignments/{scopeId}/{roleAssignmentId}";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] string scopeId,
        [FromRoute] string roleAssignmentId,
        IApiKeyRepository apiKeyRepository,
        IFolderRepository folderRepository,
        IImageRepository imageRepository,
        IOrganizationRepository organizationRepository,
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!RbacScopeHelper.TryParseScopeId(scopeId, out var typeName, out var targetId))
        {
            return Results.BadRequest("Invalid scope identifier provided.");
        }

        if (!Guid.TryParse(roleAssignmentId, out var roleAssignmentGuid))
        {
            return Results.BadRequest("Invalid role assignment provided.");
        }

        var task = typeName switch
        {
            "ApiKey" => DeleteRoleAssignmentAsync(apiKeyRepository, targetId, accessValidator, roleAssignmentGuid, cancellationToken),
            "Folder" => DeleteRoleAssignmentAsync(folderRepository, targetId, accessValidator, roleAssignmentGuid, cancellationToken),
            "Image" => DeleteRoleAssignmentAsync(imageRepository, targetId, accessValidator, roleAssignmentGuid, cancellationToken),
            "Organization" => DeleteRoleAssignmentAsync(organizationRepository, targetId, accessValidator, roleAssignmentGuid, cancellationToken),
            "Tournament" => DeleteRoleAssignmentAsync(tournamentRepository, targetId, accessValidator, roleAssignmentGuid, cancellationToken),
            "Venue" => DeleteRoleAssignmentAsync(venueRepository, targetId, accessValidator, roleAssignmentGuid, cancellationToken),
            _ => null
        };

        return task is null
            ? Results.BadRequest("Invalid scope identifier provided.")
            : await task.ConfigureAwait(false);
    }

    private static async Task<IResult> DeleteRoleAssignmentAsync<T>(
        IRepositoryWithPublicId<T, long> repository,
        PublicId targetId,
        IAccessValidator accessValidator,
        Guid roleAssignmentId,
        CancellationToken cancellationToken)
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

        var roleAssignment = entity.RoleAssignments.FirstOrDefault(x => x.Id == roleAssignmentId);

        if (roleAssignment is null)
        {
            return Results.NotFound();
        }

        entity.RemoveRoleAssignment(roleAssignment);

        if (entity is Organization organization && !organization.RoleAssignments.Any(x => x.Role is Role.Owner))
        {
            // An organization must always have at least one owner

            return Results.BadRequest("When deleting role assignments from an Organization, at least one owner must always remain.");
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }
}
