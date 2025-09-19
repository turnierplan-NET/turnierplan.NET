namespace Turnierplan.App.Extensions;

internal static class EnumerableExtensions
{
    public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        return !enumerable.Any(predicate);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
    {
        return source.Where(x => x is not null)!;
    }
}
