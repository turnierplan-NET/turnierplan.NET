using System.Text;

namespace Turnierplan.App.Csv;

internal sealed class CsvWriter : IDisposable, IAsyncDisposable
{
    private readonly StreamWriter _target;
    private bool _isAtNewLine = true;
    private bool _headerWritten;
    private int? _columnCount;

    public CsvWriter(Stream target)
    {
        _target = new StreamWriter(target, encoding: Encoding.UTF8, leaveOpen: true);
    }

    public async Task WriteHeaderAsync(params string[] header)
    {
        ArgumentOutOfRangeException.ThrowIfZero(header.Length);

        if (_headerWritten)
        {
            throw new InvalidOperationException("The CSV header was already written.");
        }

        foreach (var cell in header)
        {
            await WriteCellAsync(cell);
        }

        await WriteNewLineAsync();

        _headerWritten = true;
        _columnCount = header.Length;
    }

    public async Task WriteRowAsync(params object?[] data)
    {
        if (!_headerWritten)
        {
            throw new InvalidOperationException("The CSV header has not been written yet.");
        }

        if (data.Length != _columnCount)
        {
            throw new InvalidOperationException("The row has a different column count than the header.");
        }

        foreach (var cell in data)
        {
            switch (cell)
            {
                case string cellString:
                    await WriteCellAsync(cellString);
                    break;
                case DateTime dateTime:
                    await WriteCellAsync($"{dateTime:O}");
                    break;
                default:
                    await WriteCellAsync(cell?.ToString() ?? string.Empty);
                    break;
            }
        }

        await WriteNewLineAsync();
    }

    public void Dispose()
    {
        _target.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _target.DisposeAsync();
    }

    private async Task WriteCellAsync(string cell)
    {
        if (!_isAtNewLine)
        {
            await _target.WriteAsync(",");
        }

        _isAtNewLine = false;

        if (cell.Length == 0)
        {
            return;
        }

        await _target.WriteAsync('"');

        foreach (var c in cell)
        {
            if (c == '"')
            {
                await _target.WriteAsync("\"\"");
            }
            else
            {
                await _target.WriteAsync(c);
            }
        }

        await _target.WriteAsync('"');
    }

    private async Task WriteNewLineAsync()
    {
        await _target.WriteLineAsync();
        _isAtNewLine = true;
    }
}
