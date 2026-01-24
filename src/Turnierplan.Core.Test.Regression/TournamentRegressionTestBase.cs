using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Regression;

internal abstract class TournamentRegressionTestBase : RegressionTestBase<Tournament.Tournament>
{
    protected TournamentRegressionTestBase()
    {
        Subject(new Tournament.Tournament(new Organization.Organization(string.Empty), "reg_name", Visibility.Private));
    }
}
