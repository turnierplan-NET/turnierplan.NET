using Turnierplan.App.Models;
using Turnierplan.Core.TournamentPlanner;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class ApplicationChangeLogMappingRule : MappingRuleBase<ApplicationChangeLog, ApplicationChangeLogDto>
{
    protected override ApplicationChangeLogDto Map(IMapper mapper, MappingContext context, ApplicationChangeLog source)
    {
        return new ApplicationChangeLogDto
        {
            Id = source.Id,
            Timestamp = source.Timestamp,
            Type = source.Type,
            Properties = source.Properties.Select(x => new ApplicationChangeLogPropertyDto
            {
                Type = x.Type,
                Value = x.Value
            }).ToArray()
        };
    }
}
