using Turnierplan.App.Models;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ResourceGroupMappingRule : MappingRuleBase<ResourceGroup, ResourceGroupDto>
{
    protected override ResourceGroupDto Map(IMapper mapper, MappingContext context, ResourceGroup source)
    {
        return new ResourceGroupDto
        {
            Id = source.Id,
            Name = source.Name,
            Notes = source.Notes,
            Start = source.Start,
            End = source.End,
            Assignments =
            [
                .. source.ResourceAssignments.Select(assignment => new ResourceAssignmentDto
                {
                    ResourceId = assignment.Resource.Id,
                    State = assignment.State
                })
            ]
        };
    }
}
