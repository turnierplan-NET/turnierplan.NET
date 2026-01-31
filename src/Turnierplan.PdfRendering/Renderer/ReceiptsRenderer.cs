using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Turnierplan.Core.Tournament;
using Turnierplan.ImageStorage;
using Turnierplan.Localization;
using Turnierplan.PdfRendering.Configuration;
using Turnierplan.PdfRendering.Extensions;
using Turnierplan.PdfRendering.Tracing;

namespace Turnierplan.PdfRendering.Renderer;

[DocumentRenderer]
public sealed partial class ReceiptsRenderer(IImageStorage imageStorage) : DocumentRendererBase<ReceiptsDocumentConfiguration>
{
    protected override Document Render(Tournament tournament, ReceiptsDocumentConfiguration configuration, ILocalization localization)
    {
        var signatureDate = tournament.Matches.OrderBy(x => x.Index).FirstOrDefault()?.Kickoff ?? DateTime.UtcNow;
        var entries = GenerateReceipts(tournament, configuration);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Content().Column(baseColumn =>
                {
                    foreach (var entry in entries)
                    {
                        baseColumn.Item().Extend().Border(2).Padding(6).Column(column =>
                        {
                            if (configuration.ShowPrimaryLogo && tournament.PrimaryLogo is not null)
                            {
                                using var _ = DocumentRendererActivitySource.LoadRemoteImage(CurrentActivity, tournament.PrimaryLogo, nameof(tournament.PrimaryLogo));
                                column.Item().Unconstrained().Width(1.7f, Unit.Centimetre).Image(tournament.PrimaryLogo, imageStorage);
                            }

                            if (configuration.ShowSecondaryLogo && tournament.SecondaryLogo is not null)
                            {
                                using var _ = DocumentRendererActivitySource.LoadRemoteImage(CurrentActivity, tournament.SecondaryLogo, nameof(tournament.SecondaryLogo));
                                column.Item().AlignRight().Unconstrained().TranslateX(-1.7f, Unit.Centimetre).Width(1.7f, Unit.Centimetre).Image(tournament.SecondaryLogo, imageStorage);
                            }

                            column.Item().PaddingVertical(10).MinHeight(12, Unit.Millimetre).AlignMiddle().Column(headerColumn =>
                            {
                                if (!string.IsNullOrWhiteSpace(configuration.HeaderInfo))
                                {
                                    headerColumn.Item().AlignCenter().Text(configuration.HeaderInfo);
                                }

                                headerColumn.Item().AlignCenter().Text(tournament.Name);
                            });

                            column.Item().LineHorizontal(1);

                            column.Item().PaddingTop(15, Unit.Millimetre).AlignCenter().Text(entry.TeamName).FontSize(14);

                            column.Item().PaddingTop(2, Unit.Millimetre).AlignCenter().Text(text =>
                            {
                                text.Span(entry.TeamCount > 1
                                    ? localization.Get("Documents.Receipts.CombinedTeams", entry.TeamCount)
                                    : string.Empty).FontSize(7);
                            });

                            column.Item().PaddingTop(10, Unit.Millimetre).AlignCenter().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span(localization.Get("Documents.Receipts.Line1"));
                                text.Line(localization.Get("Documents.Receipts.Amount", entry.Amount, configuration.Currency)).Underline();
                                text.Line(localization.Get("Documents.Receipts.Line2"));
                            });

                            var signatureDateLocation = string.IsNullOrWhiteSpace(configuration.SignatureLocation)
                                ? localization.Get("Documents.Receipts.SignatureDate", signatureDate)
                                : localization.Get("Documents.Receipts.SignatureDateLocation", configuration.SignatureLocation, signatureDate);
                            column.Item().PaddingTop(5, Unit.Millimetre).AlignCenter().Text(signatureDateLocation);

                            column.Item().PaddingHorizontal(4, Unit.Millimetre).PaddingTop(17, Unit.Millimetre).Text(localization.Get("Documents.Receipts.Signature")).Underline();

                            column.Item().PaddingHorizontal(4, Unit.Millimetre).Height(17, Unit.Millimetre).Row(row =>
                            {
                                var signatureInfo = string.IsNullOrWhiteSpace(configuration.SignatureRecipient)
                                    ? localization.Get("Documents.Receipts.SignatureInfo")
                                    : configuration.SignatureRecipient;

                                row.RelativeItem(0.5f).AlignBottom().BorderTop(0.75f).Text(signatureInfo).FontSize(6);
                                row.RelativeItem(0.5f);
                            });
                        });
                    }
                });
            });
        }).WithTurnierplanMetadata(localization);
    }

    internal static ReceiptEntry[] GenerateReceipts(Tournament tournament, ReceiptsDocumentConfiguration configuration)
    {
        ReceiptEntry[] entries;

        if (configuration.CombineSimilarTeams)
        {
            entries = tournament.Teams
                .GroupBy(team =>
                {
                    var input = team.Name.Trim();
                    var match = TeamGroupingRegex().Match(input);
                    return match.Success ? match.Groups["BaseName"].Value : input;
                })
                .Select(group =>
                {
                    var teamCount = group.Count();

                    return new ReceiptEntry
                    {
                        TeamName = teamCount == 1 ? group.Single().Name.Trim() : group.Key,
                        TeamCount = teamCount,
                        Amount = configuration.GetAmountEntryForTeamCount(teamCount).Amount * teamCount
                    };
                }).ToArray();
        }
        else
        {
            var amountEntry = configuration.GetAmountEntryForTeamCount(1);

            entries = tournament.Teams.Select(team => new ReceiptEntry
            {
                TeamName = team.Name.Trim(),
                TeamCount = 1,
                Amount = amountEntry.Amount
            }).ToArray();
        }

        return entries;
    }

    [GeneratedRegex(@"^(?<BaseName>.*?)\s(\d{1,3}|I|II|III|IV|V|VI|VII|VIII|IX|X)$")]
    private static partial Regex TeamGroupingRegex();

    internal sealed record ReceiptEntry
    {
        public required string TeamName { get; init; }

        public required int TeamCount { get; init; }

        public required double Amount { get; init; }
    }
}
