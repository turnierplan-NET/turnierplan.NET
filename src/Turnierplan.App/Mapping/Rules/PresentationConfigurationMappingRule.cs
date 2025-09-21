using Turnierplan.App.Models;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class PresentationConfigurationMappingRule : MappingRuleBase<PresentationConfiguration, PresentationConfigurationDto>
{
    protected override PresentationConfigurationDto Map(IMapper mapper, MappingContext context,
        PresentationConfiguration source)
    {
        return new PresentationConfigurationDto
        {
            Header1 = new PresentationConfigurationDto.HeaderLine
            {
                Content = source.Header1.Content,
                CustomContent = source.Header1.CustomContent
            },
            Header2 = new PresentationConfigurationDto.HeaderLine
            {
                Content = source.Header2.Content,
                CustomContent = source.Header2.CustomContent
            },
            ShowResults = source.ShowResults,
            ShowPrimaryLogo = source.ShowPrimaryLogo,
            ShowSecondaryLogo = source.ShowSecondaryLogo
        };
    }
}
