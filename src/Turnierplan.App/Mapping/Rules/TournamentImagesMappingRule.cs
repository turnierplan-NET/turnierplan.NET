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
            HasPrimaryLogo = source.PrimaryLogo is not null,
            HasSecondaryLogo = source.SecondaryLogo is not null,
            HasBannerImage = source.BannerImage is not null,
            PrimaryLogo = mapper.MapNullable<ImageDto>(source.PrimaryLogo),
            SecondaryLogo = mapper.MapNullable<ImageDto>(source.SecondaryLogo),
            BannerImage = mapper.MapNullable<ImageDto>(source.BannerImage)
        };
    }
}
