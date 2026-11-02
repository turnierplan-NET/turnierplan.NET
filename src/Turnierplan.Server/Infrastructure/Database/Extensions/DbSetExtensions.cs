using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Infrastructure.Database.Extensions;

internal static class DbSetExtensions
{
    public static async Task<T?> FindByPublicIdAsync<T>(this DbSet<T> set, PublicId id, CancellationToken cancellationToken = default)
        where T : EntityBase<long>, IEntityWithPublicId
    {
        return await set.Where(x => x.PublicId == id).FirstOrDefaultAsync(cancellationToken);
    }
}
