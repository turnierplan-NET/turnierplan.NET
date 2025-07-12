using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Planning;

public sealed class Application : Entity<long>
{
    internal readonly List<ApplicationTeam> _teams = new();

    internal Application(long id, int tag, DateTime createdAt, string name, string? email, string? telephone, string? commentFromSender, string notes)
    {
        Id = id;
        Tag = tag;
        CreatedAt = createdAt;
        Name = name;
        Email = email;
        Telephone = telephone;
        CommentFromSender = commentFromSender;
        Notes = notes;
    }

    public override long Id { get; protected set; }

    public InvitationLink? SourceLink { get; internal set; }

    public int Tag { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? CommentFromSender { get; set; }

    public string Notes { get; set; }

    public IReadOnlyList<ApplicationTeam> Teams => _teams.AsReadOnly();
}
