using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationContactEmailEndpoint : PatchApplicationEndpointBase<SetApplicationContactEmailEndpoint.SetApplicationContactEmailEndpointRequest>
{
    protected override string RouteSuffix => "contact-email";

    protected override void UpdateApplication(Application application, SetApplicationContactEmailEndpointRequest request)
    {
        application.ContactEmail = request.ContactEmail;
    }

    public sealed record SetApplicationContactEmailEndpointRequest
    {
        public required string? ContactEmail { get; init; }
    }
}