namespace Turnierplan.App.Models;

public sealed record UserDto
{
    public required Guid Id { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string UserName { get; init; }

    public string? FullName { get; init; }

    public string? EMail { get; init; }

    public required DateTime LastPasswordChange { get; init; }

    public required bool IsAdministrator { get; init; }
}
