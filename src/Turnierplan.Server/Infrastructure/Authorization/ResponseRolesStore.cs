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
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            throw new InvalidOperationException($"The {nameof(HttpContext)} is currently not available.");
        }

        var roleAssignments = await _authorizationService.GetAssignedRolesAsync(entity, cancellationToken);

        if (roleAssignments.Count == 0)
        {
            return;
        }

        var rolesList = string.Join('+', roleAssignments.Select(x => x.Role.ToString()));
        var rolesHeaderValue = $"{entity.PublicId}={rolesList}";

        httpContext.Response.Headers.Append(RolesHeaderName, rolesHeaderValue);
    }
}
