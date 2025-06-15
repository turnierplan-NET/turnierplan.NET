using Turnierplan.App.Models;
using Turnierplan.Core.User;

namespace Turnierplan.App.Mapping.Rules;

internal sealed class UserMappingRule : MappingRuleBase<User, UserDto>
{
    protected override UserDto Map(IMapper mapper, MappingContext context, User source)
    {
        return new UserDto
        {
            Id = source.Id,
            CreatedAt = source.CreatedAt,
            Name = source.Name,
            EMail = source.EMail,
            LastPasswordChange = source.LastPasswordChange,
            IsAdministrator = source.IsAdministrator
        };
    }
}
