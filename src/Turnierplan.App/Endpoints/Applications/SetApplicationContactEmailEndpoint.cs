using FluentValidation;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationContactEmailEndpoint : PatchApplicationEndpointBase<SetApplicationContactEmailEndpoint.SetApplicationContactEmailEndpointRequest>
{
    protected override string RouteSuffix => "contact-email";

    protected override IValidator<SetApplicationContactEmailEndpointRequest> RequestValidator => Validator.Instance;

    protected override void UpdateApplication(Application application, SetApplicationContactEmailEndpointRequest request)
    {
        application.ContactEmail = request.ContactEmail;
    }

    public sealed record SetApplicationContactEmailEndpointRequest
    {
        public string? ContactEmail { get; init; }
    }

    internal sealed class Validator : AbstractValidator<SetApplicationContactEmailEndpointRequest>
    {
        public static readonly Validator Instance = new();

        public Validator()
        {
            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .When(x => x.ContactEmail is not null);
        }
    }
}
