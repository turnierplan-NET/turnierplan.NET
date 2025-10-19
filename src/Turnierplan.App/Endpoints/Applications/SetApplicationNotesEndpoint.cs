using FluentValidation;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationNotesEndpoint : PatchApplicationEndpointBase<SetApplicationNotesEndpoint.SetApplicationNotesEndpointRequest>
{
    protected override string RouteSuffix => "notes";

    protected override IValidator<SetApplicationNotesEndpointRequest> RequestValidator => Validator.Instance;

    protected override void UpdateApplication(Application application, SetApplicationNotesEndpointRequest request)
    {
        application.Notes = request.Notes;
    }

    public sealed record SetApplicationNotesEndpointRequest
    {
        public required string Notes { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetApplicationNotesEndpointRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(x => x.Notes)
                .NotNull();
        }
    }
}
