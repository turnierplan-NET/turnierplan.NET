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

    public async Task<IReadOnlyCollection<AssignedRole>> GetAssignedRolesAsync<T>(T entity, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var result = new List<AssignedRole>();
        var principal = _principalProvider.GetCurrentPrincipal();

        if (typeof(T).IsAssignableTo(typeof(IInheritsRolesFromOrganization)))
        {
            var organizationId = await _turnierplanContext.GetEntitiesForType<T>()
                .Where(t => t == entity)
                .Select(t => ((IInheritsRolesFromOrganization)t).Organization.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var inheritedRoles = await _turnierplanContext.OrganizationRoleAssignments
                .Where(ra => ra.Scope.Id == organizationId && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id)
                .Select(ra => new AssignedRole(IsInherited: true, ra.Scope.PublicId, ra.Role))
                .ToArrayAsync(cancellationToken);

            result.AddRange(inheritedRoles);
        }

        var directAssignments = await _turnierplanContext.GetRoleAssignmentsForType<T>()
            .Where(ra => ra.Scope == entity && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id)
            .Select(ra => new AssignedRole(IsInherited: false, ra.Scope.PublicId, ra.Role))
            .ToArrayAsync(cancellationToken);

        result.AddRange(directAssignments);

        return result.AsReadOnly();
    }

    public async Task<bool> HasAnyOfRolesAsync<T>(PublicId scopeId, IEnumerable<Role> anyOfRoles, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var principal = _principalProvider.GetCurrentPrincipal();

        if (typeof(T).IsAssignableTo(typeof(IInheritsRolesFromOrganization)))
        {
            var organizationId = await _turnierplanContext.GetEntitiesForType<T>()
                .Where(t => t.PublicId == scopeId)
                .Select(t => ((IInheritsRolesFromOrganization)t).Organization.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var hasInheritedRoleAssignment = await _turnierplanContext.OrganizationRoleAssignments
                .Where(ra => ra.Scope.Id == organizationId && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id && anyOfRoles.Contains(ra.Role))
                .AnyAsync(cancellationToken);

            if (hasInheritedRoleAssignment)
            {
                return true;
            }
        }

        return await _turnierplanContext.GetRoleAssignmentsForType<T>()
            .Where(ra => ra.Scope.PublicId == scopeId && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id && anyOfRoles.Contains(ra.Role))
            .AnyAsync(cancellationToken);
    }

    public sealed record AssignedRole(bool IsInherited, PublicId ScopeId, Role Role);
}

file static class TurnierplanContextExtensions
{
    private static readonly Type __contextType = typeof(TurnierplanContext);

    public static DbSet<T> GetEntitiesForType<T>(this TurnierplanContext context)
        where T : EntityBase<long>
    {
        var prop = __contextType.GetProperty($"{typeof(T).Name}s");

        return prop?.GetValue(context) as DbSet<T> ?? throw new InvalidOperationException($"Could not find or access db set for type '{typeof(T).FullName}'.");
    }

    public static DbSet<RoleAssignment<T>> GetRoleAssignmentsForType<T>(this TurnierplanContext context)
        where T : EntityBase<long>
    {
        var prop = __contextType.GetProperty($"{typeof(T).Name}RoleAssignments");

        return prop?.GetValue(context) as DbSet<RoleAssignment<T>> ?? throw new InvalidOperationException($"Could not find or access role assignments set for type '{typeof(T).FullName}'.");
    }
}
