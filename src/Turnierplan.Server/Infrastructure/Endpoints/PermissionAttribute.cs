using Turnierplan.Server.Domain;
using Turnierplan.Server.Infrastructure.Authorization;

namespace Turnierplan.Server.Infrastructure.Endpoints;

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class PermissionAttribute : Attribute
{
    public PermissionAttribute(Type targetType, Permission permission, string? propertyName = null)
    {
        if (!targetType.IsAssignableTo(typeof(EntityBase<long>)))
        {
            throw new ArgumentException($"The target type '{targetType.FullName}' is not assignable to '{typeof(EntityBase<long>).FullName}'.");
        }

        TargetType = targetType;
        Permission = permission;
        PropertyName = propertyName;
    }

    public Type TargetType { get; }

    public Permission Permission { get; }

    public string? PropertyName { get; }
}
