using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Database;

namespace Turnierplan.Server.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly TurnierplanContext _turnierplanContext;

    public AuthorizationService(TurnierplanContext turnierplanContext)
    {
        _turnierplanContext = turnierplanContext;
    }

    public async Task<Blubb1[]> Blubb<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var dbSet = _turnierplanContext.GetRoleAssignmentsForType<T>();
        var entityIds = entities.Select(x => x.Id).ToArray().AsEnumerable();

        // TODO: Consider roles that are 'inherited' from the parent - if applicable in the future
        // TODO: Only consider role assignment for the authenticated user :D

        return await dbSet
            .Where(ra => entityIds.Contains(ra.Scope.Id))
            .Select(ra => new Blubb1(ra.Scope.PublicId, ra.Role))
            .ToArrayAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> Blubb3<T>(PublicId scopeId, PrincipalKind principalKind, Guid principalId, IEnumerable<Role> anyOfRoles, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var dbSet = _turnierplanContext.GetRoleAssignmentsForType<T>();

        // TODO: Consider roles that are 'inherited' from the parent - if applicable in the future

        var roleAssignment = await dbSet
            .Where(ra => ra.Scope.PublicId == scopeId && ra.PrincipalKind == principalKind && ra.PrincipalId == principalId && anyOfRoles.Contains(ra.Role))
            .FirstOrDefaultAsync(cancellationToken);

        return roleAssignment is not null;
    }



    public sealed record Blubb1(PublicId ScopeId, Role Role);
}
