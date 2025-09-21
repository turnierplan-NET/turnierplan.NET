using Turnierplan.App.Models;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class TournamentImagesMappingRule : MappingRuleBase<Tournament, TournamentImagesDto>
{
    protected override TournamentImagesDto Map(IMapper mapper, MappingContext context, Tournament source)
    {
        return new TournamentImagesDto // TODO Rename properties
        {
            TournamentId = source.PublicId,
            HasOrganizerLogo = source.PrimaryLogo is not null,
            HasSponsorLogo = source.SecondaryLogo is not null,
            HasSponsorBanner = source.BannerImage is not null,
            OrganizerLogo = mapper.MapNullable<ImageDto>(source.PrimaryLogo),
            SponsorLogo = mapper.MapNullable<ImageDto>(source.SecondaryLogo),
            SponsorBanner = mapper.MapNullable<ImageDto>(source.BannerImage)
        };
    }
}
