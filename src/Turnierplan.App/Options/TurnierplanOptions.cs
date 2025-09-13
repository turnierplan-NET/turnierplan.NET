namespace Turnierplan.App.Options;

internal sealed record TurnierplanOptions
{
    public string? ApplicationUrl { get; init; }

    public string? InstanceName { get; init; }

    public string? LogoUrl { get; init; }

    public string? ImprintUrl { get; init; }

    public string? PrivacyUrl { get; init; }

    public string? InitialUserName { get; init; }

    public string? InitialUserEmail { get; init; }

    public string? InitialUserPassword { get; init; }
}
