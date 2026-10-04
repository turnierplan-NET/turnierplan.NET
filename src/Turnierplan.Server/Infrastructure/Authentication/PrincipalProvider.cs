using Turnierplan.Server.Domain.RoleAssignments;

namespace Turnierplan.Server.Infrastructure.Authentication;

internal sealed class PrincipalProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PrincipalProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public (PrincipalKind Kind, Guid Id) GetCurrentPrincipal()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context is null)
        {
            throw new InvalidOperationException($"The {nameof(HttpContext)} is currently not available.");
        }

        PrincipalKind? kind = null;
        Guid? principalId = null;

        foreach (var claim in context.User.Claims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.PrincipalKind:
                    kind = Enum.Parse<PrincipalKind>(claim.Value);
                    break;
                case ClaimTypes.PrincipalId:
                    principalId = Guid.Parse(claim.Value);
                    break;
            }
        }

        if (kind.HasValue && principalId.HasValue)
        {
            return (kind.Value, principalId.Value);
        }

        throw new InvalidOperationException("Could not determine current principal from the claims of the current user identity.");
    }
}
