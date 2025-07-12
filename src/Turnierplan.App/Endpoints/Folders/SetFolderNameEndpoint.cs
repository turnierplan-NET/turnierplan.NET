using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Folder;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Folders;

internal sealed class SetFolderNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/folders/{id}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] SetFolderNameEndpointRequest request,
        IFolderRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var folder = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (folder is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(folder, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        folder.Name = request.Name.Trim();

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetFolderNameEndpointRequest
    {
        public required string Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetFolderNameEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
