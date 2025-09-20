namespace Turnierplan.Adapter;

/// <summary>
/// Represents an error that occurred in the <see cref="TurnierplanClient"/> API abstraction layer.
/// </summary>
public sealed class TurnierplanClientException : Exception
{
    internal TurnierplanClientException(string? message)
        : base(message)
    {
    }
}
