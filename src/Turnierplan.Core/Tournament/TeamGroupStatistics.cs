namespace Turnierplan.Core.Tournament;

public sealed record TeamGroupStatistics
{
    public int Position { get; internal set; }

    public int ScoreFor { get; internal set; }

    public int ScoreAgainst { get; internal set; }

    public int MatchesWon { get; internal set; }

    public int MatchesDrawn { get; internal set; }

    public int MatchesLost { get; internal set; }

    public int Points { get; internal set; }

    public int ScoreDifference => ScoreFor - ScoreAgainst;

    public int MatchesPlayed => MatchesWon + MatchesDrawn + MatchesLost;

    internal void Reset()
    {
        Position = 0;
        ScoreFor = 0;
        ScoreAgainst = 0;
        MatchesWon = 0;
        MatchesDrawn = 0;
        MatchesLost = 0;
        Points = 0;
    }

    internal void AddMatchOutcome(int scoreFor, int scoreAgainst, ComputationConfiguration computationConfiguration)
    {
        ScoreFor += scoreFor;
        ScoreAgainst += scoreAgainst;

        if (scoreFor > scoreAgainst)
        {
            MatchesWon++;
            Points += computationConfiguration.MatchWonPoints;
        }
        else if (scoreFor == scoreAgainst)
        {
            MatchesDrawn++;
            Points += computationConfiguration.MatchDrawnPoints;
        }
        else
        {
            MatchesLost++;
            Points += computationConfiguration.MatchLostPoints;
        }
    }
}
