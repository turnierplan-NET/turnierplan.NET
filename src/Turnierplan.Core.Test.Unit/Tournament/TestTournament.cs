using Turnierplan.Core.Tournament;

namespace Turnierplan.Core.Test.Unit.Tournament;

internal static class TestTournament
{
    public static Core.Tournament.Tournament Default => Create();

    public static Core.Tournament.Tournament Create(long id = 1, string name = "", Visibility visibility = Visibility.Private, int publicPageViews = 0)
    {
        return new Core.Tournament.Tournament(id, new Core.PublicId.PublicId(), false, DateTime.UtcNow, name, visibility, publicPageViews);
    }
}
