using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Infrastructure.Database.Converters;

internal sealed class PublicIdConverter() : ValueConverter<PublicId, long>(publicId => publicId.ToSignedInt64(), input => new PublicId(input));
