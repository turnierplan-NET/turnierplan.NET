using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class CreateApplicationEndpoint : EndpointBase<ApplicationDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromBody] CreateApplicationEndpointRequest request,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Include.TournamentClasses).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ManageApplications))
        {
            return Results.Forbid();
        }

        var application = planningRealm.AddApplication(null, request.Contact);

        application.Notes = request.Notes;
        application.ContactEmail = request.ContactEmail;
        application.ContactTelephone = request.ContactTelephone;

        foreach (var entry in request.Entries)
        {
            var tournamentClass = planningRealm.TournamentClasses.FirstOrDefault(x => x.Id == entry.TournamentClassId);

            if (tournamentClass is null)
            {
                return Results.NotFound();
            }

            for (var i = 0; i < entry.NumberOfTeams; i++)
            {
                var name = $"{request.Name} {i + 1}";
                application.AddTeam(tournamentClass, name);
            }
        }

        await planningRealmRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<ApplicationDto>(application));
    }

    public sealed record CreateApplicationEndpointRequest
    {
        public required string Name { get; init; }

        public required string Notes { get; init; }

        public required string Contact { get; init; }

        public string? ContactEmail { get; init; }

        public string? ContactTelephone { get; init; }

        public required CreateApplicationEndpointRequestEntry[] Entries { get; init; }
    }

    public sealed record CreateApplicationEndpointRequestEntry
    {
        public required long TournamentClassId { get; init; }

        public required int NumberOfTeams { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateApplicationEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Notes)
                .NotNull(); // notes can be empty

            RuleFor(x => x.Contact)
                .NotEmpty();

            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .When(x => x.ContactEmail is not null);

            RuleFor(x => x.ContactTelephone)
                .NotEmpty()
                .When(x => x.ContactTelephone is not null);

            RuleFor(x => x.Entries)
                .NotEmpty();

            RuleFor(x => x.Entries)
                .Must(entries =>
                {
                    return entries.Length == entries.DistinctBy(x => x.TournamentClassId).Count();
                })
                .WithMessage("There may only be one entry for each tournament class id.");

            RuleForEach(x => x.Entries)
                .ChildRules(entry =>
                {
                    entry.RuleFor(x => x.TournamentClassId)
                        .GreaterThan(0);

                    entry.RuleFor(x => x.NumberOfTeams)
                        .GreaterThanOrEqualTo(1);
                });
        }
    }
}
