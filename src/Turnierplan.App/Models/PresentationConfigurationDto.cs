using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Models;

public sealed record PresentationConfigurationDto
{
    public required HeaderLine Header1 { get; init; }

    public required HeaderLine Header2 { get; init; }

    public required PresentationConfiguration.ResultsMode ShowResults { get; init; }

    public required bool ShowPrimaryLogo { get; init; }

    public required bool ShowSecondaryLogo { get; init; }

    public sealed record HeaderLine
    {
        public required PresentationConfiguration.HeaderLineContent Content { get; init; }

        public string? CustomContent { get; init; }
    }
}
