using Turnierplan.App.Helpers;
using Turnierplan.App.Models;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ResourcePlannerMappingRule : MappingRuleBase<ResourcePlanner, ResourcePlannerDto>
{
    protected override ResourcePlannerDto Map(IMapper mapper, MappingContext context, ResourcePlanner source)
    {
        return new ResourcePlannerDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            RbacScopeId = source.GetScopeId(),
            Name = source.Name,
            // groups
            // views
        };
    }
}
