using Turnierplan.App.Models;
using Turnierplan.Core.TournamentPlanner;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class TournamentPlannerHeaderMappingRule : MappingRuleBase<TournamentPlanner, TournamentPlannerHeaderDto>
{
    protected override TournamentPlannerHeaderDto Map(IMapper mapper, MappingContext context, TournamentPlanner source)
    {
        return new TournamentPlannerHeaderDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            Name = source.Name
        };
    }
}
