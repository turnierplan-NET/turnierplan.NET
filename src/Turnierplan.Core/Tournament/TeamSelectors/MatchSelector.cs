namespace Turnierplan.Core.Tournament.TeamSelectors;

public sealed record MatchSelector(int TargetMatchIndex, MatchSelector.Mode SelectionMode) : TeamSelectorBase
{
    public override Team? GetTargetTeam(Tournament tournament)
    {
       var match = tournament._matches.FirstOrDefault(x => x.Index == TargetMatchIndex);

        if (match is null || !match.IsFinished || match.ScoreA is null || match.ScoreB is null)
        {
            return null;
        }

        if (match.ScoreA > match.ScoreB)
        {
            return SelectionMode == Mode.Winner ? match.TeamA : match.TeamB;
        }

        if (match.ScoreA < match.ScoreB)
        {
            return SelectionMode == Mode.Winner ? match.TeamB : match.TeamA;
        }

        return null;
    }

    public enum Mode
    {
        Winner,
        Loser
    }
}
