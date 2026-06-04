namespace Turnierplan.App.Csv;

internal sealed class CsvResult : IResult
{
    private readonly Func<CsvWriter, Task> _generate;

    public CsvResult(Func<CsvWriter, Task> generate)
    {
        _generate = generate;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 200;
        httpContext.Response.Headers.ContentType = "text/csv";

        await using var csvWriter = new CsvWriter(httpContext.Response.Body);

        await _generate(csvWriter);
    }
}
