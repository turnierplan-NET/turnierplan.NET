using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.Repositories;

internal sealed class TeamLinkRepository(TurnierplanContext context) : RepositoryBase<TeamLink, long>(context, context.TeamLinks), ITeamLinkRepository;
