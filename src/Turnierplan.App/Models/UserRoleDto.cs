namespace Turnierplan.App.Models;

public sealed record UserRoleDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}
