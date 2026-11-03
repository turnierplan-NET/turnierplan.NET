namespace Turnierplan.Core;

public sealed class TournamentException : Exception
{
    public TournamentException(string? message)
        : base(message)
    {
    }
}
