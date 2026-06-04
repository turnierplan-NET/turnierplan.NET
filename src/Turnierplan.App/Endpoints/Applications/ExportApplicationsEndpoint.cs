using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.OpenApi;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class ExportApplicationsEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications-export";

    protected override Delegate Handler => Handle;

    protected override void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.ProducesCsv();
    }

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromQuery] bool includeApplicationTeams,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator)
    {
        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.ApplicationsWithTeams);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ApplicationsRead))
        {
            return Results.Forbid();
        }

        return Results.Csv(async csv =>
        {
            if (includeApplicationTeams)
            {
                await csv.WriteHeaderAsync(
                    "ApplicationTag",
                    "ApplicationCreatedAt",
                    "ApplicationContactPerson",
                    "ApplicationContactEmail",
                    "ApplicationContactTelephone",
                    "ApplicationComment",
                    "ApplicationNotes",
                    "TournamentClass",
                    "TeamName",
                    "TeamLabels"
                );
            }
            else
            {
                await csv.WriteHeaderAsync(
                    "Tag",
                    "CreatedAt",
                    "NumberOfTeams",
                    "ContactPerson",
                    "ContactEmail",
                    "ContactTelephone",
                    "Comment",
                    "Notes"
                );
            }

            foreach (var application in planningRealm.Applications.OrderBy(x => x.CreatedAt))
            {
                if (includeApplicationTeams)
                {
                    foreach (var team in application.Teams.OrderBy(x => x.Name))
                    {
                        await csv.WriteRowAsync(
                            application.Tag,
                            application.CreatedAt,
                            application.Contact,
                            application.ContactEmail,
                            application.ContactTelephone,
                            application.Comment,
                            application.Notes,
                            team.Class.Name,
                            team.Name,
                            string.Join(", ", team.Labels.Select(x => x.Name))
                        );
                    }
                }
                else
                {
                    await csv.WriteRowAsync(
                        application.Tag,
                        application.CreatedAt,
                        application.Teams.Count,
                        application.Contact,
                        application.ContactEmail,
                        application.ContactTelephone,
                        application.Comment,
                        application.Notes
                    );
                }
            }
        });
    }
}
