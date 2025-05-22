using Turnierplan.App.Models;
using Turnierplan.Core.Folder;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class FolderMappingRule : MappingRuleBase<Folder, FolderDto>
{
    protected override FolderDto Map(IMapper mapper, MappingContext context, Folder source)
    {
        return new FolderDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            Name = source.Name
        };
    }
}
