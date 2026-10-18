namespace Turnierplan.Server.Application.Projects.GetProjects;

internal sealed class GetProjectsResponse
{
    public required GetProjectsResponseItem[] Projects { get; init; }
}
