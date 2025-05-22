using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Models;

public sealed record ComputationConfigurationDto
{
    public required int MatchWonPoints { get; init; }

    public required int MatchDrawnPoints { get; init; }

    public required int MatchLostPoints { get; init; }

    public required TeamComparisonMode[] ComparisonModes { get; init; }
}
