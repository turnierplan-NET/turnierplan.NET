namespace Turnierplan.Core.Configuration;

public sealed record GroupRoundConfig
{
    public required GroupMatchOrder GroupMatchOrder { get; set; }

    public required int GroupPhaseRounds { get; set; }
}
