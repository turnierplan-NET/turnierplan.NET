using Turnierplan.App.Helpers;
using Turnierplan.App.Models;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Mapping.Rules;

internal abstract class RoleAssignmentMappingRuleBase<T> : MappingRuleBase<RoleAssignment<T>, RoleAssignmentDto>
    where T : Entity<long>, IEntityWithRoleAssignments<T>
{
    protected override RoleAssignmentDto Map(IMapper mapper, MappingContext context, RoleAssignment<T> source)
    {
        return new RoleAssignmentDto
        {
            Id = source.Id,
            ScopeId = source.Scope.GetScopeId(),
            ScopeName = source.Scope.Name,
            CreatedAt = source.CreatedAt,
            Role = source.Role,
            Principal = new PrincipalDto
            {
                Kind = source.Principal.Kind,
                PrincipalId = source.Principal.PrincipalId
            },
            Description = source.Description,
            IsInherited = false
        };
    }
}

internal sealed class ApiKeyRoleAssignmentMappingRule : RoleAssignmentMappingRuleBase<ApiKey>;

internal sealed class FolderRoleAssignmentMappingRule : RoleAssignmentMappingRuleBase<Folder>;

internal sealed class ImageRoleAssignmentMappingRule : RoleAssignmentMappingRuleBase<Image>;

internal sealed class OrganizationRoleAssignmentMappingRule : RoleAssignmentMappingRuleBase<Organization>;

internal sealed class TournamentRoleAssignmentMappingRule : RoleAssignmentMappingRuleBase<Tournament>;

internal sealed class VenueRoleAssignmentMappingRule : RoleAssignmentMappingRuleBase<Venue>;
