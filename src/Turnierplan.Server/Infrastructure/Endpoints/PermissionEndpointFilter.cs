using System.Collections.Immutable;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Authentication;
using Turnierplan.Server.Infrastructure.Authentication.Extensions;
using Turnierplan.Server.Infrastructure.Database;
using BindingFlags = System.Reflection.BindingFlags;

namespace Turnierplan.Server.Infrastructure.Endpoints;

internal sealed class PermissionEndpointFilter : IEndpointFilter
{
    private static readonly IReadOnlyDictionary<Actions, ImmutableArray<Role>> __actionRequiresRolesCache;

    static PermissionEndpointFilter()
    {
        __actionRequiresRolesCache = Enum.GetValues<Actions>().ToDictionary(action => action, action =>
        {
            var attribute = typeof(Actions).GetField(action.ToString())?.GetCustomAttribute<ActionRolesAttribute>();

            if (attribute is null)
            {
                throw new InvalidOperationException($"The enum member '{typeof(Actions).FullName}.{action}' does not declare a '{nameof(ActionRolesAttribute)}'.");
            }

            return attribute.RequiresAnyOf;
        });
    }

    private readonly List<IPermissionCheck> _checks = [];

    public void AddPermissionCheck(int parameterIndex, Type targetType, Actions action)
    {
        var checkType = typeof(PermissionCheck<>).MakeGenericType(targetType);
        var ctor = checkType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, [typeof(int), typeof(Actions)]);

        if (ctor?.Invoke([parameterIndex, action]) is not IPermissionCheck check)
        {
            throw new InvalidOperationException($"Failed to instantiate permission check. Is '{targetType.FullName}' a valid permission check target?");
        }

        _checks.Add(check);
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_checks.Count == 0)
        {
            // This case should never happen. Therefore, we don't throw PermissionException
            // since that exception type would be caught and converted into a 403 Forbidden.
            throw new InvalidOperationException("Required permission checks do not contain any entries.");
        }

        if (context.HttpContext.User.HasIsAdministratorClaim())
        {
            return await next(context);
        }

        var services = context.HttpContext.RequestServices;
        var principal = services.GetRequiredService<PrincipalProvider>().GetCurrentPrincipal();
        var turnierplanContext = services.GetRequiredService<TurnierplanContext>();

        foreach (var permissionCheck in _checks)
        {
            var publicId = context.GetArgument<PublicId>(permissionCheck.ParameterIndex);
            var hasPermission = await permissionCheck.EvaluateAsync(turnierplanContext, publicId, principal.Kind, principal.Id);

            if (!hasPermission)
            {
                throw new PermissionException();
            }
        }

        return await next(context);
    }

    private interface IPermissionCheck
    {
        int ParameterIndex { get; }

        Task<bool> EvaluateAsync(TurnierplanContext turnierplanContext, PublicId publicId, PrincipalKind principalKind, Guid principalId);
    }

    private sealed class PermissionCheck<T> : IPermissionCheck
        where T : EntityBase<long>, IEntityWithPublicId
    {
        private readonly Actions _action;

        public PermissionCheck(int parameterIndex, Actions action)
        {
            ParameterIndex = parameterIndex;
            _action = action;
        }

        public int ParameterIndex { get; }

        public async Task<bool> EvaluateAsync(TurnierplanContext turnierplanContext, PublicId publicId, PrincipalKind principalKind, Guid principalId)
        {
            var dbSet = turnierplanContext.GetRoleAssignmentsForType<T>();
            var anyOfRoles = __actionRequiresRolesCache[_action].AsEnumerable();

            // TODO: Consider roles that are 'inherited' from the parent - if applicable in the future

            var roleAssignment = await dbSet
                .Where(ra => ra.Scope.PublicId == publicId && ra.PrincipalKind == principalKind && ra.PrincipalId == principalId && anyOfRoles.Contains(ra.Role))
                .FirstOrDefaultAsync();

            return roleAssignment is not null;
        }
    }
}
