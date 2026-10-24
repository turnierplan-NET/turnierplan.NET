using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Infrastructure.Authorization;

internal sealed class ResponseRolesStore // TODO: Find better name
{
    private const string RolesHeaderName = "X-Turnierplan-Roles";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthorizationService _authorizationService;

    public ResponseRolesStore(IHttpContextAccessor httpContextAccessor, AuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
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

        var roleAssignments = await _authorizationService.Blubb(entities, cancellationToken);

        if (roleAssignments.Length == 0)
        {
            return;
        }

        foreach (var group in roleAssignments.GroupBy(x => x.ScopeId))
        {
            var rolesList = string.Join('+', group.Select(x => x.Role.ToString()));
            var rolesHeaderValue = $"{group.Key}={rolesList}";

            httpContext.Response.Headers.Append(RolesHeaderName, rolesHeaderValue);
        }
    }
}
