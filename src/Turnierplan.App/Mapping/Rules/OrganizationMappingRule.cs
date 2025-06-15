using Turnierplan.App.Helpers;
using Turnierplan.App.Models;
using Turnierplan.Core.Organization;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class OrganizationMappingRule : MappingRuleBase<Organization, OrganizationDto>
{
    protected override OrganizationDto Map(IMapper mapper, MappingContext context, Organization source)
    {
        return new OrganizationDto
        {
            Id = source.PublicId,
            RbacScopeId = source.GetScopeId(),
            Name = source.Name
        };
    }
}
