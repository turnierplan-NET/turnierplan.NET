using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Helpers;
using Turnierplan.Core.User;

namespace Turnierplan.App.Endpoints.Users;

internal sealed class DeleteUserEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Delete;

    protected override string Route => "/api/users/{id}";

    protected override Delegate Handler => Handle;

    protected override bool? RequireAdministrator => true;

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        IUserRepository repository,
        IDeletionHelper deletionHelper,
        CancellationToken cancellationToken)
    {
        if (id == context.GetCurrentUserIdOrThrow())
        {
            return Results.BadRequest("You cannot delete yourself.");
        }

        var user = await repository.GetByIdAsync(id, true).ConfigureAwait(false);

        if (user is null)
        {
            return Results.NotFound();
        }

        var result = await deletionHelper.DeleteUserAsync(user, cancellationToken).ConfigureAwait(false);

        if (!result)
        {
            return Results.InternalServerError("The deletion of the user failed. Please check the application logs for details.");
        }

        return Results.NoContent();
    }
}
