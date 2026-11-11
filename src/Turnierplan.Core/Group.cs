namespace Turnierplan.Core;

public sealed class Group
{
    internal readonly List<GroupParticipant> _participants = [];
    internal Tournament? _tournament;

    internal Group(char alphabeticalId, string? displayName = null)
    {
        AlphabeticalId = alphabeticalId;
        DisplayName = displayName;
    }

    public char AlphabeticalId { get; }

    public string? DisplayName { get; set; }

    public IReadOnlyList<GroupParticipant> Participants => _participants.AsReadOnly();

    public void ClearParticipants() => _participants.Clear();
}
