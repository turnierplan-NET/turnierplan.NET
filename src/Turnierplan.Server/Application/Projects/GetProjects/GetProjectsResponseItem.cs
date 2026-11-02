using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Projects.GetProjects;

internal sealed class GetProjectsResponseItem
{
    public required PublicId Id { get; init; }

    public required string Scope { get; init; }

    public required string Name { get; init; }
}
