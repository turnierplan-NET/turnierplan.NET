using Turnierplan.Core.Organization;

namespace Turnierplan.App.Security;

internal interface IAccessValidator
{
    bool CanSessionUserAccess(Organization organization);
}

internal sealed class AccessValidator : IAccessValidator
{
    private readonly HttpContext _httpContext;

    public AccessValidator(IHttpContextAccessor contextAccessor)
    {
        _httpContext = contextAccessor.HttpContext ?? throw new InvalidOperationException("Cannot access HttpContext");
    }

    public bool CanSessionUserAccess(Organization organization)
    {
        // TODO: Implement new access check :)

        return true;

        // return _httpContext.User.HasClaim(ClaimTypes.UserId, organization.OwnerId.ToString())
        // || _httpContext.User.HasClaim(ClaimTypes.OrganizationId, organization.Id.ToString());
    }
}
