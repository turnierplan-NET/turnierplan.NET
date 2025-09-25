using Microsoft.ApplicationInsights;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp.QrCode;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.ImageStorage;
using Turnierplan.Localization;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Extensions;

namespace Turnierplan.PdfRendering.Renderer;

[DocumentRenderer]
public sealed class MatchPlanRenderer(TelemetryClient telemetryClient, IImageStorage imageStorage, IApplicationUrlProvider applicationUrlProvider) : DocumentRendererBase<MatchPlanDocumentConfiguration>(telemetryClient)
{
    private readonly TelemetryClient _telemetryClient = telemetryClient;

    protected override Document Render(Tournament tournament, MatchPlanDocumentConfiguration configuration, ILocalization localization)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(19, Unit.Millimetre);
                page.MarginVertical(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Content().Column(column =>
                {
                    var currentSectionIndex = 1;

                    string GetSectionHeader(string key)
                    {
                        var sectionNumber = localization.Get($"Documents.Common.RomanNumerals.{currentSectionIndex}");
                        var sectionTitle = localization.Get(key);

                        if (sectionNumber.StartsWith("Documents."))
                        {
                            // The wanted number does not exist, so fall back to arabic numerals
                            sectionNumber = $"{currentSectionIndex}";
                        }

                        currentSectionIndex++;

                        return $"{sectionNumber}. {sectionTitle}";
                    }

                    if (tournament.PrimaryLogo is not null)
                    {
                        _telemetryClient.TrackTrace("Loading tournament primary logo from external source.");
                        column.Item().Unconstrained().Width(3, Unit.Centimetre).Image(tournament.PrimaryLogo, imageStorage);
                    }

                    if (tournament.SecondaryLogo is not null)
                    {
                        _telemetryClient.TrackTrace("Loading tournament secondary logo from external source.");
                        column.Item().AlignRight().Unconstrained().TranslateX(-3, Unit.Centimetre).Width(3, Unit.Centimetre).Image(tournament.SecondaryLogo, imageStorage);
                    }

                    var organizerName = string.IsNullOrWhiteSpace(configuration.OrganizerNameOverride)
                        ? tournament.Organization.Name
                        : configuration.OrganizerNameOverride;
                    column.Item().AlignCenter().PaddingBottom(12).Text(organizerName).FontSize(18);

                    foreach (var headerRow in GetHeaderRows(tournament, configuration, localization))
                    {
                        column.Item().PaddingTop(4).AlignCenter().Text(headerRow).FontSize(11);
                    }

                    var firstGroupMatch = tournament.Matches.Where(x => x.IsGroupMatch).MinBy(x => x.Kickoff);

                    if (firstGroupMatch is not null && tournament.MatchPlanConfiguration?.ScheduleConfig is not null)
                    {
                        column.Item().PaddingTop(16).AlignCenter().MatchTimeSection(firstGroupMatch.Kickoff, tournament.MatchPlanConfiguration.ScheduleConfig?.GroupPhasePlayTime, tournament.MatchPlanConfiguration.ScheduleConfig?.GroupPhasePauseTime, localization);
                    }

                    column.Item().PaddingVertical(16).Text(GetSectionHeader("Documents.MatchPlan.Sections.Participants")).Underline();

                    column.Groups(tournament, localization, MatchPlanOutcomes.HideOutcomeStructures);

                    column.Item().PaddingVertical(16).Text(GetSectionHeader("Documents.MatchPlan.Sections.GroupPhase")).Underline();

                    column.Item().AlignCenter().GroupPhaseMatches(tournament, localization, configuration.Outcomes);

                    if (tournament.BannerImage is not null)
                    {
                        _telemetryClient.TrackTrace("Loading tournament banner image from external source.");
                        column.Item().PaddingVertical(16).Image(tournament.BannerImage, imageStorage);
                    }

                    if (configuration.Outcomes is MatchPlanOutcomes.ShowEmptyOutcomeStructures or MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes)
                    {
                        column.Item().ShowEntire().Column(column2 =>
                        {
                            column2.Item().PaddingVertical(16).Text(GetSectionHeader("Documents.MatchPlan.Sections.GroupPhaseResults")).Underline();
                            column2.Groups(tournament, localization, configuration.Outcomes);
                        });
                    }

                    var decidingMatches = tournament.Matches.Where(x => x.IsDecidingMatch).ToList();

                    if (decidingMatches.Count > 0)
                    {
                        // 200pt is approx. the height of a finals block with two semi-finals, 3rd place playoff and final match
                        column.Item().EnsureSpace(200).Column(column2 =>
                        {
                            column2.Item().PaddingVertical(16).Text(GetSectionHeader("Documents.MatchPlan.Sections.FinalsPhase")).Underline();

                            if (tournament.MatchPlanConfiguration?.ScheduleConfig is not null)
                            {
                                var firstFinalsMatch = decidingMatches.MinBy(x => x.Kickoff);
                                column2.Item().PaddingBottom(16).AlignCenter().MatchTimeSection(firstFinalsMatch!.Kickoff, tournament.MatchPlanConfiguration.ScheduleConfig?.FinalsPhasePlayTime, tournament.MatchPlanConfiguration.ScheduleConfig?.FinalsPhasePauseTime, localization);
                            }

                            // All matches of each group should appear on the same page. I.e. there shall be no page breaks between matches with the same "color"
                            var matchGroupings = decidingMatches.OrderBy(x => x.Index).GroupBy(GetDecidingMatchColor);

                            var isFirstGrouping = true;

                            foreach (var grouping in matchGroupings)
                            {
                                column2.Item().PaddingTop(isFirstGrouping ? 0 : 8).ShowEntire().Column(column3 =>
                                {
                                    column3.Spacing(4);

                                    foreach (var match in grouping)
                                    {
                                        column3.Item().DecidingMatch(tournament, match, grouping.Key, localization, configuration.Outcomes);
                                    }
                                });

                                isFirstGrouping = false;
                            }
                        });
                    }

                    if (configuration.IncludeRankingTable)
                    {
                        column.Item().ShowEntire().Column(column2 =>
                        {
                            column2.Item().PaddingVertical(16).Text(GetSectionHeader("Documents.MatchPlan.Sections.Ranking")).Underline();

                            column2.Item().AlignCenter().Width(9, Unit.Centimetre).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(0.07f);
                                    columns.RelativeColumn(0.93f);
                                });

                                var numberOfRankings = tournament.Teams.Count;
                                for (var ranking = 1; ranking <= numberOfRankings; ranking++)
                                {
                                    var team = configuration.Outcomes is MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes
                                        ? tournament.Teams.FirstOrDefault(x => x.Ranking.HasValue && x.Ranking.Value == ranking)?.Name ?? string.Empty
                                        : string.Empty;

                                    var background = ranking switch
                                    {
                                        1 => "ffd700",
                                        2 => "c9c9c9",
                                        3 => "f49e4c",
                                        _ => "ffffff"
                                    };

                                    table.Cell().Background(background)
                                        .BorderLeft(2)
                                        .BorderTop(ranking == 1 ? 2 : 0.5f)
                                        .BorderBottom(ranking == numberOfRankings ? 2 : 0.5f)
                                        .BorderColor(Colors.Black).Padding(2).Text($"{ranking}.").AlignEnd();

                                    table.Cell().Background(background)
                                        .BorderRight(2)
                                        .BorderTop(ranking == 1 ? 2 : 0.5f)
                                        .BorderBottom(ranking == numberOfRankings ? 2 : 0.5f)
                                        .BorderColor(Colors.Black).Padding(2).Text(team);
                                }
                            });
                        });
                    }

                    if (configuration.IncludeQrCode)
                    {
                        column.Item().ShowEntire().Column(column2 =>
                        {
                            column2.Item().PaddingTop(40).AlignCenter().Height(3.0f, Unit.Centimetre).Row(row =>
                            {
                                row.Spacing(1, Unit.Centimetre);

                                var url = GetPublicTournamentUrl(tournament.PublicId);

                                row.ConstantItem(3.0f, Unit.Centimetre).SkiaSharpSvgCanvas((canvas, size) =>
                                {
                                    using var qr = new QRCodeGenerator();
                                    var data = qr.CreateQrCode(url, ECCLevel.M, quietZoneSize: 1);
                                    canvas.Render(data, (int)size.Width, (int)size.Height);
                                });

                                row.ConstantItem(9, Unit.Centimetre).AlignMiddle().Text(text =>
                                {
                                    text.Line(localization.Get("Documents.MatchPlan.QrCodeInfo1")).Bold();
                                    text.Line(localization.Get("Documents.MatchPlan.QrCodeInfo2")).Bold();
                                    text.EmptyLine();
                                    text.Hyperlink(url, url).Underline();
                                });
                            });
                        });
                    }
                });
            });
        }).WithTurnierplanMetadata(localization);
    }

    /// <remarks>Internal for testing.</remarks>
    internal string GetPublicTournamentUrl(PublicId tournamentId)
    {
        return $"{applicationUrlProvider.GetApplicationUrl().TrimEnd('/')}/tournament?id={tournamentId.ToString()}";
    }

    private static IEnumerable<string> GetHeaderRows(Tournament tournament, MatchPlanDocumentConfiguration configuration, ILocalization localization)
    {
        if (!string.IsNullOrWhiteSpace(configuration.TournamentNameOverride))
        {
            yield return configuration.TournamentNameOverride;
        }
        else
        {
            yield return tournament.Name;
        }

        if (configuration.DateFormat is not MatchPlanDateFormat.NoDate)
        {
            var matchKickoffTimes = tournament.Matches
                .Select(x => x.Kickoff)
                .WhereNotNull()
                .ToList();

            if (matchKickoffTimes.Count > 0)
            {
                var kickoff = matchKickoffTimes.Min();

                switch (configuration.DateFormat)
                {
                    case MatchPlanDateFormat.DateOnly:
                        yield return localization.Get("Documents.MatchPlan.TournamentKickoff.DateOnly", kickoff);
                        break;
                    case MatchPlanDateFormat.DateAndDayOfWeek:
                        yield return localization.Get("Documents.MatchPlan.TournamentKickoff.DateAndDayOfWeek", kickoff);
                        break;
                    case MatchPlanDateFormat.DateAndDayOfWeekAndTimeOfDay:
                        var hourOfDay = kickoff.TimeOfDay.Hours;
                        yield return localization.Get(hourOfDay switch
                        {
                            <= 12 => "Documents.MatchPlan.TournamentKickoff.DateAndDayOfWeekForenoon",
                            <= 18 => "Documents.MatchPlan.TournamentKickoff.DateAndDayOfWeekAfternoon",
                            _ => "Documents.MatchPlan.TournamentKickoff.DateAndDayOfWeekEvening"
                        }, kickoff);
                        break;
                    case MatchPlanDateFormat.NoDate:
                    default:
                        throw new InvalidOperationException($"Date format is invalid: {configuration.DateFormat}");
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(configuration.VenueOverride))
        {
            yield return configuration.VenueOverride;
        }
        else
        {
            var venueName = tournament.Venue?.Name;

            if (!string.IsNullOrWhiteSpace(venueName))
            {
                yield return venueName;
            }
        }
    }

    private static string GetDecidingMatchColor(Match match)
    {
        if (match.PlayoffPosition is not null)
        {
            return "cdffcd";
        }

        return match.FinalsRound switch
        {
            (int)FinalsRoundOrder.SemiFinals => "cdffff",
            (int)FinalsRoundOrder.QuarterFinals => "fecc97",
            (int)FinalsRoundOrder.EighthFinals => "c0bdff",
            _ => "aaaaaa"
        };
    }
}

file static class MatchPlanQuestPdfExtensions
{
    private const string TableHeaderBackgroundColor = "aaaaaa";

    public static void MatchTimeSection(this IContainer container, DateTime? kickoff, TimeSpan? playTime, TimeSpan? pauseTime, ILocalization localization)
    {
        container.Row(row =>
        {
            row.Spacing(5, Unit.Millimetre);

            if (kickoff.HasValue)
            {
                row.AutoItem().Text(text =>
                {
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.KickoffPre"));
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.Kickoff", kickoff)).Underline().Bold();
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.KickoffPost"));
                });
            }

            if (playTime.HasValue)
            {
                row.AutoItem().Text(text =>
                {
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.PlayTimePre"));
                    var minutes = (int)playTime.Value.TotalMinutes;
                    var seconds = (int)playTime.Value.TotalSeconds - minutes * 60;
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.PlayTime", minutes, seconds)).Underline().Bold();
                    text.Span(localization.Get($"Documents.MatchPlan.MatchTimes.PlayTimePost.{(minutes == 1 && seconds == 0 ? "One" : "Many")}"));
                });
            }

            if (pauseTime.HasValue)
            {
                row.AutoItem().Text(text =>
                {
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.PauseTimePre"));
                    var minutes = (int)pauseTime.Value.TotalMinutes;
                    var seconds = (int)pauseTime.Value.TotalSeconds - minutes * 60;
                    text.Span(localization.Get("Documents.MatchPlan.MatchTimes.PauseTime", minutes, seconds)).Underline().Bold();
                    text.Span(localization.Get($"Documents.MatchPlan.MatchTimes.PauseTimePost.{(minutes == 1 && seconds == 0 ? "One" : "Many")}"));
                });
            }
        });
    }

    public static void Groups(this ColumnDescriptor column, Tournament tournament, ILocalization localization, MatchPlanOutcomes outcomes)
    {
        var groupsArray = tournament.Groups.OrderBy(group => group.AlphabeticalId).ToArray();
        for (var i = 0; i < groupsArray.Length; i += 2)
        {
            var groupLeft = groupsArray[i];
            var groupRight = (i + 1) < groupsArray.Length ? groupsArray[i + 1] : null;

            if (groupRight is not null)
            {
                var isLastRow = i + 2 >= groupsArray.Length;
                column.Item().PaddingBottom(isLastRow ? 0 : 6, Unit.Millimetre).AlignCenter().Row(row =>
                {
                    row.Spacing(6, Unit.Millimetre);
                    row.RelativeItem().AlignRight().Group(groupLeft, localization, outcomes);
                    row.RelativeItem().Group(groupRight, localization, outcomes);
                });
            }
            else
            {
                column.Item().AlignCenter().Group(groupLeft, localization, outcomes);
            }
        }
    }

    public static void GroupPhaseMatches(this IContainer container, Tournament tournament, ILocalization localization, MatchPlanOutcomes outcomes)
    {
        var showResultColumn = outcomes is MatchPlanOutcomes.ShowEmptyOutcomeStructures or MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes;
        var insertOutcomes = outcomes is MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                // Sum of all column widths should equal the width of two group tables + the spacing in between them
                columns.ConstantColumn(7, Unit.Millimetre); // Index
                columns.ConstantColumn(9, Unit.Millimetre); // Group
                columns.ConstantColumn(9, Unit.Millimetre); // Court
                columns.ConstantColumn(12, Unit.Millimetre); // Kickoff
                columns.ConstantColumn(showResultColumn ? 55 : 62, Unit.Millimetre); // Team A
                columns.ConstantColumn(3, Unit.Millimetre); // Separator
                columns.ConstantColumn(showResultColumn ? 55 : 62, Unit.Millimetre); // Team B

                if (showResultColumn)
                {
                    // If this column is not shown, its width is evenly distributed amongst team A/B columns
                    columns.ConstantColumn(14, Unit.Millimetre); // Outcome
                }

                columns.ConstantColumn(6, Unit.Millimetre); // Empty Column
            });

            table.Header(header =>
            {
                header.Cell().Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Index")).Bold();
                header.Cell().Background(TableHeaderBackgroundColor).BorderHorizontal(2).BorderLeft(2).BorderRight(0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Group")).Bold();
                header.Cell().Background(TableHeaderBackgroundColor).BorderHorizontal(2).BorderVertical(0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Court")).Bold();
                header.Cell().Background(TableHeaderBackgroundColor).BorderHorizontal(2).BorderRight(2).BorderLeft(0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Kickoff")).Bold();
                header.Cell().ColumnSpan(3).Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Teams")).Bold();

                if (showResultColumn)
                {
                    header.Cell().Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Outcome")).Bold();
                }

                header.Cell().Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black);
            });

            var matches = tournament.Matches.Where(x => x.IsGroupMatch).OrderBy(x => x.Index).ToArray();

            for (var i = 0; i < matches.Length; i++)
            {
                var match = matches[i];
                var isLast = i == matches.Length - 1;

                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderLeft(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text($"{match.Index}");
                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderLeft(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(match.Group!.AlphabeticalId.ToString());
                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderLeft(0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text($"{match.Court + 1}");
                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderLeft(0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(match.Kickoff is null ? string.Empty : localization.Get("Documents.MatchPlan.MatchKickoffTime", match.Kickoff));
                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderLeft(2).BorderColor(Colors.Black).Padding(2).Text(match.TeamA?.Name ?? string.Empty).ClampLines(1);
                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text("-");
                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderColor(Colors.Black).Padding(2).Text(match.TeamB?.Name ?? string.Empty).ClampLines(1);

                if (showResultColumn)
                {
                    var text = insertOutcomes && match.IsFinished ? $"{match.ScoreA} : {match.ScoreB}" : ":";
                    table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderLeft(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(text).Bold();
                }

                table.Cell().BorderTop(0.5f).BorderBottom(isLast ? 2 : 0.5f).BorderVertical(2).BorderColor(Colors.Black);
            }
        });
    }

    public static void DecidingMatch(this IContainer container, Tournament tournament, Match match, string headerColor, ILocalization localization, MatchPlanOutcomes outcomes)
    {
        var showResultColumn = outcomes is MatchPlanOutcomes.ShowEmptyOutcomeStructures or MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes;
        var insertOutcomes = outcomes is MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                // Sum of all column widths should equal the width of two group tables + the spacing in between them
                columns.ConstantColumn(7, Unit.Millimetre); // Index
                columns.ConstantColumn(9, Unit.Millimetre); // Court
                columns.ConstantColumn(12, Unit.Millimetre); // Kickoff
                columns.ConstantColumn(showResultColumn ? 59 : 66, Unit.Millimetre); // Team A
                columns.ConstantColumn(4, Unit.Millimetre); // Separator
                columns.ConstantColumn(showResultColumn ? 59 : 66, Unit.Millimetre); // Team B

                if (showResultColumn)
                {
                    // If this column is not shown, its width is evenly distributed amongst team A/B columns
                    columns.ConstantColumn(14, Unit.Millimetre); // Outcome
                }

                columns.ConstantColumn(6, Unit.Millimetre); // Empty Column
            });

            table.Header(header =>
            {
                header.Cell().Background(headerColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Index")).Bold();
                header.Cell().Background(headerColor).BorderHorizontal(2).BorderRight(0.5f).BorderLeft(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Court")).Bold();
                header.Cell().Background(headerColor).BorderHorizontal(2).BorderRight(2).BorderLeft(0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Kickoff")).Bold();
                header.Cell().ColumnSpan(3).Background(headerColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.LocalizeMatchDisplayName(match)).Bold();

                if (showResultColumn)
                {
                    header.Cell().Background(headerColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Outcome")).Bold();
                }

                header.Cell().Background(headerColor).Border(2).BorderColor(Colors.Black);
            });

            table.Cell().RowSpan(2).Border(2).Padding(2).AlignCenter().AlignMiddle().Text($"{match.Index}");
            table.Cell().RowSpan(2).BorderHorizontal(2).BorderLeft(2).BorderRight(0.5f).Padding(2).AlignCenter().AlignMiddle().Text($"{match.Court + 1}");
            table.Cell().RowSpan(2).BorderHorizontal(2).BorderLeft(0.5f).BorderRight(2).Padding(2).AlignCenter().AlignMiddle().Text(match.Kickoff is null ? string.Empty : localization.Get("Documents.MatchPlan.MatchKickoffTime", match.Kickoff));

            if (insertOutcomes)
            {
                table.Cell().BorderBottom(0.5f).Padding(2).Text(match.TeamA?.Name ?? string.Empty).AlignCenter().ClampLines(1);
                table.Cell().BorderBottom(0.5f).Padding(2).Text("-").AlignCenter();
                table.Cell().BorderBottom(0.5f).Padding(2).Text(match.TeamB?.Name ?? string.Empty).AlignCenter().ClampLines(1);
            }
            else
            {
                table.Cell().BorderBottom(0.5f).Padding(2).Text(string.Empty);
                table.Cell().BorderBottom(0.5f).Padding(2).Text("-").AlignCenter();
                table.Cell().BorderBottom(0.5f).Padding(2).Text(string.Empty);
            }

            if (showResultColumn)
            {
                var text = insertOutcomes && match.IsFinished ? $"{match.ScoreA} : {match.ScoreB}" : ":";
                table.Cell().RowSpan(2).Border(2).Padding(2).AlignCenter().AlignMiddle().Text(text).Bold();
            }

            table.Cell().RowSpan(2).Border(2).Padding(2).AlignCenter().AlignMiddle();
            table.Cell().BorderBottom(2).PaddingBottom(1).AlignCenter().Text(localization.LocalizeTeamSelector(match.TeamSelectorA, tournament)).FontSize(7);
            table.Cell().BorderBottom(2);
            table.Cell().BorderBottom(2).PaddingBottom(1).AlignCenter().Text(localization.LocalizeTeamSelector(match.TeamSelectorB, tournament)).FontSize(7);
        });
    }

    private static void Group(this IContainer container, Group group, ILocalization localization, MatchPlanOutcomes outcomes)
    {
        var showResultColumns = outcomes is MatchPlanOutcomes.ShowEmptyOutcomeStructures or MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes;
        var insertOutcomes = outcomes is MatchPlanOutcomes.ShowOutcomeStructuresWithOutcomes;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                if (showResultColumns)
                {
                    columns.ConstantColumn(7, Unit.Millimetre); // Index
                    columns.ConstantColumn(48, Unit.Millimetre); // Team Name
                    columns.ConstantColumn(8, Unit.Millimetre); // Points
                    columns.ConstantColumn(11, Unit.Millimetre); // Goals
                    columns.ConstantColumn(8, Unit.Millimetre); // Goal Diff.
                }
                else
                {
                    columns.ConstantColumn(7, Unit.Millimetre); // Index
                    columns.ConstantColumn(75, Unit.Millimetre); // Team Name
                }
            });

            table.Header(header =>
            {
                header.Cell().ColumnSpan(2).Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.LocalizeGroupName(group)).Bold();

                if (showResultColumns)
                {
                    header.Cell().Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Points")).Bold();
                    header.Cell().Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.Score")).Bold();
                    header.Cell().Background(TableHeaderBackgroundColor).Border(2).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(localization.Get("Documents.MatchPlan.Headers.ScoreDiff")).Bold();
                }
            });

            var participants = insertOutcomes
                ? group.Participants.OrderBy(x => x.Statistics.Position).ToList()
                : group.Participants.ToList();

            for (var i = 0; i < participants.Count; i++)
            {
                var participant = participants[i];
                var isLast = i == participants.Count - 1;

                table.Cell().BorderLeft(2).BorderBottom(isLast ? 2 : (showResultColumns ? 0.5f : 0)).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(insertOutcomes ? $"{participant.Statistics.Position}." : $"{i + 1}.");
                table.Cell().BorderRight(2).BorderBottom(isLast ? 2 : (showResultColumns ? 0.5f : 0)).BorderColor(Colors.Black).Padding(2).Text(participant.Team.Name);

                if (showResultColumns)
                {
                    table.Cell().BorderRight(2).BorderBottom(isLast ? 2 : 0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text(insertOutcomes ? $"{participant.Statistics.Points}" : string.Empty);
                    table.Cell().BorderRight(2).BorderBottom(isLast ? 2 : 0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text(insertOutcomes ? $"{participant.Statistics.ScoreFor} : {participant.Statistics.ScoreAgainst}" : ":");
                    table.Cell().BorderRight(2).BorderBottom(isLast ? 2 : 0.5f).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text(insertOutcomes ? $"{participant.Statistics.ScoreDifference}" : string.Empty).FontColor(participant.Statistics.ScoreDifference < 0 ? Colors.Red.Medium : Colors.Black);
                }
            }
        });
    }
}
