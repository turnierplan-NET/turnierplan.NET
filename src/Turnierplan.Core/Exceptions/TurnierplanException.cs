namespace Turnierplan.Core.Exceptions;

public sealed class TurnierplanException : Exception
{
    public TurnierplanException(string? message)
        : base(message)
    {
    }

    public TurnierplanException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
