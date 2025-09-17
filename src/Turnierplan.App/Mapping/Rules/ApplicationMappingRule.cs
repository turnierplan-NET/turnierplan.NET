using Turnierplan.App.Models;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ApplicationMappingRule : MappingRuleBase<Application, ApplicationDto>
{
    protected override ApplicationDto Map(IMapper mapper, MappingContext context, Application source)
    {
        return new ApplicationDto
        {
            Id = source.Id,
            Tag = source.Tag,
            CreatedAt = source.CreatedAt,
            Notes = source.Notes,
            Contact = source.Contact,
            ContactEmail = source.ContactEmail,
            ContactTelephone = source.ContactTelephone,
            Comment = source.Comment,
            Teams = source.Teams.Select(team =>
            {
                if (team.TeamLink is null)
                {
                    return new ApplicationTeamDto
                    {
                        Id = team.Id,
                        TournamentClassId = team.Class.Id,
                        Name = team.Name
                    };
                }

                return new ApplicationTeamDto
                {
                    Id = team.Id,
                    TournamentClassId = team.Class.Id,
                    Name = team.Name,
                    LinkedTournamentId = team.TeamLink.Team.Tournament.PublicId,
                    LinkedTournamentFolderName = team.TeamLink.Team.Tournament.Folder?.Name,
                    LinkedTournamentName = team.TeamLink.Team.Tournament.Name
                };
            }).ToArray()
        };
    }
}
