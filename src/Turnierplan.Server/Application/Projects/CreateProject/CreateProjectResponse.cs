using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Projects.CreateProject;

internal sealed record CreateProjectResponse
{
    public required PublicId Id { get; init; }

    public required string ScopeId { get; init; }

    public required string Name { get; init; }
}
