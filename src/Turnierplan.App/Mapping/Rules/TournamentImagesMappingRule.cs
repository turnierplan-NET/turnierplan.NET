using Turnierplan.App.Models;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class TournamentImagesMappingRule : MappingRuleBase<Tournament, TournamentImagesDto>
{
    protected override TournamentImagesDto Map(IMapper mapper, MappingContext context, Tournament source)
    {
        return new TournamentImagesDto
        {
            TournamentId = source.PublicId,
            HasOrganizerLogo = source.OrganizerLogo is not null,
            HasSponsorLogo = source.SponsorLogo is not null,
            HasSponsorBanner = source.SponsorBanner is not null,
            OrganizerLogo = mapper.MapNullable<ImageDto>(source.OrganizerLogo),
            SponsorLogo = mapper.MapNullable<ImageDto>(source.SponsorLogo),
            SponsorBanner = mapper.MapNullable<ImageDto>(source.SponsorBanner)
        };
    }
}
