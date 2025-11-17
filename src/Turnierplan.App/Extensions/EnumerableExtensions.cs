namespace Turnierplan.App.Extensions;

internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> enumerable)
    {
        public bool None(Func<T, bool> predicate)
        {
            return !enumerable.Any(predicate);
        }

        public IEnumerable<T> WhereNotNull()
        {
            return enumerable.Where(x => x is not null)!;
        }
    }
}
