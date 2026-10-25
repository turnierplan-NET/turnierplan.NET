using System.Collections.Immutable;
using System.Reflection;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Authentication.Extensions;
using Turnierplan.Server.Infrastructure.Authorization;
using BindingFlags = System.Reflection.BindingFlags;

namespace Turnierplan.Server.Infrastructure.Endpoints;

internal sealed class PermissionEndpointFilter : IEndpointFilter
{
    private static readonly IReadOnlyDictionary<Permission, ImmutableArray<Role>> __permissionRolesCache;

    static PermissionEndpointFilter()
    {
        __permissionRolesCache = Enum.GetValues<Permission>().ToDictionary(permission => permission, permission =>
        {
            var attribute = typeof(Permission).GetField(permission.ToString())?.GetCustomAttribute<GrantedByRolesAttribute>();

            if (attribute is null)
            {
                throw new InvalidOperationException($"The enum member '{typeof(Permission).FullName}.{permission}' does not declare a '{nameof(GrantedByRolesAttribute)}'.");
            }

            return attribute.RequiresAnyOf;
        });
    }

    private readonly List<IPermissionCheck> _checks = [];

    public void AddPermissionCheck(int parameterIndex, string? parameterPropertyName, Type targetType, Permission permission)
    {
        var checkType = typeof(PermissionCheck<>).MakeGenericType(targetType);
        var ctor = checkType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, [typeof(int), typeof(string), typeof(Permission)]);

        if (ctor?.Invoke([parameterIndex, parameterPropertyName, permission]) is not IPermissionCheck check)
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
        var authorizationService = services.GetRequiredService<AuthorizationService>();

        foreach (var permissionCheck in _checks)
        {
            PublicId publicId;

            if (permissionCheck.ParameterPropertyName is null)
            {
                publicId = context.GetArgument<PublicId>(permissionCheck.ParameterIndex);
            }
            else
            {
                var @object = context.Arguments[permissionCheck.ParameterIndex];
                var type = @object?.GetType();
                var property = type?.GetProperty(permissionCheck.ParameterPropertyName);

                if (type is null || property is null)
                {
                    throw new InvalidOperationException($"The parameter '{type?.FullName}' at index {permissionCheck.ParameterIndex} does not contain a property called '{permissionCheck.ParameterPropertyName}'.");
                }

                var value = property.GetValue(@object);

                if (value is not PublicId publicIdValue)
                {
                    throw new InvalidOperationException($"The property '{permissionCheck.ParameterPropertyName}' of type '{type.FullName}' must be a '{nameof(PublicId)}'.");
                }

                publicId = publicIdValue;
            }

            var hasPermission = await permissionCheck.EvaluateAsync(authorizationService, publicId, context.HttpContext.RequestAborted);

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

        string? ParameterPropertyName { get; }

        Task<bool> EvaluateAsync(AuthorizationService authorizationService, PublicId publicId, CancellationToken cancellationToken);
    }

    private sealed class PermissionCheck<T> : IPermissionCheck
        where T : EntityBase<long>, IEntityWithPublicId
    {
        private readonly Permission _permission;

        public PermissionCheck(int parameterIndex, string? parameterPropertyName, Permission permission)
        {
            ParameterIndex = parameterIndex;
            ParameterPropertyName = parameterPropertyName;
            _permission = permission;
        }

        public int ParameterIndex { get; }

        public string? ParameterPropertyName { get; }

        public async Task<bool> EvaluateAsync(AuthorizationService authorizationService, PublicId publicId, CancellationToken cancellationToken)
        {
            return await authorizationService.HasAnyOfRolesAsync<T>(publicId, __permissionRolesCache[_permission], cancellationToken);
        }
    }
}
