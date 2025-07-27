namespace Turnierplan.App.Extensions;

internal static class EnumerableExtensions
{
    public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) // TODO: Add unit test
    {
        return !enumerable.Any(predicate);
    }
}
