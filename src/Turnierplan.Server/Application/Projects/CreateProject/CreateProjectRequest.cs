using Turnierplan.Server.Domain;

namespace Turnierplan.Server.Application.Projects.CreateProject;

internal sealed class CreateProjectRequest
{
    public required PublicId OrganizationId { get; init; }

    public required string Name { get; init; }
}
