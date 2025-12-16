using System.Diagnostics.CodeAnalysis;

namespace Turnierplan.Core.Tournament;

internal interface IComparableTeam
{
    Team Team { get; }

    int Order { get; }

    int Priority { get; }

    TeamGroupStatistics Statistics { get; }

    [MemberNotNullWhen(true, nameof(AssociatedGroup))]
    bool HasAssociatedGroup { get; }

    Group? AssociatedGroup { get; }
}
