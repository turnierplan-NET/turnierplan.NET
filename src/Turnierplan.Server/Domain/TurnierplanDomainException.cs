namespace Turnierplan.Server.Domain;

internal sealed class TurnierplanDomainException : Exception
{
    public TurnierplanDomainException(string? message)
        : base(message)
    {
    }

    public TurnierplanDomainException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
