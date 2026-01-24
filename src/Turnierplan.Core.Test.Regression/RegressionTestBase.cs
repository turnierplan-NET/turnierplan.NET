using System.Text;

namespace Turnierplan.Core.Test.Regression;

internal abstract class RegressionTestBase<TSubject> : IRegressionTest
    where TSubject : class
{
    private readonly StringBuilder _builder = new();
    private TSubject? _subject;
    private bool _finished;
    private int _saveStateIndex;

    public string Result
    {
        get => !_finished ? throw new InvalidOperationException("This test is not finished yet.") : field;
        private set;
    } = string.Empty;

    private TSubject Subject => _subject ?? throw new InvalidOperationException("This test is not currently running.");

    public void Execute()
    {
        if (_subject is not null)
        {
            throw new InvalidOperationException("This test was already executed.");
        }

        _subject = CreateSubject();
        AddSaveState();
        RunSteps();
        _finished = true;
        Result = _builder.ToString();
    }


    protected abstract TSubject CreateSubject();

    protected abstract void RunSteps();

    protected void Step(Action<TSubject> step)
    {
        step(Subject);

        AddSaveState();
    }

    protected TResult Step<TResult>(Func<TSubject, TResult> step)
    {
        var result = step(Subject);

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
        _builder.AppendLine();
    }
}
