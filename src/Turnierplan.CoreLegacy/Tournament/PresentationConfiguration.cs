using System.Text.Json.Serialization;

namespace Turnierplan.Core.Tournament;

public sealed record PresentationConfiguration
{
    [JsonPropertyName("h1")]
    public HeaderLine Header1 { get; set; } = new(HeaderLineContent.TournamentName);

    [JsonPropertyName("h2")]
    public HeaderLine Header2 { get; set; } = new(HeaderLineContent.OrganizerName);

    [JsonPropertyName("o")]
    public ResultsMode ShowResults { get; set; } = ResultsMode.Default;

    [JsonPropertyName("ol")]
    public bool ShowPrimaryLogo { get; set; } = true;

    [JsonPropertyName("sl")]
    public bool ShowSecondaryLogo { get; set; } = true;

    public sealed record HeaderLine
    {
        public HeaderLine()
        {
        }

        internal HeaderLine(HeaderLineContent content)
        {
            Content = content;
        }

        [JsonPropertyName("c")]
        public HeaderLineContent Content { get; set; }

        [JsonPropertyName("cc")]
        public string? CustomContent { get; set; }
    }

    public enum HeaderLineContent
    {
        // Note: Don't change enum values (DB serialization)

        CustomValue = 1,
        OrganizerName = 2,
        TournamentName = 3
    }

    public enum ResultsMode
    {
        // Note: Don't change enum values (DB serialization)

        Default = 1,
        OnlyMatchOutcomes = 2,
        NoResults = 3
    }
}
