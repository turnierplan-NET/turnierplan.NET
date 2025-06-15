using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Tournament;

public sealed class Group : Entity<int>
{
    internal readonly List<GroupParticipant> _participants = [];

    internal Group(int id, char alphabeticalId, string? displayName = null)
    {
        Id = id;
        AlphabeticalId = alphabeticalId;
        DisplayName = displayName;
    }

    public override int Id { get; protected set; }

    public char AlphabeticalId { get; }

    public string? DisplayName { get; set; }

    public IReadOnlyList<GroupParticipant> Participants => _participants.AsReadOnly();

    public void ClearParticipants() => _participants.Clear();
}
