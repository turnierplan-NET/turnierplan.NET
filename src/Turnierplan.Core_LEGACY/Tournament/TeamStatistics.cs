namespace Turnierplan.Core.Tournament;

public sealed record TeamStatistics
{
    public int ScoreFor { get; private set; }

    public int ScoreAgainst { get; private set; }

    public int MatchesWon { get; private set; }

    public int MatchesDrawn { get; private set; }

    public int MatchesLost { get; private set; }

    public int MatchesPlayed => MatchesWon + MatchesDrawn + MatchesLost;

    internal void Reset()
    {
        ScoreFor = 0;
        ScoreAgainst = 0;
        MatchesWon = 0;
        MatchesDrawn = 0;
        MatchesLost = 0;
    }

    internal void AddMatchOutcome(int scoreFor, int scoreAgainst, bool ignoreScoreCounter, ComputationConfiguration computationConfiguration)
    {
        if (!ignoreScoreCounter)
        {
            ScoreFor += scoreFor;
            ScoreAgainst += scoreAgainst;
        }

        if (scoreFor == scoreAgainst)
        {
            MatchesDrawn++;

            return;
        }

        if (computationConfiguration.HigherScoreLoses)
        {
            if (scoreFor > scoreAgainst)
            {
                MatchesLost++;
            }
            else
            {
                MatchesWon++;
            }
        }
        else
        {
            if (scoreFor > scoreAgainst)
            {
                MatchesWon++;
            }
            else
            {
                MatchesLost++;
            }
        }
    }
}
