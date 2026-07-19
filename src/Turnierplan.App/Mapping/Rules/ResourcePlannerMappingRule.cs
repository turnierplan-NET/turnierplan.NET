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
            Resources =
            [
                ..source.ResourceGroups
                    .SelectMany(group => group.ResourceAssignments)
                    .Select(assignment => assignment.Resource)
                    .Distinct()
                    .Select(resource => new ResourceDto
                    {
                        Id = resource.Id,
                        Type = resource.Type,
                        Name = resource.Name,
                        Notes = resource.Notes
                    })
            ],
            ResourceGroups =
            [
                ..source.ResourceGroups.Select(group => new ResourceGroupDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Type = group.Type,
                    Start = group.Start,
                    End = group.End,
                    Assignment =
                    [
                        ..group.ResourceAssignments.Select(assignment => new ResourceAssignmentDto
                        {
                            ResourceId = assignment.Resource.Id,
                            State = assignment.State
                        })
                    ]
                })
            ],
            Views =
            [
                ..source.ResourcePlannerViews.Select(view => new ResourcePlannerViewDto
                {
                    Id = view.Id,
                    PublicId = view.PublicId,
                    IsActive = view.IsActive,
                    DisplayAllGroups = view.DisplayAllGroups,
                    ResourceGroupIds =
                    [
                        ..view.ResourceGroups.Select(group => group.Id)
                    ]
                })
            ]
        };
    }
}
