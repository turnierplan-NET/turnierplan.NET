using Turnierplan.Core.SeedWork;

namespace Turnierplan.Core.Document;

public interface IDocumentRepository : IRepositoryWithPublicId<Document, long>
{
    Task<Document?> GetByPublicIdAsync(PublicId.PublicId id, bool includeTournamentDetails = false);
}
