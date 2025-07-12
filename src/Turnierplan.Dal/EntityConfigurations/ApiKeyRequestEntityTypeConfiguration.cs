using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ApiKey;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ApiKeyRequestEntityTypeConfiguration : IEntityTypeConfiguration<ApiKeyRequest>
{
    public void Configure(EntityTypeBuilder<ApiKeyRequest> builder)
    {
        builder.ToTable("ApiKeyRequests", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.HasIndex(x => x.Timestamp);

        builder.Property(x => x.Path)
            .IsRequired();
    }
}
