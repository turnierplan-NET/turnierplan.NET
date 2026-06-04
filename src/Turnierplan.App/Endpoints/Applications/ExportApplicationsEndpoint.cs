using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.OpenApi;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;
using Turnierplan.Localization;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class ExportApplicationsEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/tournament-planners/{tournamentPlannerId}/applications-export";

    protected override Delegate Handler => Handle;

    protected override void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.ProducesCsv();
    }

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentPlannerId,
        [FromQuery] string languageCode,
        [FromQuery] bool includeApplicationTeams,
        ITournamentPlannerRepository tournamentPlannerRepository,
        IAccessValidator accessValidator,
        ILocalizationProvider localizationProvider)
    {
        if (!localizationProvider.TryGetLocalization(languageCode, out var localization))
        {
            return Results.BadRequest("Invalid language code specified.");
        }

        var tournamentPlanner = await tournamentPlannerRepository.GetByPublicIdAsync(tournamentPlannerId, ITournamentPlannerRepository.Includes.ApplicationsWithTeams);

        if (tournamentPlanner is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournamentPlanner, Actions.ApplicationsRead))
        {
            return Results.Forbid();
        }

        return Results.Csv(async csv =>
        {
            if (includeApplicationTeams)
            {
                await csv.WriteHeaderAsync(
                    localization.Get("ApplicationsExport.Columns.Tag"),
                    localization.Get("ApplicationsExport.Columns.CreatedAt"),
                    localization.Get("ApplicationsExport.Columns.ContactPerson"),
                    localization.Get("ApplicationsExport.Columns.ContactEmail"),
                    localization.Get("ApplicationsExport.Columns.ContactTelephone"),
                    localization.Get("ApplicationsExport.Columns.Comment"),
                    localization.Get("ApplicationsExport.Columns.Notes"),
                    localization.Get("ApplicationsExport.Columns.TournamentClass"),
                    localization.Get("ApplicationsExport.Columns.TeamName"),
                    localization.Get("ApplicationsExport.Columns.TeamLabels")
                );
            }
            else
            {
                await csv.WriteHeaderAsync(
                    localization.Get("ApplicationsExport.Columns.Tag"),
                    localization.Get("ApplicationsExport.Columns.CreatedAt"),
                    localization.Get("ApplicationsExport.Columns.NumberOfTeams"),
                    localization.Get("ApplicationsExport.Columns.ContactPerson"),
                    localization.Get("ApplicationsExport.Columns.ContactEmail"),
                    localization.Get("ApplicationsExport.Columns.ContactTelephone"),
                    localization.Get("ApplicationsExport.Columns.Comment"),
                    localization.Get("ApplicationsExport.Columns.Notes")
                );
            }

            foreach (var application in tournamentPlanner.Applications.OrderBy(x => x.CreatedAt))
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
                            team.Labels.Count == 0 ? string.Empty : string.Join(", ", team.Labels.Select(x => x.Name))
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
