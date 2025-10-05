using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.ApplicationTeams;

internal sealed class SetApplicationTeamLabelsEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Patch;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications/{applicationId:int}/teams/{applicationTeamId:int}/labels";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromRoute] long applicationId,
        [FromRoute] long applicationTeamId,
        [FromBody] SetApplicationTeamLabelsEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.ApplicationsWithTeams | IPlanningRealmRepository.Includes.Labels);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ApplicationsWrite))
        {
            return Results.Forbid();
        }

        var application = planningRealm.Applications.FirstOrDefault(x => x.Id == applicationId);

        if (application is null)
        {
            return Results.NotFound();
        }

        var applicationTeam = application.Teams.FirstOrDefault(x => x.Id == applicationTeamId);

        if (applicationTeam is null)
        {
            return Results.NotFound();
        }

        var labelsToAdd = request.LabelIds.Where(id => !applicationTeam.Labels.Any(label => label.Id == id)).ToList();
        var labelsToRemove = applicationTeam.Labels.Where(label => !request.LabelIds.Contains(label.Id)).ToList();

        foreach (var label in labelsToRemove)
        {
            applicationTeam.RemoveLabel(label);
        }

        foreach (var labelId in labelsToAdd)
        {
            var label = planningRealm.Labels.FirstOrDefault(x => x.Id == labelId);

            if (label is null)
            {
                return Results.NotFound();
            }

            applicationTeam.AddLabel(label);
        }

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record SetApplicationTeamLabelsEndpointRequest
    {
        public required long[] LabelIds { get; init; }
    }

    private sealed class Validator : AbstractValidator<SetApplicationTeamLabelsEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleForEach(x => x.LabelIds)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.LabelIds)
                .Must(x => x.Distinct().Count() == x.Length)
                .WithMessage($"{nameof(SetApplicationTeamLabelsEndpointRequest.LabelIds)} must not contain duplicate values.");
        }
    }
}
