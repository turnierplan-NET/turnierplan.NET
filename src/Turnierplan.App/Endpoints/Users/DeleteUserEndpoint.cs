using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.Core.Extensions;
using Turnierplan.Dal.Repositories;

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
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        if (id == context.GetCurrentUserIdOrThrow())
        {
            return Results.BadRequest("You cannot delete yourself.");
        }

        var user = await repository.GetByIdAsync(id);

        if (user is null)
        {
            return Results.NotFound();
        }

        repository.Remove(user);

        var principal = user.AsPrincipal();

        foreach (var roleAssignmentRepository in serviceProvider.GetServices<IRoleAssignmentRepository>())
        {
            await roleAssignmentRepository.RemoveAllByPrincipalAsync(principal);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
