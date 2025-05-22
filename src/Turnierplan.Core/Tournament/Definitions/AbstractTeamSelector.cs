using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Tournament.Definitions;

/// <summary>
/// <c>AbstractTeamSelector</c> is a special type of team selector which does not have an association with an existing
/// group in the database. Rather, abstract team selectors target any collection of groups which might not have been
/// created yet and that therefore do not have a specific id. Abstract team selectors are used to configure the
/// generation of tournaments because the group ids are only generated during the generation process.
/// </summary>
/// <remarks>
/// If <paramref name="IsNthRanked"/> is <c>true</c>, <paramref name="PlacementRank"/> and <paramref name="OrdinalNumber"/>
/// must be specified and denote the team selector parameters as used in <see cref="GroupResultsNthRankedSelector"/>.
/// If <paramref name="IsNthRanked"/> is <c>false</c>, <paramref name="GroupIndex"/> and <paramref name="PlacementRank"/>
/// must be specified and will be used to generate a team selector to select the team with that position in a specific
/// group in the tournament.
/// </remarks>
public sealed record AbstractTeamSelector(bool IsNthRanked, int? GroupIndex, int PlacementRank, int? OrdinalNumber);
