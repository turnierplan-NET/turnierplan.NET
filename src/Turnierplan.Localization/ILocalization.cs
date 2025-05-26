using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Localization;

public interface ILocalization
{
    string Get(string key);

    string Get(string key, object arg0);

    string Get(string key, object arg0, object arg1);

    string Get(string key, object arg0, object arg1, object arg2);

    string Get(string key, params object[] args);

    string LocalizeGroupName(Group group);

    string LocalizeMatchDisplayName(Match match);

    string LocalizeTeamSelector(TeamSelectorBase teamSelector, Tournament tournament);
}
