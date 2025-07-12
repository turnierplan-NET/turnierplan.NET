using Turnierplan.App.Helpers;
using Turnierplan.App.Models;
using Turnierplan.Core.Planning;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class PlanningRealmMappingRule: MappingRuleBase<PlanningRealm, PlanningRealmDto>
{
    protected override PlanningRealmDto Map(IMapper mapper, MappingContext context, PlanningRealm source)
    {
        return new PlanningRealmDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            RbacScopeId = source.GetScopeId(),
            Name = source.Name
        };
    }
}
