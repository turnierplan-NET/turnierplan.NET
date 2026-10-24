using Turnierplan.Server.Domain;
using Turnierplan.Server.Infrastructure.Authorization;

namespace Turnierplan.Server.Infrastructure.Endpoints;

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class PermissionAttribute : Attribute
{
    public PermissionAttribute(Type targetType, Actions action)
    {
        if (!targetType.IsAssignableTo(typeof(EntityBase<long>)))
        {
            throw new ArgumentException($"The target type '{targetType.FullName}' is not assignable to '{typeof(EntityBase<long>).FullName}'.");
        }

        TargetType = targetType;
        Action = action;
    }

    public Type TargetType { get; }

    public Actions Action { get; }
}
