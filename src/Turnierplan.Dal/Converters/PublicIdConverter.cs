using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Turnierplan.Core.PublicId;

namespace Turnierplan.Dal.Converters;

internal sealed class PublicIdConverter() : ValueConverter<PublicId, long>(publicId => publicId.ToSignedInt64(), input => new PublicId(input));
