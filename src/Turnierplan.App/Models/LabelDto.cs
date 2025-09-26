namespace Turnierplan.App.Models;

public sealed class LabelDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string ColorCode { get; init; }
}
