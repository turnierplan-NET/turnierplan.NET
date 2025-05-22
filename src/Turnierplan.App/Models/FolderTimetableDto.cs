using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Models;

public sealed record FolderTimetableDto
{
    public required PublicId FolderId { get; init; }

    public required PublicId OrganizationId { get; init; }

    public required string FolderName { get; init; }

    public required FolderTimetableTournamentEntry[] Tournaments { get; init; }

    public sealed record FolderTimetableTournamentEntry
    {
        public required PublicId Id { get; init; }

        public required string Name { get; init; }

        public required DateTime? StartDate { get; init; }

        public required DateTime? EndDate { get; init; }
    }
}
