using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Server.Domain.ApiKeys;

namespace Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

internal sealed class ApiKeyRequestEntityTypeConfiguration : IEntityTypeConfiguration<ApiKeyRequest>
{
    public void Configure(EntityTypeBuilder<ApiKeyRequest> builder)
    {
        builder.ToTable("api_key_request", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        // TODO: index on timestamp?

        builder.Property(x => x.Path)
            .IsRequired();
    }
}
