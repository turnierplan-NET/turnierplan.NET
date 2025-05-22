using System.Numerics;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Extensions;

internal static class EnumerableExtensions
{
    public static TeamGroupStatistics Combine(this IEnumerable<TeamGroupStatistics?> source)
    {
        var result = new TeamGroupStatistics();

        foreach (var sourceEntry in source)
        {
            if (sourceEntry is null)
            {
                continue;
            }

            result.ScoreFor += sourceEntry.ScoreFor;
            result.ScoreAgainst += sourceEntry.ScoreAgainst;
            result.MatchesWon += sourceEntry.MatchesWon;
            result.MatchesDrawn += sourceEntry.MatchesDrawn;
            result.MatchesLost += sourceEntry.MatchesLost;
            result.Points += sourceEntry.Points;
        }

        return result;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(x => x is not null)!;
    }

    public static bool IsSequential<T>(this IEnumerable<T> source, T first)
        where T : IIncrementOperators<T>, IEquatable<T>
    {
        var orderedItems = source.Order().ToList();

        var expectedValue = first;

        foreach (var item in orderedItems)
        {
            if (!item.Equals(expectedValue))
            {
                return false;
            }

            expectedValue++;
        }

        return true;
    }
}
