using System.Text.Json.Serialization;

namespace Turnierplan.Core.Configuration;

public sealed record AdditionalPlayoffConfig
{
    public AdditionalPlayoffConfig(int playoffPosition, string teamSelectorA, string teamSelectorB)
    {
        PlayoffPosition = playoffPosition;
        TeamSelectorA = teamSelectorA;
        TeamSelectorB = teamSelectorB;
    }

    public int PlayoffPosition { get; set; }

    public string TeamSelectorA { get; set; }

    public string TeamSelectorB { get; set; }
}
