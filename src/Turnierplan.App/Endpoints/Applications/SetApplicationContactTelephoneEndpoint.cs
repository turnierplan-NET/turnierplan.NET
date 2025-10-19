using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationContactTelephoneEndpoint : PatchApplicationEndpointBase<SetApplicationContactTelephoneEndpoint.SetApplicationContactTelephoneEndpointRequest>
{
    protected override string RouteSuffix => "contact-telephone";

    protected override void UpdateApplication(Application application, SetApplicationContactTelephoneEndpointRequest request)
    {
        application.ContactTelephone = request.ContactTelephone;
    }

    public sealed record SetApplicationContactTelephoneEndpointRequest
    {
        public required string? ContactTelephone { get; init; }
    }
}