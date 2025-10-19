using FluentValidation;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationContactEndpoint : PatchApplicationEndpointBase<SetApplicationContactEndpoint.SetApplicationContactEndpointRequest>
{
    protected override string RouteSuffix => "contact";

    protected override IValidator<SetApplicationContactEndpointRequest> RequestValidator => Validator.Instance;

    protected override void UpdateApplication(Application application, SetApplicationContactEndpointRequest request)
    {
        application.Contact = request.Contact;
    }

    public sealed record SetApplicationContactEndpointRequest
    {
        public required string Contact { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetApplicationContactEndpointRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(x => x.Contact)
                .NotEmpty();
        }
    }
}
