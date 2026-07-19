using Turnierplan.App.Models;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ResourcePlannerHeaderMappingRule : MappingRuleBase<ResourcePlanner, ResourcePlannerHeaderDto>
{
    protected override ResourcePlannerHeaderDto Map(IMapper mapper, MappingContext context, ResourcePlanner source)
    {
        return new ResourcePlannerHeaderDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            Name = source.Name
        };
    }
}
