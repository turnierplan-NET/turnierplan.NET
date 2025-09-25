using System.Text.Json.Serialization;

namespace Turnierplan.App.Models;

public sealed record FolderStatisticsOutcomeDto
{
    internal FolderStatisticsOutcomeDto(int scoreA, int scoreB)
    {
        if (scoreA < scoreB)
        {
            throw new ArgumentException($"{nameof(scoreA)} must be greater than or equal to {nameof(scoreB)}.");
        }

        ScoreA = scoreA;
        ScoreB = scoreB;
        Difference = scoreA - scoreB;
    }

    public int ScoreA { get; }

    public int ScoreB { get; }

    [JsonIgnore]
    internal int Difference { get; }
}
