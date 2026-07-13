using System.Diagnostics.CodeAnalysis;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Tournament.Definitions;

/// <summary>
/// <c>AbstractTeamSelector</c> is a special type of team selector which does not have an association with an existing
/// group in the database. Rather, abstract team selectors target any collection of groups which might not have been
/// created yet and that therefore do not have a specific id. Abstract team selectors are used to configure the
/// generation of tournaments because the group ids are only generated during the generation process.
/// </summary>
public sealed record AbstractTeamSelector
{
    /// <remarks>
    /// If <paramref name="isNthRanked"/> is <c>true</c>, <paramref name="placementRank"/> and <paramref name="ordinalNumber"/>
    /// must be specified and denote the team selector parameters as used in <see cref="GroupResultsNthRankedSelector"/>.
    /// If <paramref name="isNthRanked"/> is <c>false</c>, <paramref name="groupIndex"/> and <paramref name="placementRank"/>
    /// must be specified and will be used to generate a team selector to select the team with that position in a specific
    /// group in the tournament.
    /// </remarks>
    internal AbstractTeamSelector(bool isNthRanked, int? groupIndex, int placementRank, int? ordinalNumber)
    {
        IsNthRanked = isNthRanked;
        GroupIndex = groupIndex;
        PlacementRank = placementRank;
        OrdinalNumber = ordinalNumber;
    }

    [MemberNotNullWhen(true, nameof(OrdinalNumber))]
    [MemberNotNullWhen(false, nameof(GroupIndex))]
    public bool IsNthRanked { get; }

    public int? GroupIndex { get; }

    public int PlacementRank { get; }

    public int? OrdinalNumber { get; }
}
