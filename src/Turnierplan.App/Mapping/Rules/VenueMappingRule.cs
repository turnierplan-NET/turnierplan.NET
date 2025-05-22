using Turnierplan.App.Models;
using Turnierplan.Core.Venue;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class VenueMappingRule : MappingRuleBase<Venue, VenueDto>
{
    protected override VenueDto Map(IMapper mapper, MappingContext context, Venue source)
    {
        return new VenueDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            Name = source.Name,
            Description = source.Description,
            AddressDetails = source.AddressDetails.ToArray(),
            ExternalLinks = source.ExternalLinks.ToArray()
        };
    }
}
