using Turnierplan.App.Models;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class TournamentHeaderMappingRule : MappingRuleBase<Tournament, TournamentHeaderDto>
{
    protected override TournamentHeaderDto Map(IMapper mapper, MappingContext context, Tournament source)
    {
        return new TournamentHeaderDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            FolderId = source.Folder?.PublicId,
            Name = source.Name,
            OrganizationName = source.Organization.Name,
            FolderName = source.Folder?.Name,
            Visibility = source.Visibility
        };
    }
}
