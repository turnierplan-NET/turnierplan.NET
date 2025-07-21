using Turnierplan.App.Helpers;
using Turnierplan.App.Models;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class PlanningRealmMappingRule : MappingRuleBase<PlanningRealm, PlanningRealmDto>
{
    protected override PlanningRealmDto Map(IMapper mapper, MappingContext context, PlanningRealm source)
    {
        return new PlanningRealmDto
        {
            Id = source.PublicId,
            OrganizationId = source.Organization.PublicId,
            RbacScopeId = source.GetScopeId(),
            Name = source.Name,
            TournamentClasses = source.TournamentClasses.Select(x => new TournamentClassDto
            {
                Id = x.Id,
                Name = x.Name,
                MaxTeamCount = x.MaxTeamCount,
                NumberOfTeams = source.Applications.SelectMany(y => y.Teams).Count(y => y.Class == x)
            }).ToArray(),
            InvitationLinks = source.InvitationLinks.Select(x => new InvitationLinkDto
            {
                Id = x.Id,
                PublicId = x.PublicId,
                Name = x.Name,
                Title = x.Title,
                Description = x.Description,
                ColorCode = x.ColorCode,
                ValidUntil = x.ValidUntil,
                ContactPerson = x.ContactPerson,
                ContactEmail = x.ContactEmail,
                ContactTelephone = x.ContactTelephone,
                PrimaryLogo = mapper.MapNullable<ImageDto>(x.PrimaryLogo),
                SecondaryLogo = mapper.MapNullable<ImageDto>(x.SecondaryLogo),
                Entries = x.Entries.Select(y => new InvitationLinkEntryDto
                {
                    TournamentClassId = y.Class.Id,
                    MaxTeamsPerRegistration = y.MaxTeamsPerRegistration,
                    AllowNewRegistrations = y.AllowNewRegistrations
                }).ToArray(),
                NumberOfApplications = source.Applications.Count(y => y.SourceLink == x)
            }).ToArray()
        };
    }
}
