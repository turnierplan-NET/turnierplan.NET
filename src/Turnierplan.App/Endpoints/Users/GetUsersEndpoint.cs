using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.Core.User;

namespace Turnierplan.App.Endpoints.Users;

internal sealed class GetUsersEndpoint : EndpointBase<IEnumerable<UserDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/users";

    protected override Delegate Handler => Handle;

    protected override bool? RequireAdministrator => true;

    private static async Task<IResult> Handle(
        IUserRepository repository,
        IMapper mapper)
    {
        var users = await repository.GetAllUsers().ConfigureAwait(false);

        return Results.Ok(mapper.MapCollection<UserDto>(users));
    }
}
