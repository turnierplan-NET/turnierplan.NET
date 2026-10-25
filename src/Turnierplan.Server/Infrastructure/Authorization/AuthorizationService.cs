using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Domain.Projects;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Database;

namespace Turnierplan.Server.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private static readonly ReadOnlyDictionary<Type, IInheritanceEvaluator>  __inheritanceEvaluators;

    static AuthorizationService()
    {
        __inheritanceEvaluators = new Dictionary<Type, IInheritanceEvaluator>
        {
            // TODO: Detect using reflection via attribute
            { typeof(Project), new InheritanceEvaluator<Project, Organization>(nameof(Project.Organization)) }
        }.AsReadOnly();
    }

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

        await GetAssignedRolesAsyncImpl<T>(result, principal, isTransientCall: false, entity.Id, cancellationToken);

        return result.AsReadOnly();
    }

    public async Task<bool> HasAnyOfRolesAsync<T>(PublicId scopeId, IReadOnlyCollection<Role> anyOfRoles, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var principal = _principalProvider.GetCurrentPrincipal();

        return await HasAnyOfRolesAsyncImpl<T>(principal, scopeId, anyOfRoles, cancellationToken);
    }

    private async Task GetAssignedRolesAsyncImpl<T>(
        List<AssignedRole> result,
        (PrincipalKind Kind, Guid Id) principal,
        bool isTransientCall,
        long entityId,
        CancellationToken cancellationToken)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        if (__inheritanceEvaluators.TryGetValue(typeof(T), out var inheritanceLogic))
        {
            await inheritanceLogic.InvokeGetAssignedRolesAsync(this, result, principal, entityId, cancellationToken);
        }

        var assignments = await _turnierplanContext.GetRoleAssignmentsForType<T>()
            .Where(ra => ra.Scope.Id == entityId && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id)
            .Select(ra => new AssignedRole(IsInherited: isTransientCall, ra.Scope.PublicId, ra.Role))
            .ToArrayAsync(cancellationToken);

        result.AddRange(assignments);
    }

    private async Task<bool> HasAnyOfRolesAsyncImpl<T>(
        (PrincipalKind Kind, Guid Id) principal,
        PublicId entityPublicId,
        IReadOnlyCollection<Role> anyOfRoles,
        CancellationToken cancellationToken)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        if (__inheritanceEvaluators.TryGetValue(typeof(T), out var inheritanceLogic))
        {
            var result = await inheritanceLogic.InvokeHasAnyOfRolesAsync(this, principal, entityPublicId, anyOfRoles, cancellationToken);

            if (result)
            {
                return true;
            }
        }

        return await _turnierplanContext.GetRoleAssignmentsForType<T>()
            .Where(ra => ra.Scope.PublicId == entityPublicId && ra.PrincipalKind == principal.Kind && ra.PrincipalId == principal.Id && anyOfRoles.Contains(ra.Role))
            .AnyAsync(cancellationToken);
    }

    public sealed record AssignedRole(bool IsInherited, PublicId ScopeId, Role Role);

    private interface IInheritanceEvaluator
    {
        Task InvokeGetAssignedRolesAsync(
            AuthorizationService service,
            List<AssignedRole> result,
            (PrincipalKind Kind, Guid Id) principal,
            long entityId,
            CancellationToken cancellationToken);

        Task<bool> InvokeHasAnyOfRolesAsync(
            AuthorizationService service,
            (PrincipalKind Kind, Guid Id) principal,
            PublicId entityPublicId,
            IReadOnlyCollection<Role> anyOfRoles,
            CancellationToken cancellationToken);
    }

    private sealed class InheritanceEvaluator<TEntity, TParent> : IInheritanceEvaluator
        where TEntity : EntityBase<long>, IEntityWithPublicId
        where TParent : EntityBase<long>, IEntityWithPublicId
    {
        private readonly Expression<Func<TEntity, long>> _parentIdPropertyAccessor;
        private readonly Expression<Func<TEntity, PublicId>> _parentPublicIdPropertyAccessor;

        public InheritanceEvaluator(string parentIdPropertyName)
        {
            var entityParameter = Expression.Parameter(typeof(TEntity));
            var parentAccessor = Expression.Property(entityParameter, parentIdPropertyName);

            _parentIdPropertyAccessor = Expression.Lambda<Func<TEntity, long>>(Expression.Property(parentAccessor, nameof(EntityBase<>.Id)), entityParameter);
            _parentPublicIdPropertyAccessor = Expression.Lambda<Func<TEntity, PublicId>>(Expression.Property(parentAccessor, nameof(IEntityWithPublicId.PublicId)), entityParameter);
        }

        public async Task InvokeGetAssignedRolesAsync(
            AuthorizationService service,
            List<AssignedRole> result,
            (PrincipalKind Kind, Guid Id) principal,
            long entityId,
            CancellationToken cancellationToken)
        {
            var parentId = await service._turnierplanContext.GetEntitiesForType<TEntity>()
                .Where(t => t.Id == entityId)
                .Select(_parentIdPropertyAccessor)
                .FirstOrDefaultAsync(cancellationToken);

            await service.GetAssignedRolesAsyncImpl<TParent>(result, principal, isTransientCall: true, parentId, cancellationToken);
        }

        public async Task<bool> InvokeHasAnyOfRolesAsync(
            AuthorizationService service,
            (PrincipalKind Kind, Guid Id) principal,
            PublicId entityPublicId,
            IReadOnlyCollection<Role> anyOfRoles,
            CancellationToken cancellationToken)
        {
            var parentPublicId = await service._turnierplanContext.GetEntitiesForType<TEntity>()
                .Where(t => t.PublicId == entityPublicId)
                .Select(_parentPublicIdPropertyAccessor)
                .FirstOrDefaultAsync(cancellationToken);

            return await service.HasAnyOfRolesAsyncImpl<TParent>(principal, parentPublicId, anyOfRoles, cancellationToken);
        }
    }
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
