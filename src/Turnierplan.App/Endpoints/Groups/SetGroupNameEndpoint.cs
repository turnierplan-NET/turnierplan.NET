using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal;

namespace Turnierplan.App.Endpoints.Groups;

internal sealed class SetGroupNameEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/tournaments/{tournamentId}/groups/{groupId:int}/name";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId tournamentId,
        [FromRoute] int groupId,
        [FromBody] SetGroupNameEndpointRequest request,
        ITournamentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var tournament = await repository.GetByPublicIdAsync(tournamentId, ITournamentRepository.Include.Groups).ConfigureAwait(false);

        if (tournament is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(tournament, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        var group = tournament.Groups.FirstOrDefault(x => x.Id == groupId);

        if (group is null)
        {
            return Results.NotFound();
        }

        group.DisplayName = request.Name?.Trim();

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record SetGroupNameEndpointRequest
    {
        public required string? Name { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetGroupNameEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ValidationConstants.Group.MaxDisplayNameLength)
                .When(x => x.Name is not null);
        }
    }
}
