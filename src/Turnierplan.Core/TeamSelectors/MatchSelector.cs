namespace Turnierplan.Core.TeamSelectors;

public sealed record MatchSelector(Match TargetMatch, MatchSelector.Mode SelectionMode) : TeamSelectorBase
{
    public override Team? GetTargetTeam()
    {
        if (!TargetMatch.IsFinished || TargetMatch.ScoreA is null || TargetMatch.ScoreB is null)
        {
            return null;
        }

        if (TargetMatch.ScoreA > TargetMatch.ScoreB)
        {
            return SelectionMode == Mode.Winner ? TargetMatch.TeamA : TargetMatch.TeamB;
        }

        if (TargetMatch.ScoreA < TargetMatch.ScoreB)
        {
            return SelectionMode == Mode.Winner ? TargetMatch.TeamB : TargetMatch.TeamA;
        }

        return null;
    }

    public enum Mode
    {
        Winner,
        Loser
    }
}
