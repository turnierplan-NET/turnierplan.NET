using Turnierplan.App.Models;
using Turnierplan.Core.Tournament.Definitions;

namespace Turnierplan.App.Endpoints.Metadata;

internal sealed class GetAvailableFinalsMatchDefinitionsEndpoint : EndpointBase<IEnumerable<FinalsMatchDefinitionDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/metadata/finals-match-definitions";

    protected override Delegate Handler => Handle;

    private static IResult Handle()
    {
        var source = MatchPlanDefinitions.GetAllFinalsMatchDefinitions();

        var definitions = source.Select(x => new FinalsMatchDefinitionDto
        {
            GroupCount = x.GroupCount,
            MatchCount = x.MatchCount,
            RequiredTeamsPerGroup = x.Definition.RequiredTeamsPerGroup
        });

        return Results.Ok(definitions);
    }
}
