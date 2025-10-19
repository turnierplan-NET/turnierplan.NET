using FluentValidation;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationContactTelephoneEndpoint : PatchApplicationEndpointBase<SetApplicationContactTelephoneEndpoint.SetApplicationContactTelephoneEndpointRequest>
{
    protected override string RouteSuffix => "contact-telephone";

    protected override IValidator<SetApplicationContactTelephoneEndpointRequest> RequestValidator => Validator.Instance;

    protected override void UpdateApplication(Application application, SetApplicationContactTelephoneEndpointRequest request)
    {
        application.ContactTelephone = request.ContactTelephone;
    }

    public sealed record SetApplicationContactTelephoneEndpointRequest
    {
        public required string? ContactTelephone { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetApplicationContactTelephoneEndpointRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(x => x.ContactTelephone)
                .NotEmpty()
                .When(x => x.ContactTelephone is not null);
        }
    }
}
