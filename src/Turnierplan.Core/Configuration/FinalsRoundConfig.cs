namespace Turnierplan.Core.Configuration;

public sealed record FinalsRoundConfig
{
    public required FinalsRoundOrder FirstFinalsRoundOrder { get; set; }

    public required bool EnableThirdPlacePlayoff { get; set; }

    public List<string>? TeamSelectors { get; set; }

    public List<AdditionalPlayoffConfig>? AdditionalPlayoffs { get; set; }
}
