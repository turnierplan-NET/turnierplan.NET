namespace Turnierplan.PdfRendering.Configuration;

public sealed record ReceiptsDocumentConfiguration : IDocumentConfiguration
{
    private int[]? _cachedAmountKeys;

    /// <summary>
    /// If <see cref="CombineSimilarTeams"/> is <c>false</c>, the dictionary entry with the key <c>1</c> will be used for
    /// each team. If <see cref="CombineSimilarTeams"/> is <c>true</c>, the dictionary entry with the largest key that is
    /// equal to or smaller than <c>n</c> will be used for each grouping of teams of size <c>n</c>. An entry with key
    /// <c>1</c> is always required.
    /// </summary>
    public Dictionary<int, AmountEntry> Amounts { get; set; } = new() { { 1, new AmountEntry() } };

    public string Currency { get; init; } = "\u20AC"; // "Euro" Sign

    public string HeaderInfo { get; init; } = string.Empty;

    public string SignatureLocation { get; init; } = string.Empty;

    public string SignatureRecipient { get; init; } = string.Empty;

    public bool ShowPrimaryLogo { get; init; } = true;

    public bool ShowSecondaryLogo { get; init; } = false;

    public bool CombineSimilarTeams { get; init; } = false;

    public sealed record AmountEntry
    {
        public double Amount { get; init; } = 10;
    }

    internal AmountEntry GetAmountEntryForTeamCount(int teamCount)
    {
        if (teamCount == 1)
        {
            // Amount for one team must always be given
            return Amounts[1];
        }

        if (Amounts.TryGetValue(teamCount, out var result))
        {
            return result;
        }

        _cachedAmountKeys ??= Amounts.Keys.Order().ToArray();

        for (var i = _cachedAmountKeys.Length - 1; i >= 0; i--)
        {
            if (_cachedAmountKeys[i] <= teamCount)
            {
                return Amounts[_cachedAmountKeys[i]];
            }
        }

        return Amounts[1];
    }
}
