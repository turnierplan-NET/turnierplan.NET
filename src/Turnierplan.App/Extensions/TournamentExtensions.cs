using System.Diagnostics.CodeAnalysis;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Extensions;

internal static class TournamentExtensions
{
    /// <summary>
    /// Computes the tournament and returns a boolean value representing success. If the computation failed, the
    /// out parameter is set to an <see cref="IResult"/> which can be returned by the endpoint. The purpose of this
    /// method is to safeguard against saving a tournament to database with an invalid state. If this was the case,
    /// the "get tournament" would always fail because it internally calls <see cref="Tournament.Compute"/>.
    /// </summary>
    public static bool TryCompute(this Tournament tournament, [NotNullWhen(false)] out IResult? result)
    {
        try
        {
            tournament.Compute();
        }
        catch (TurnierplanException ex)
        {
            result = Results.BadRequest($"Changes to match plan result in computation failure: {ex.Message}");
            return false;
        }

        result = null;
        return true;
    }
}
