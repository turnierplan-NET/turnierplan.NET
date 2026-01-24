namespace Turnierplan.Core.Test.Regression;

internal interface IRegressionTest
{
    void Execute();

    string Result { get; }
}
