namespace Turnierplan.Core.Test.Regression;

internal abstract class TournamentRegressionTestBase : RegressionTestBase<Tournament>
{
    protected TournamentRegressionTestBase()
    {
        Subject(new Tournament("reg_name"));
    }
}
