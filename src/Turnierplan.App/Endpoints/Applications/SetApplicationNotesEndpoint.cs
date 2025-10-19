using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class SetApplicationNotesEndpoint : PatchApplicationEndpointBase<SetApplicationNotesEndpoint.SetApplicationNotesEndpointRequest>
{
    protected override string RouteSuffix => "notes";

    protected override void UpdateApplication(Application application, SetApplicationNotesEndpointRequest request)
    {
        application.Notes = request.Notes;
    }

    public sealed record SetApplicationNotesEndpointRequest
    {
        public required string Notes { get; init; }
    }
}
