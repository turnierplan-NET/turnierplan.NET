using System.Text;

namespace Turnierplan.Core.Test.Regression;

internal abstract class RegressionTestBase<TSubject> : IRegressionTest
    where TSubject : class
{
    private readonly StringBuilder _builder = new();
    private TSubject? _subject;
    private int _saveStateIndex;

    public string Result => _builder.ToString();

    protected void Subject(TSubject subject)
    {
        if (_subject is not null)
        {
            throw new InvalidOperationException($"The '{nameof(Subject)}()' method may only be called once.");
        }

        _subject = subject;

        AddSaveState();
    }

    protected void Step(Action<TSubject> step)
    {
        if (_subject is null)
        {
            throw new InvalidOperationException($"Call '{nameof(Subject)}()' first to initialize the test subject.");
        }

        step(_subject);

        AddSaveState();
    }

    protected TResult Step<TResult>(Func<TSubject, TResult> step)
    {
        if (_subject is null)
        {
            throw new InvalidOperationException($"Call '{nameof(Subject)}()' first to initialize the test subject.");
        }

        var result = step(_subject);

        AddSaveState();

        return result;
    }

    private void AddSaveState()
    {
        _builder.Append('[');
        _builder.Append(++_saveStateIndex);
        _builder.Append(']');
        _builder.AppendLine();
        _builder.AppendSubject(_subject);
    }
}
