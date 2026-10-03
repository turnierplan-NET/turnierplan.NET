using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Infrastructure.Authorization;

internal static class ScopeIdentifier
{
    public static string Create<T>(T entity)
        where T : IEntityWithPublicId
    {
        return $"{typeof(T).Name.ToLower()}:{entity.PublicId.ToString()}";
    }
}
