using Turnierplan.App.Csv;

namespace Turnierplan.App.Extensions;

internal static class ResultsExtensions
{
    extension(Results)
    {
        public static IResult Csv(Func<CsvWriter, Task> generate)
        {
            return new CsvResult(generate);
        }
    }
}
