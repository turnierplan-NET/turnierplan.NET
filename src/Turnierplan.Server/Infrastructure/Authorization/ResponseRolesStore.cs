using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Infrastructure.Database;

namespace Turnierplan.Server.Infrastructure.Authorization;

internal sealed class ResponseRolesStore // TODO: Find better name
{
    private const string RolesHeaderName = "X-Turnierplan-Roles";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TurnierplanContext _turnierplanContext;

    public ResponseRolesStore(IHttpContextAccessor httpContextAccessor, TurnierplanContext turnierplanContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _turnierplanContext = turnierplanContext;
    }

    public async Task AddRolesToResponseHeaderAsync<T>(T entity, CancellationToken cancellationToken)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        await AddRolesToResponseHeaderAsync([entity], cancellationToken);
    }

    public async Task AddRolesToResponseHeaderAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            throw new InvalidOperationException($"The {nameof(HttpContext)} is currently not available.");
        }

        var dbSet = _turnierplanContext.GetRoleAssignmentsForType<T>();
        var entityIds = entities.Select(x => x.Id).ToArray();

        // TODO: Consider roles that are 'inherited' from the parent - if applicable in the future

        var roleAssignments = await dbSet
            .Where(ra => entityIds.Contains(ra.Scope.Id))
            .Select(ra => new { ScopePublicId = ra.Scope.PublicId, Role = ra.Role })
            .ToArrayAsync(cancellationToken: cancellationToken);

        if (roleAssignments.Length == 0)
        {
            return;
        }

        foreach (var group in roleAssignments.GroupBy(x => x.ScopePublicId))
        {
            var rolesList = string.Join('+', group.Select(x => x.Role.ToString()));
            var rolesHeaderValue = $"{group.Key}={rolesList}";

            httpContext.Response.Headers.Append(RolesHeaderName, rolesHeaderValue);
        }
    }
}
