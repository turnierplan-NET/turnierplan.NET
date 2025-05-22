using Turnierplan.App.Models;
using Turnierplan.Core.Document;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class DocumentMappingRule : MappingRuleBase<Document, DocumentDto>
{
    protected override DocumentDto Map(IMapper mapper, MappingContext context, Document source)
    {
        return new DocumentDto
        {
            Id = source.PublicId,
            TournamentId = source.Tournament.PublicId,
            Type = source.Type,
            Name = source.Name,
            LastModifiedAt = source.LastModifiedAt,
            GenerationCount = source.GenerationCount
        };
    }
}
