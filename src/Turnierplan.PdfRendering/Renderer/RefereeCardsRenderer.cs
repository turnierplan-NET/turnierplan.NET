using Microsoft.ApplicationInsights;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Turnierplan.Core.Tournament;
using Turnierplan.Localization;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Extensions;

namespace Turnierplan.PdfRendering.Renderer;

[DocumentRenderer]
public sealed class RefereeCardsRenderer(TelemetryClient telemetryClient) : DocumentRendererBase<RefereeCardsDocumentConfiguration>(telemetryClient)
{
    protected override Document Render(Tournament tournament, RefereeCardsDocumentConfiguration configuration, ILocalization localization)
    {
        var cards = tournament.GenerateRefereeCards();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(column =>
                {
                    column.Spacing(3, Unit.Millimetre);

                    for (var i = 0; i < cards.Count; i += 2)
                    {
                        var cardLeft = cards[i];
                        var cardRight = i + 1 >= cards.Count ? null : cards[i + 1];

                        column.Item().Row(row =>
                        {
                            row.Spacing(3, Unit.Millimetre);

                            row.RelativeItem(0.5f).RefereeCard(tournament, cardLeft.Match, cardLeft.RefereeTeam, localization);

                            if (cardRight is null)
                            {
                                row.RelativeItem(0.5f);
                            }
                            else
                            {
                                row.RelativeItem(0.5f).RefereeCard(tournament, cardRight.Match, cardRight.RefereeTeam, localization);
                            }
                        });
                    }
                });
            });
        }).WithTurnierplanMetadata(localization);
    }
}

file static class RefereeCardsQuestPdfExtensions
{
    public static void RefereeCard(this IContainer container, Tournament tournament, Match match, Team? refereeTeam, ILocalization localization)
    {
        container.EnsureSpace().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(0.5f);
                columns.RelativeColumn(0.5f);
            });

            table.Cell().ColumnSpan(2).MinHeight(18, Unit.Millimetre).Border(1).Padding(4).Column(column =>
            {
                var matchTitle = match.Kickoff is null
                    ? localization.Get("Documents.RefereeCards.MatchInfo", match.Index)
                    : localization.Get("Documents.RefereeCards.MatchInfoWithKickoff", match.Index, match.Kickoff);
                column.Item().Text(matchTitle).SemiBold();

                column.Item().Text(match.IsDecidingMatch
                    ? localization.LocalizeMatchDisplayName(match)
                    : localization.Get("Documents.RefereeCards.RefereeTeam", refereeTeam?.Name ?? string.Empty));
            });

            var teamA = match.IsGroupMatch
                ? match.TeamA?.Name ?? string.Empty
                : localization.LocalizeTeamSelector(match.TeamSelectorA, tournament);

            var teamB = match.IsGroupMatch
                ? match.TeamB?.Name ?? string.Empty
                : localization.LocalizeTeamSelector(match.TeamSelectorB, tournament);

            table.Cell().MinHeight(25, Unit.Millimetre).Border(1).Padding(4).Text(teamA).FontSize(8);
            table.Cell().MinHeight(25, Unit.Millimetre).Border(1).Padding(4).Text(teamB).FontSize(8);
        });
    }
}
