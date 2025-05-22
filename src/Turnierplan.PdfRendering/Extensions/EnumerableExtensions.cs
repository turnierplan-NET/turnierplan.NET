namespace Turnierplan.PdfRendering.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<DateTime> WhereNotNull(this IEnumerable<DateTime?> source)
    {
        return source.Where(x => x is not null).Select(x => x!.Value);
    }
}
