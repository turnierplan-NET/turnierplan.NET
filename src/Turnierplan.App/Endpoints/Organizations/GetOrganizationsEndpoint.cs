using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.Core.Organization;

namespace Turnierplan.App.Endpoints.Organizations;

internal sealed class GetOrganizationsEndpoint : EndpointBase<IEnumerable<OrganizationDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/organizations";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        HttpContext context,
        IOrganizationRepository repository,
        IMapper mapper)
    {
        List<Organization> organizations;

        if (context.IsCurrentUserAdministrator())
        {
            organizations = await repository.GetAllAsync();
        }
        else
        {
            var principal = context.GetActivePrincipal();

            organizations = await repository.GetByPrincipalAsync(principal);
        }

        return Results.Ok(mapper.MapCollection<OrganizationDto>(organizations));
    }
}
