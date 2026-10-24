using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Database;

namespace Turnierplan.Server.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly TurnierplanContext _turnierplanContext;
    private readonly PrincipalProvider _principalProvider;

    public AuthorizationService(TurnierplanContext turnierplanContext, PrincipalProvider principalProvider)
    {
        _turnierplanContext = turnierplanContext;
        _principalProvider = principalProvider;
    }

    public async Task<AssignedRole[]> GetAssignedRolesAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var dbSet = _turnierplanContext.GetRoleAssignmentsForType<T>();
        var principal = _principalProvider.GetCurrentPrincipal();

        var entityIds = entities.Select(x => x.Id).ToArray().AsEnumerable();

        // TODO: Consider roles that are 'inherited' from the parent - if applicable in the future

        return await dbSet
            .Where(ra => ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id && entityIds.Contains(ra.Scope.Id))
            .Select(ra => new AssignedRole(ra.Scope.PublicId, ra.Role))
            .ToArrayAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> HasAnyOfRolesAsync<T>(PublicId scopeId, IEnumerable<Role> anyOfRoles, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var dbSet = _turnierplanContext.GetRoleAssignmentsForType<T>();
        var principal = _principalProvider.GetCurrentPrincipal();

        // TODO: Consider roles that are 'inherited' from the parent - if applicable in the future

        var roleAssignment = await dbSet
            .Where(ra => ra.Scope.PublicId == scopeId && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id && anyOfRoles.Contains(ra.Role))
            .FirstOrDefaultAsync(cancellationToken);

        return roleAssignment is not null;
    }

    public sealed record AssignedRole(PublicId ScopeId, Role Role);
}
