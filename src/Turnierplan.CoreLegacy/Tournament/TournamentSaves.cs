namespace Turnierplan.Core.Tournament;

internal sealed class TournamentSaves
{
    

    public void SetMatchOrder(Dictionary<int, int> matchIndices)
    {
        if (!matchIndices.Keys.ToHashSet().SetEquals(Matches.Select(x => x.Id)))
        {
            throw new TournamentException("The given match IDs must equal the existing match IDs.");
        }

        if (matchIndices.Values.Distinct().Count() != matchIndices.Count)
        {
            throw new TournamentException("The match indices must be unique.");
        }

        if (!matchIndices.Values.IsSequential(1))
        {
            throw new TournamentException("The match indices must be sequential, starting from 1.");
        }

        foreach (var match in Matches)
        {
            match.Index = matchIndices[match.Id];
        }
    }
}
