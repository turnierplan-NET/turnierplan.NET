namespace Turnierplan.Adapter;

/// <summary>
/// An error that occurred in the <see cref="TurnierplanClient"/>.
/// </summary>
public sealed class TurnierplanClientException : Exception
{
    internal TurnierplanClientException(string? message)
        : base(message)
    {
    }
}
