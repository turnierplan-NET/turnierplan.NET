using Turnierplan.App.Models;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class PlanningRealmHeaderMappingRule : MappingRuleBase<PlanningRealm, PlanningRealmHeaderDto>
{
    protected override PlanningRealmHeaderDto Map(IMapper mapper, MappingContext context, PlanningRealm source)
    {
        return new PlanningRealmHeaderDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            Name = source.Name
        };
    }
}
