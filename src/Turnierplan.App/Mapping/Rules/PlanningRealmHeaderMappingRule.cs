using Turnierplan.App.Models;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class PlanningRealmHeaderMappingRule : MappingRuleBase<TournamentPlanner, PlanningRealmHeaderDto>
{
    protected override PlanningRealmHeaderDto Map(IMapper mapper, MappingContext context, TournamentPlanner source)
    {
        return new PlanningRealmHeaderDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            Name = source.Name
        };
    }
}
