namespace Turnierplan.Core;

internal sealed class TournamentException : Exception
{
    public TournamentException(string? message)
        : base(message)
    {
    }
}
